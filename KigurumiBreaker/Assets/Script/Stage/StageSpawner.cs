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
    private string _beforeSkill;
    private string _afterSkill;
    private List<SkillData.SkillElement> _acquiredSkills = new List<SkillData.SkillElement>();

    [Header("WaveSpawner用 SkillSelectManager")]
    [SerializeField] private SkillSelectManager _skillSelectManager;
    [SerializeField] private BattleManager _battleManager;

    private int _currentStageIndex = 0;
    private GameObject _currentStageInstance;

    /// <summary>
    /// 指定したインデックスのステージを生成する
    /// </summary>
    /// <param name="index"></param>
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
            waveSpawner.SetSkillSelect(_skillSelectManager);
            waveSpawner.SetBattleManager(_battleManager);

            // 前ステージで会得したスキルを渡す
            if (_acquiredSkills.Count > 0)
            {
                waveSpawner.SetBeforeSkill(string.Join(",", _acquiredSkills));
            }

            // StageSpawner の参照を渡す
            waveSpawner.SetStageSpawner(this);
        }

        _currentStageIndex = index;
    }

    /// <summary>
    /// 次のステージを生成する
    /// </summary>
    public void NextStage()
    {
        int nextIndex = _currentStageIndex + 1;
        if (nextIndex >= _stageSets.Length)
        {
            return;
        }

        SpawnStage(nextIndex);
    }

    private void Start()
    {
        SpawnStage(0);
    }

    /// <summary>
    /// WaveSpawner から通知される
    /// </summary>
    /// <param name="selectedSkill"></param>
    public void OnPathSelected(SkillData.SkillElement selectedSkill)
    {
        _beforeSkill = selectedSkill.ToString();
    }

    /// <summary>
    /// WaveSpawner からスキル会得通知
    /// </summary>
    /// <param name="acquiredSkill"></param>
    public void AcquireSkill(SkillData.SkillElement acquiredSkill)
    {
        if (!_acquiredSkills.Contains(acquiredSkill))
        {
            _acquiredSkills.Add(acquiredSkill);
            _afterSkill = acquiredSkill.ToString();
        }
    }

    public string GetBeforeSkill()
    {
        if (string.IsNullOrEmpty(_beforeSkill)) Debug.Break();

        return _beforeSkill;
    }
}
