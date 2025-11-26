using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using static SpecialAttackCameraMoveData;

[System.Serializable]
public class EnemyPopPatern
{
    [Header("敵の配置データ")]
    [SerializeField] private List<EnemyPopGroup> _enemyPopGroups;

    [HideInInspector][SerializeField] private int _index = 0;

    public void SetIndex(int index)
    {
        _index = index;
    }

    // 読み取り専用プロパティ
    public int index => _index;
    public List<EnemyPopGroup> enemyPopGroups => _enemyPopGroups;
}



[System.Serializable]
public class StageProbability
{
    [SerializeField] private StageEventType _eventType;
    [Range(0f, 1f)]  public float probability;


    // 読み取り専用プロパティ
    public StageEventType eventType => _eventType;
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
    [SerializeField] private Transform _areaCenter;
    [SerializeField] private Vector3 _areaSize = new Vector3(10, 0, 10);

    [Header("敵データ（SpawnData参照用）")]
    [SerializeField] private SpawnData _enemySetData;

    [Header("どのステージデータを呼び出すか")]
    [SerializeField] private WaveData _waveData; 


    private EnemyPopGroup _currentStageInfo;
    private List<EnemyPopGroup> _groups = new List<EnemyPopGroup>();

    [SerializeField] private float _spawnInterval = 0.5f;

    [Header("GoalPosition 配列")]
    [SerializeField] private GameObject[] _goalPositions;

    [Header("StageEvent 確率設定")]
    [SerializeField] private List<StageProbability> _stageProbabilities = new List<StageProbability>();

    [Header("スキル関連")]
    private SkillSelectManager _skillSelectManager;
    [SerializeField] private SkillData.SkillElement _nextSkillElement;

    [Header("スキル取得後に消す壁エフェクト")]
    [SerializeField] private GameObject[] _wallEffects;

    private bool _skillSelectFinished = false;

    private string _beforeSkill;
    private string _aftereSkill;
    private BattleManager _battleManager;
    private StageSpawner stageSpawner;

    private int _groupsClearedCount = 0;
    private Vector3 _lastDeadEnemyPos;

    private void OnValidate()
    {
        if (_waveData == null) return;

        var groups = _waveData.waveEnemyDataList;
        if (groups == null) return;

        for (int i = 0; i < groups.Count; i++)
        {
            groups[i].SetIndex(i);

            var spawnList = groups[i].spawnDataList;
            if (spawnList == null) continue;

        }
    }

    private void Start()
    {
        if (_waveData == null || _waveData.waveEnemyDataList.Count == 0)
        {
            Debug.LogError("WaveData が設定されていないか、waveEnemyDataList が空です");
            return;
        }

        // ランダムで1つのグループを選択
        _currentStageInfo = _waveData.waveEnemyDataList[Random.Range(0, _waveData.waveEnemyDataList.Count)];

        StartCoroutine(HandleStageGroups(_currentStageInfo));
        AssignSkillsToGoals();
    }

    private void AssignSkillsToGoals()
    {

        if (_goalPositions == null || _goalPositions.Length == 0) return;

        for (int i = 0; i < _goalPositions.Length; i++)
        {
            StageProbability sp = i < _stageProbabilities.Count ? _stageProbabilities[i] : null;
            if (sp == null) continue;

            if (i == 0)
                _nextSkillElement = ConvertStageEventToSkill(sp.eventType);
              Debug.Log("Assign skill: " + sp.eventType);
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
            default:
                Debug.LogError("未対応のイベント: " + eventType);
                return SkillData.SkillElement.Fire;

        }
    }

    private IEnumerator HandleStageGroups(EnemyPopGroup group)
    {
        if (group == null || group.spawnDataList == null || group.spawnDataList.Count == 0)
        {
            Debug.LogWarning("EnemyPopGroup が空です");
            yield break;
        }

        foreach (var wave in group.spawnDataList)
        {
            if (wave == null || wave.popEnemies == null || wave.popEnemies.Count == 0) continue;

            List<GameObject> spawned = new List<GameObject>();

            foreach (var pop in wave.popEnemies)
            {
                if (pop == null) continue;

                GameObject prefabToSpawn = _enemySetData?.GetPrefabByKind(pop.spawnKind);
                if (prefabToSpawn == null) continue;

                Vector3 spawnPos = pop.randomizePosition ? GetRandomNavMeshPosition() : pop.spawnPosition;
                GameObject enemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

                spawned.Add(enemy);

                _battleManager?.AddEnemy(enemy);
                enemy.GetComponent<EnemyBase>()?.SetBattleManager(_battleManager);

                yield return new WaitForSeconds(_spawnInterval);
            }

            yield return StartCoroutine(WaitForWaveClear(spawned));
        }

        OnGroupCleared();
    }


