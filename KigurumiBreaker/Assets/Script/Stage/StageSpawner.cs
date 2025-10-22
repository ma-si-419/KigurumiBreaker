using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;

[System.Serializable]
public class StageSet
{
    public GameObject[] stagePrefabs;
}

public class StageSpawner : MonoBehaviour
{
    [SerializeField] private StageSet[] stageSets;
    [SerializeField] private Transform player;

    [Header("スキル情報")]
    [SerializeField] public string beforskill; // 現在持っているスキル
    [SerializeField] public string afterskill;
    public List<SkillData.SkillElement> acquiredSkills = new List<SkillData.SkillElement>();

    [Header("WaveSpawner用 SkillSelectManager")]
    [SerializeField] private SkillSelectManager skillSelectManager;
    [SerializeField] private BattleManager battleManager;

    private int currentStageIndex = 0;
    private GameObject currentStageInstance;

    public void SpawnStage(int index)
    {
        if (index < 0 || index >= stageSets.Length) return;

        // 前ステージを破棄
        if (currentStageInstance != null)
            Destroy(currentStageInstance);

        StageSet set = stageSets[index];

        // ランダムでPrefab選択
        int prefabIndex = Random.Range(0, set.stagePrefabs.Length);
        currentStageInstance = Instantiate(set.stagePrefabs[prefabIndex]);

        // NavMesh再生成
        var surface = currentStageInstance.GetComponent<NavMeshSurface>();
        if (surface != null) surface.BuildNavMesh();

        // Player初期位置
        Transform spawn = currentStageInstance.transform.Find("SpawnPoint");
        if (spawn != null && player != null)
        {
            player.position = spawn.position;
            player.rotation = spawn.rotation;
        }

        // WaveSpawner に SkillSelectManager とスキル情報をセット
        WaveSpawner[] waveSpawners = currentStageInstance.GetComponentsInChildren<WaveSpawner>();
        foreach (var wave in waveSpawners)
        {
            if (wave.skillSelectManager == null && skillSelectManager != null)
            {
                wave.skillSelectManager = skillSelectManager;
                wave.SetBattleManager(battleManager);
            }

            // 前ステージで会得したスキルを渡す
            if (acquiredSkills.Count > 0)
            {
                wave.beforskill = string.Join(",", acquiredSkills);
            }

            // StageSpawner の参照も渡す
            wave.stageSpawner = this;
        }

        currentStageIndex = index;
        Debug.Log($"Stage {index + 1} を生成: {set.stagePrefabs[prefabIndex].name}");
    }

    public void NextStage()
    {
        int nextIndex = currentStageIndex + 1;
        if (nextIndex >= stageSets.Length)
        {
            Debug.Log("全ステージクリア！");
            return;
        }
        SpawnStage(nextIndex);
    }

    private void Start()
    {
        SpawnStage(0);
        AudioManager.Instance.PlayBGM("TitleBGM");
        AudioManager.Instance.PlaySE("TestSE");

    }

    // WaveSpawner から通知される
    public void OnPathSelected(SkillData.SkillElement selectedSkill)
    {
        beforskill = selectedSkill.ToString();
        Debug.Log($"StageSpawner: プレイヤーが選択した道のスキルは {beforskill}");
    }

    // WaveSpawner からスキル会得通知
    public void AcquireSkill(SkillData.SkillElement acquiredSkill)
    {
        if (!acquiredSkills.Contains(acquiredSkill))
        {
            acquiredSkills.Add(acquiredSkill);
            afterskill = acquiredSkill.ToString();
            Debug.Log($"StageSpawner: 会得スキルに {afterskill} を追加");
        }
    }
}
