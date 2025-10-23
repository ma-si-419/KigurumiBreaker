// WaveSpawner.cs
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PopEnemy
{
    public EnemyKind spawnKind;
    public Vector3 spawnPosition;
    public bool randomizePosition = true;
}

[System.Serializable]
public class EnemyWave
{
    public List<PopEnemy> popEnemies = new List<PopEnemy>();
}

[System.Serializable]
public class EnemyGroup
{
    public string groupName = "Group";
    public List<EnemyWave> waves = new List<EnemyWave>();
}

[System.Serializable]
public class StageProbability
{
    public StageEventType eventType;
    [Range(0f, 1f)] public float probability;
}

public enum StageEventType
{
    Shop,
    Gold,
    Heal,
    Treasure,
    Fire,
    Water,
    Wind,
    Thunder,
    Freeze,
    Poison
}

public class WaveSpawner : MonoBehaviour
{
    [Header("全体の出現範囲設定")]
    public Transform areaCenter;
    public Vector3 areaSize = new Vector3(10, 0, 10);

    [Header("敵データ（SpawnData）")]
    public SpawnData enemySetData;

    [Header("グループ設定")]
    public List<EnemyGroup> groups = new List<EnemyGroup>();

    [Header("出現間隔")]
    public float spawnInterval = 0.5f;

    [Header("GoalPosition 配列")]
    public GameObject[] goalPositions;

    [Header("StageEvent 確率設定")]
    public List<StageProbability> stageProbabilities = new List<StageProbability>();

    [Header("スキル関連")]
    public SkillSelectManager skillSelectManager;
    [HideInInspector] public SkillData.SkillElement nextSkillElement;

    [Header("スキル取得後に消す壁エフェクト")]
    public GameObject[] wallEffects;

    [HideInInspector] public bool isAllWavesCleared = false;
    [HideInInspector] public bool skillSelectFinished = false;

    [SerializeField] public string beforskill; // 前ステージで取得したスキル
    [SerializeField] public string afterskill;
    private BattleManager _battleManager;

    [HideInInspector] public StageSpawner stageSpawner;

    private int groupsClearedCount = 0;
    private Vector3 lastDeadEnemyPos;

    private void Start()
    {
        // StageSpawner を取得
        if (stageSpawner == null)
            stageSpawner = FindObjectOfType<StageSpawner>();

        if (stageSpawner != null && !string.IsNullOrEmpty(stageSpawner.beforeSkill))
        {
            beforskill = stageSpawner.beforeSkill;
            Debug.Log($"WaveSpawner: 前ステージスキル {beforskill} を取得");
        }

        AssignSkillsToGoals();

        foreach (var group in groups)
        {
            StartCoroutine(HandleGroupWaves(group));
        }
    }

    private void AssignSkillsToGoals()
    {
        if (goalPositions == null || goalPositions.Length == 0) return;

        for (int i = 0; i < goalPositions.Length; i++)
        {
            StageProbability sp = i < stageProbabilities.Count ? stageProbabilities[i] : null;
            if (sp == null) continue;

            Debug.Log($"GoalPosition[{i}] ({goalPositions[i].name}) に StageEventType {sp.eventType} を割り当て");

            // 最初のGoalをnextSkillElementに設定
            if (i == 0)
                nextSkillElement = ConvertStageEventToSkill(sp.eventType);
        }
    }

    private SkillData.SkillElement ConvertStageEventToSkill(StageEventType eventType)
    {
        switch (eventType)
        {
            case StageEventType.Fire: return SkillData.SkillElement.Fire;
            case StageEventType.Water: return SkillData.SkillElement.Water;
            case StageEventType.Wind: return SkillData.SkillElement.Wind;
            case StageEventType.Thunder: return SkillData.SkillElement.Thunder;
            case StageEventType.Freeze: return SkillData.SkillElement.Freeze;
            case StageEventType.Poison: return SkillData.SkillElement.Poison;
            default: return SkillData.SkillElement.Fire;
        }
    }

    private IEnumerator HandleGroupWaves(EnemyGroup group)
    {
        for (int w = 0; w < group.waves.Count; w++)
        {
            var wave = group.waves[w];
            List<GameObject> spawned = new List<GameObject>();

            foreach (var pop in wave.popEnemies)
            {
                GameObject prefabToSpawn = enemySetData?.GetPrefabByKind(pop.spawnKind);
                if (prefabToSpawn == null) continue;

                Vector3 spawnPos = pop.randomizePosition ? GetRandomNavMeshPosition() : pop.spawnPosition;
                GameObject enemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                spawned.Add(enemy);
                _battleManager.AddEnemy(enemy);
                enemy.GetComponent<EnemyBase>().SetBattleManager(_battleManager);

                yield return new WaitForSeconds(spawnInterval);
            }

            yield return StartCoroutine(WaitForWaveClear(spawned));
        }

        OnGroupCleared();
    }

