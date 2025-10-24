using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StageEnemyInfo
{
    public List<EnemyGroup> enemyGroups; // このステージに出現する敵グループ
}

[System.Serializable]
public class EnemyGroup
{
    public List<SpawnEnemyData> spawnDataList; // すべてのSpawnDataを順に処理する
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
    [SerializeField] private Transform areaCenter;
    [SerializeField] private Vector3 areaSize = new Vector3(10, 0, 10);

    [Header("敵データ（SpawnData参照用）")]
    [SerializeField] private SpawnData enemySetData;

    [Header("ステージごとの敵配置データ")]
    [SerializeField] private List<StageEnemyInfo> stageEnemyInfos;

    private StageEnemyInfo currentStageInfo;
    private List<EnemyGroup> groups = new List<EnemyGroup>();

    [SerializeField] private float spawnInterval = 0.5f;

    [Header("GoalPosition 配列")]
    [SerializeField] private GameObject[] goalPositions;

    [Header("StageEvent 確率設定")]
    [SerializeField] private List<StageProbability> stageProbabilities = new List<StageProbability>();

    [Header("スキル関連")]
    public SkillSelectManager skillSelectManager;
    [HideInInspector] public SkillData.SkillElement nextSkillElement;

    [Header("スキル取得後に消す壁エフェクト")]
    public GameObject[] wallEffects;

    [HideInInspector] private bool skillSelectFinished = false;

    [SerializeField] public string beforeSkill;
    [SerializeField] public string aftereSkill;
    private BattleManager _battleManager;
    [HideInInspector] public StageSpawner stageSpawner;

    private int groupsClearedCount = 0;
    private Vector3 lastDeadEnemyPos;

    private void Start()
    {
        if (stageSpawner == null)
            stageSpawner = FindObjectOfType<StageSpawner>();

        if (stageSpawner != null && !string.IsNullOrEmpty(stageSpawner.beforeSkill))
        {
            beforeSkill = stageSpawner.beforeSkill;
        }

        AssignSkillsToGoals();
        if (stageEnemyInfos == null || stageEnemyInfos.Count == 0)
            return;

        currentStageInfo = stageEnemyInfos[Random.Range(0, stageEnemyInfos.Count)];

        StartCoroutine(HandleStageGroups(currentStageInfo));
    }

    private void AssignSkillsToGoals()
    {
        if (goalPositions == null || goalPositions.Length == 0) return;

        for (int i = 0; i < goalPositions.Length; i++)
        {
            StageProbability sp = i < stageProbabilities.Count ? stageProbabilities[i] : null;
            if (sp == null) continue;

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

    private IEnumerator HandleStageGroups(StageEnemyInfo stageInfo)
    {
        // 各グループを順に処理
        foreach (var group in stageInfo.enemyGroups)
        {
            yield return HandleGroupWaves(group);
        }

        Debug.Log("すべてのグループがクリアされました。");
    }

    private IEnumerator HandleGroupWaves(EnemyGroup group)
    {
        if (group.spawnDataList == null || group.spawnDataList.Count == 0)
            yield break;

        // 複数のSpawnDataすべてを順に処理
        foreach (var spawnData in group.spawnDataList)
        {
            if (spawnData == null) continue;

            foreach (var wave in spawnData.waveEnemyDataList)
            {
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
                    Debug.Log("Aaaaa");
                }

                yield return StartCoroutine(WaitForWaveClear(spawned));
            }
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

        if (skillSelectManager != null && goalPositions.Length > 0)
        {
            Vector3 spawnPos = lastDeadEnemyPos != Vector3.zero ? lastDeadEnemyPos : areaCenter.position;

            StageEventType stageEventType = StageEventType.Fire;
            if (!string.IsNullOrEmpty(beforeSkill))
            {
                if (System.Enum.TryParse(beforeSkill, out StageEventType parsed))
                {
                    stageEventType = parsed;
                }
            }

            switch (stageEventType)
            {
                case StageEventType.Fire:
                    skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Fire, this);
                    break;
                case StageEventType.Water:
                    skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Water, this);
                    break;
                case StageEventType.Wind:
                    skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Wind, this);
                    break;
                case StageEventType.Thunder:
                    skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Thunder, this);
                    break;
                case StageEventType.Freeze:
                    skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Freeze, this);
                    break;
                case StageEventType.Poison:
                    skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Poison, this);
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
            }
        }

        StartCoroutine(WaitForSkillSelectFinish());
    }

    private IEnumerator WaitForSkillSelectFinish()
    {
        yield return new WaitUntil(() => skillSelectFinished);

        if (stageSpawner != null)
            stageSpawner.AcquireSkill(nextSkillElement);

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

    public void OnGoalReached(int goalIndex)
    {
        if (goalPositions == null || goalIndex < 0 || goalIndex >= goalPositions.Length) return;

        StageEventType eventType = stageProbabilities[goalIndex].eventType;
        SkillData.SkillElement selectedSkill = ConvertStageEventToSkill(eventType);

        if (stageSpawner != null)
            stageSpawner.OnPathSelected(selectedSkill);
    }

    public void OnSkillSelectFinished()
    {
        skillSelectFinished = true;

        foreach (var wall in wallEffects)
        {
            if (wall != null)
            {
                Destroy(wall);
            }
        }
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