    private IEnumerator HandleGroupWaves(EnemyPopGroup group)
    {
        if (group == null || group.spawnDataList == null || group.spawnDataList.Count == 0)
            yield break;

        // 複数の WaveEnemyData を順に処理
        foreach (var wave in group.spawnDataList) // WaveEnemyData のリスト
        {
            if (wave == null || wave.popEnemies == null || wave.popEnemies.Count == 0)
                continue;

            List<GameObject> spawned = new List<GameObject>();

            // 各敵を出現
            foreach (var pop in wave.popEnemies)
            {
                if (pop == null) continue;

                GameObject prefabToSpawn = _enemySetData?.GetPrefabByKind(pop.spawnKind);
                if (prefabToSpawn == null) continue;

                Vector3 spawnPos = pop.randomizePosition ? GetRandomNavMeshPosition() : pop.spawnPosition;
                GameObject enemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

                spawned.Add(enemy);

                // バトルマネージャーに追加
                _battleManager.AddEnemy(enemy);

                // EnemyBase に BattleManager 設定
                var enemyBase = enemy.GetComponent<EnemyBase>();
                if (enemyBase != null)
                    enemyBase.SetBattleManager(_battleManager);

                yield return new WaitForSeconds(_spawnInterval);
            }

            // このウェーブの敵が全滅するまで待機
            yield return StartCoroutine(WaitForWaveClear(spawned));
        }

        // グループクリア後の処理
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

        _lastDeadEnemyPos = lastPos;
    }

    private void OnGroupCleared()
    {
        _groupsClearedCount++;
        if (_groupsClearedCount < _groups.Count) return;

        if (_skillSelectManager != null && _goalPositions.Length > 0)
        {
            Vector3 spawnPos = _lastDeadEnemyPos != Vector3.zero ? _lastDeadEnemyPos : _areaCenter.position;

            StageEventType stageEventType = StageEventType.Fire;
            if (!string.IsNullOrEmpty(_beforeSkill))
            {
                if (System.Enum.TryParse(_beforeSkill, out StageEventType parsed))
                {
                    stageEventType = parsed;
                }
            }

            switch (stageEventType)
            {
                case StageEventType.Fire:
                    _skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Fire, this);
                    break;
                case StageEventType.Water:
                    _skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Water, this);
                    break;
                case StageEventType.Wind:
                    _skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Wind, this);
                    break;
                case StageEventType.Thunder:
                    _skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Thunder, this);
                    break;
                case StageEventType.Freeze:
                    _skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Freeze, this);
                    break;
                case StageEventType.Poison:
                    _skillSelectManager.PopSkillGetObject(spawnPos, SkillData.SkillElement.Poison, this);
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
        yield return new WaitUntil(() => _skillSelectFinished);

        if (stageSpawner != null)
            stageSpawner.AcquireSkill(_nextSkillElement);

        _skillSelectFinished = false;
        _groupsClearedCount = 0;
        _lastDeadEnemyPos = Vector3.zero;
    }

    private Vector3 GetRandomNavMeshPosition()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = _areaCenter.position + new Vector3(
                Random.Range(-_areaSize.x / 2, _areaSize.x / 2),
                0f,
                Random.Range(-_areaSize.z / 2, _areaSize.z / 2)
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
        return _areaCenter.position;
    }

    public void OnGoalReached(int goalIndex)
    {
        if (_goalPositions == null || goalIndex < 0 || goalIndex >= _goalPositions.Length) return;

        StageEventType eventType = _stageProbabilities[goalIndex].eventType;
        SkillData.SkillElement selectedSkill = ConvertStageEventToSkill(eventType);

        //本当にスキルが選択されたのかの確認
        Debug.Log("GoalReachedで選択されたスキル: " + selectedSkill.ToString());

        if (stageSpawner != null)
            stageSpawner.OnPathSelected(selectedSkill);
    }

    public void OnSkillSelectFinished()
    {
        _skillSelectFinished = true;

        foreach (var wall in _wallEffects)
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
    public void SetBeforeSkill(string skillName)
    {
        _beforeSkill = skillName;
        Debug.Log("SetBeforeSkill 呼び出し: " + skillName);
    }
    public void SetStageSpawner(StageSpawner spawner)
    {
        stageSpawner = spawner;
    }

    public void SetSkillSelect(SkillSelectManager selectManager)
    {
        _skillSelectManager = FindObjectOfType<SkillSelectManager>();
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_areaCenter == null) return;
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawCube(_areaCenter.position, _areaSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_areaCenter.position, _areaSize);
    }
#endif
}
