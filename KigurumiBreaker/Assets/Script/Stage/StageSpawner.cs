using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.AI.Navigation;

[System.Serializable]
public class StageSet
{
    public GameObject[] stagePrefabs;
}

public class StageSpawner : MonoBehaviour
{
    [SerializeField] private StageSet[] _stageSets;
    [SerializeField] private Transform _player;

    [Header("スキル情報")]
    [SerializeField] public string beforeSkill;   // 現在持っているスキル
    [SerializeField] public string afterSkill;
    public List<SkillData.SkillElement> acquiredSkills = new List<SkillData.SkillElement>();

    [Header("WaveSpawner用 SkillSelectManager")]
    [SerializeField] private SkillSelectManager _skillSelectManager;
    [SerializeField] private BattleManager _battleManager;

    private int _currentStageIndex = 0;
    private GameObject _currentStageInstance;

    public void SpawnStage(int index)
    {
        if (index < 0 || index >= _stageSets.Length) return;

        // 前ステージを破棄
        if (_currentStageInstance != null)
        {
            Destroy(_currentStageInstance);
        }

        StageSet stageSet = _stageSets[index];

        // ランダムでPrefab選択
        int prefabIndex = Random.Range(0, stageSet.stagePrefabs.Length);
        _currentStageInstance = Instantiate(stageSet.stagePrefabs[prefabIndex]);

        // NavMesh再生成
        var navMeshSurface = _currentStageInstance.GetComponent<NavMeshSurface>();
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }

        // Player初期位置設定
        Transform spawnPoint = _currentStageInstance.transform.Find("SpawnPoint");
        if (spawnPoint != null && _player != null)
        {
            _player.position = spawnPoint.position;
            _player.rotation = spawnPoint.rotation;
        }

        // WaveSpawner に SkillSelectManager とスキル情報をセット
        WaveSpawner[] waveSpawners = _currentStageInstance.GetComponentsInChildren<WaveSpawner>();
        foreach (var waveSpawner in waveSpawners)
        {
            if (waveSpawner.skillSelectManager == null && _skillSelectManager != null)
            {
                waveSpawner.skillSelectManager = _skillSelectManager;
                waveSpawner.SetBattleManager(_battleManager);
            }

            // 前ステージで会得したスキルを渡す
            if (acquiredSkills.Count > 0)
            {
                waveSpawner.beforskill = string.Join(",", acquiredSkills);
            }

            // StageSpawner の参照を渡す
            waveSpawner.stageSpawner = this;
        }

        _currentStageIndex = index;
        Debug.Log($"Stage {index + 1} を生成: {stageSet.stagePrefabs[prefabIndex].name}");
    }

    public void NextStage()
    {
        int nextIndex = _currentStageIndex + 1;
        if (nextIndex >= _stageSets.Length)
        {
            Debug.Log("全ステージクリア！");
            return;
        }

        SpawnStage(nextIndex);
    }

    private void Start()
    {
        SpawnStage(0);
    }

    // WaveSpawner から通知される
    public void OnPathSelected(SkillData.SkillElement selectedSkill)
    {
        beforeSkill = selectedSkill.ToString();
        Debug.Log($"StageSpawner: プレイヤーが選択した道のスキルは {beforeSkill}");
    }

    // WaveSpawner からスキル会得通知
    public void AcquireSkill(SkillData.SkillElement acquiredSkill)
    {
        if (!acquiredSkills.Contains(acquiredSkill))
        {
            acquiredSkills.Add(acquiredSkill);
            afterSkill = acquiredSkill.ToString();
            Debug.Log($"StageSpawner: 会得スキルに {afterSkill} を追加");
        }
    }
}
