using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.AI.Navigation;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

[System.Serializable]
public class StageSet
{
    public enum StageKind
    {
        [InspectorName("チュートリアル")]
        Tutorial,
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

        // === 前ステージの削除 ===
        if (_currentStageInstance != null)
        {
            // 旧ステージの NavMeshSurface を完全クリア
            var oldSurfaces = _currentStageInstance.GetComponentsInChildren<NavMeshSurface>();
            foreach (var s in oldSurfaces)
            {
                s.RemoveData();
            }

            Destroy(_currentStageInstance);
        }

        // === 新ステージ生成 ===
        StageSet stageSet = _stageSets[index];
        int prefabIndex = Random.Range(0, stageSet.stagePrefabs.Length);
        _currentStageInstance = Instantiate(stageSet.stagePrefabs[prefabIndex]);

        // === NavMesh 再生成 ===
        var newSurfaces = _currentStageInstance.GetComponentsInChildren<NavMeshSurface>();
        foreach (var surface in newSurfaces)
        {
            // 念のため古いデータを削除
            surface.RemoveData();

            // Bake
            surface.BuildNavMesh();
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

        // 配列の範囲外なら → シーン遷移
        if (nextIndex >= _stageSets.Length)
        {
            Debug.Log("すべてのステージが終了しました。リザルトシーンへ遷移します。");
            //SceneManager.LoadScene("ResultScene"); // 
            // 今後Fade関係の処理を呼ぶ予定(安田が追加してるっぽい。)
            BaseSceneController.instance.ChangeSceneWithFade(SceneType.ResultScene);
            return;
        }

        // まだステージが残っているなら次を生成
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
