using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.AI.Navigation;
using JetBrains.Annotations;

[System.Serializable]
public class StageSet
{
    public enum StageKind
    {
        [InspectorName("テスト")]
        Test,
        [InspectorName("ステージ1　森")]
        Forest,
        [InspectorName("ステージ2　洞窟")]
        Cave
    }

    private const int SECTIONS_MAX = 5;

    [Header("ステージ情報")]
    [SerializeField] private StageKind _stageKind;

    [HideInInspector][SerializeField] private int _index;

    [Header("出てくるステージの種類")]
    public GameObject[] stagePrefabs;
    public void SetIndex(int index)
    {
        _index = index;
    }

    // 読み取り専用プロパティ
    public StageKind stageKind => _stageKind;
}

public class StageSpawner : MonoBehaviour
{
    [Header("全体のステージすべて")]
    [SerializeField] private StageSet[] _stageSets;
    [Header("プレイヤーのTransform")]
    [SerializeField] private Transform _player;

    private string _beforeSkill;
    private string _afterSkill;
    private List<SkillData.SkillElement> _acquiredSkills = new List<SkillData.SkillElement>();

    [Header("スキル選択マネージャー")]
    [SerializeField] private SkillSelectManager _skillSelectManager;

    [Header("バトルマネージャー")]
    [SerializeField] private BattleManager _battleManager;

    private int _currentStageIndex = 0;
    private GameObject _currentStageInstance;

    // ステージデータのインデックスを設定
    private void OnValidate()
    {
        ///// ステージデータの設定 /////
        // ステージ森から始める
        StageSet.StageKind stageKind = StageSet.StageKind.Forest;

        // ステージごとにセクションを設定
        int sectionNumber = 0;

        for (int i = 0; i < _stageSets.Length; i++)
        {
            if (_stageSets[i] != null)
            {
                // ステージの種類が変わったらステージ番号を更新し、セクション番号をリセット
                if (stageKind != _stageSets[i].stageKind)
                {
                    stageKind = _stageSets[i].stageKind;
                    sectionNumber = 0;
                }

                sectionNumber++;
                // ステージ1から始めたいので+1する
                _stageSets[i].SetIndex(sectionNumber);
            }
        }
        ////////////////////////////////

    }

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

        return _beforeSkill;
    }
}