    private IEnumerator WaitForWaveClear(List<GameObject> spawned)
    {
        Vector3 lastPos = Vector3.zero;

        while (spawned.Count > 0)
        {
            for (int i = spawned.Count - 1; i >= 0; i--)
            {
                var go = spawned[i];
                if (go == null) spawned.RemoveAt(i);
                else lastPos = go.transform.position;
            }
            yield return null;
        }

        lastDeadEnemyPos = lastPos;
    }

    private void OnGroupCleared()
    {
        groupsClearedCount++;
        if (groupsClearedCount < groups.Count) return;

        isAllWavesCleared = true;

        if (skillSelectManager != null && goalPositions.Length > 0)
        {
            Vector3 spawnPos = lastDeadEnemyPos != Vector3.zero ? lastDeadEnemyPos : areaCenter.position;

            // beforskill を StageEventType に変換
            StageEventType stageEventType = StageEventType.Fire; // デフォルト
            if (!string.IsNullOrEmpty(beforskill))
            {
                if (System.Enum.TryParse(beforskill, out StageEventType parsed))
                {
                    stageEventType = parsed;
                }
            }

            // 変換した StageEventType に応じて処理
            switch (stageEventType)
            {
                case StageEventType.Fire:
                    skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Fire, this);
                    Debug.Log($"Fireスキル提供: {SkillData.SkillElement.Fire} at {spawnPos}");
                    break;
                case StageEventType.Water:
                    skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Water, this);
                    Debug.Log($"Waterスキル提供: {SkillData.SkillElement.Water} at {spawnPos}");
                    break;
                case StageEventType.Wind:
                    skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Wind, this);
                    Debug.Log($"Windスキル提供: {SkillData.SkillElement.Wind} at {spawnPos}");
                    break;
                case StageEventType.Thunder:
                    skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Thunder, this);
                    Debug.Log($"Thunderスキル提供: {SkillData.SkillElement.Thunder} at {spawnPos}");
                    break;
                case StageEventType.Freeze:
                    skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Freeze, this);
                    Debug.Log($"Freezeスキル提供: {SkillData.SkillElement.Freeze} at {spawnPos}");
                    break;
                case StageEventType.Poison:
                    skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Poison, this);
                    Debug.Log($"Poisonスキル提供: {SkillData.SkillElement.Poison} at {spawnPos}");
                    break;
                case StageEventType.Shop:
                    Debug.Log("Shopイベント発生：ショップUIを開く処理をここに実装予定");
                    break;
                case StageEventType.Gold:
                    Debug.Log("Goldイベント発生：ゴールド付与処理");
                    break;
                case StageEventType.Heal:
                    Debug.Log("Healイベント発生：HP回復処理");
                    break;
                case StageEventType.Treasure:
                    Debug.Log("Treasureイベント発生：宝箱出現処理");
                    break;
            }
        }

        StartCoroutine(WaitForSkillSelectFinish());
    }

    private IEnumerator WaitForSkillSelectFinish()
    {
        yield return new WaitUntil(() => skillSelectFinished);

        if (stageSpawner != null)
            stageSpawner.AcquireSkill(nextSkillElement);

        isAllWavesCleared = false;
        skillSelectFinished = false;
        groupsClearedCount = 0;
        lastDeadEnemyPos = Vector3.zero;
    }

    private Vector3 GetRandomNavMeshPosition()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = areaCenter.position + new Vector3(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                0f,
                Random.Range(-areaSize.z / 2, areaSize.z / 2)
            );

            Collider[] cols = Physics.OverlapSphere(randomPos, 1f);
            bool invalid = false;
            foreach (var col in cols)
                if (col.CompareTag("Wall"))
                {
                    invalid = true;
                    break;
                }
            if (invalid) continue;

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }
        return areaCenter.position;
    }

    // Goal に到達したら StageSpawner にスキル通知
    public void OnGoalReached(int goalIndex)
    {
        if (goalPositions == null || goalIndex < 0 || goalIndex >= goalPositions.Length) return;

        StageEventType eventType = stageProbabilities[goalIndex].eventType;
        SkillData.SkillElement selectedSkill = ConvertStageEventToSkill(eventType);

        if (stageSpawner != null)
        {
            stageSpawner.OnPathSelected(selectedSkill);
            Debug.Log($"WaveSpawner: Goal {goalIndex} に到達したのでスキル {selectedSkill} を StageSpawner に通知");
        }
    }

    public void OnSkillSelectFinished()
    {
        skillSelectFinished = true;

        // 壁エフェクトを消す
        foreach (var wall in wallEffects)
        {
            if (wall != null)
            {
                Destroy(wall);
            }
        }
        Debug.Log("壁エフェクトをすべて破棄しました");
    }

    public void SetBattleManager(BattleManager manager)
    {
        _battleManager = manager;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (areaCenter == null) return;
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawCube(areaCenter.position, areaSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(areaCenter.position, areaSize);
    }
#endif
}