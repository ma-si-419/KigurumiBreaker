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
public class StageEnemyInfo
{
    public string stageInfoName;               // ステージ名
    public List<EnemyGroup> enemyGroups;       // このステージに出現する敵グループ
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
    [SerializeField] private Transform areaCenter;
    [SerializeField] private Vector3 areaSize = new Vector3(10, 0, 10);

    [Header("敵データ（SpawnData）")]
    [SerializeField] private SpawnData enemySetData;

    [Header("グループ設定")]
    [SerializeField]
    private List<StageEnemyInfo> stageEnemyInfos;  // ステージごとの敵配置データ

    private StageEnemyInfo currentStageInfo;       // 今回選ばれたステージ情報

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

    [SerializeField] public string beforeSkill; // 前ステージで取得したスキル
    [SerializeField] public string aftereSkill;
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
            beforeSkill = stageSpawner.beforeSkill;
        }

        AssignSkillsToGoals();
        if (stageEnemyInfos == null || stageEnemyInfos.Count == 0)
        {
            return;
        }

        // ステージ内の敵情報からランダム選択
        currentStageInfo = stageEnemyInfos[Random.Range(0, stageEnemyInfos.Count)];

        Debug.Log($"選ばれたステージ: {currentStageInfo.stageInfoName}");

        StartCoroutine(HandleStageGroups(currentStageInfo));
    }

    private void AssignSkillsToGoals()
    {
        if (goalPositions == null || goalPositions.Length == 0) return;

        for (int i = 0; i < goalPositions.Length; i++)
        {
            StageProbability sp = i < stageProbabilities.Count ? stageProbabilities[i] : null;
            if (sp == null) continue;

            // 最初のGoalをnextSkillElementに設定
            if (i == 0)
                nextSkillElement = ConvertStageEventToSkill(sp.eventType);
        }
    }

    /// <summary>
    /// StageEventType を SkillData.SkillElement に変換
    /// </summary>
    /// <param name="eventType"></param>
    /// <returns></returns>
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
        foreach (var group in stageInfo.enemyGroups)
        {
            yield return HandleGroupWaves(group);
        }

        Debug.Log("すべてのグループがクリアされました。");
    }


    /// <summary>
    /// グループ内のウェーブを順次処理
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
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

    /// <summary>
    /// ウェーブ内の敵が全滅するまで待機
    /// </summary>
    /// <param name="spawned"></param>
    /// <returns></returns>
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

    /// <summary>
    /// グループが全滅したときの処理
    /// </summary>
    private void OnGroupCleared()
    {
        groupsClearedCount++;
        if (groupsClearedCount < groups.Count) return;


        if (skillSelectManager != null && goalPositions.Length > 0)
        {
            Vector3 spawnPos = lastDeadEnemyPos != Vector3.zero ? lastDeadEnemyPos : areaCenter.position;

            // beforskill を StageEventType に変換
            StageEventType stageEventType = StageEventType.Fire; // デフォルト
            if (!string.IsNullOrEmpty(beforeSkill))
            {
                if (System.Enum.TryParse(beforeSkill, out StageEventType parsed))
                {
                    stageEventType = parsed;
                }
            }

            // 変換した StageEventType に応じて処理
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

    /// <summary>
    /// スキル選択完了まで待機
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForSkillSelectFinish()
    {
        yield return new WaitUntil(() => skillSelectFinished);

        if (stageSpawner != null)
            stageSpawner.AcquireSkill(nextSkillElement);

        skillSelectFinished = false;
        groupsClearedCount = 0;
        lastDeadEnemyPos = Vector3.zero;
    }

    /// <summary>
    /// NavMesh上のランダム位置を取得
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Goal に到達したら StageSpawner にスキル通知
    /// </summary>
    /// <param name="goalIndex"></param>
    public void OnGoalReached(int goalIndex)
    {
        if (goalPositions == null || goalIndex < 0 || goalIndex >= goalPositions.Length) return;

        StageEventType eventType = stageProbabilities[goalIndex].eventType;
        SkillData.SkillElement selectedSkill = ConvertStageEventToSkill(eventType);

        if (stageSpawner != null)
        {
            stageSpawner.OnPathSelected(selectedSkill);
        }
    }

    /// <summary>
    /// スキル選択完了時の処理
    /// </summary>
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
    }

    /// <summary>
    /// BattleManager をセット
    /// </summary>
    /// <param name="manager"></param>
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