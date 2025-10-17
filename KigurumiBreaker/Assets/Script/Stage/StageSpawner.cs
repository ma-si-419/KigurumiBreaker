using Unity.AI.Navigation;
using UnityEngine;

public class StageSpawner : MonoBehaviour
{
    [System.Serializable]
    public class StageSet
    {
        public GameObject[] stagePrefabs; // この大ステージの候補群
    }

    [SerializeField] private StageSet[] stageSets; // [0]=First, [1]=Second, ...
    [SerializeField] private Transform player;     // プレイヤー
    [SerializeField] private SkillSelectManager skillSelectManager; // ★ ここを追加

    private int currentStageIndex = 0;
    private GameObject currentStageInstance;

    private void SpawnStage(int index)
    {
        if (index < 0 || index >= stageSets.Length) return;

        // 既存ステージ削除
        if (currentStageInstance != null)
        {
            Destroy(currentStageInstance);
        }

        // ランダムに選択
        var prefabs = stageSets[index].stagePrefabs;
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError($"Stage {index} にプレハブが設定されていません");
            return;
        }

        int randomIndex = Random.Range(0, prefabs.Length);
        currentStageInstance = Instantiate(prefabs[randomIndex]);

        // NavMesh再構築
        var surface = currentStageInstance.GetComponent<NavMeshSurface>();
        if (surface != null)
        {
            surface.BuildNavMesh();
        }

        // プレイヤー位置移動
        Transform spawn = currentStageInstance.transform.Find("SpawnPoint");
        if (spawn != null && player != null)
        {
            player.position = spawn.position;
            player.rotation = spawn.rotation;
        }

        // GoalPoint設定
        foreach (Transform child in currentStageInstance.transform)
        {
            if (child.name.Contains("GoalPoint"))
            {
                Collider col = child.GetComponent<Collider>();
                if (col == null) col = child.gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;
            }
        }

        //  WaveSpawnerにSkillSelectManagerをアタッチ
        AttachSkillManagerToWaveSpawners(currentStageInstance);

        currentStageIndex = index;
        Debug.Log($"Stage {index + 1} を生成: {prefabs[randomIndex].name}");
    }

    /// <summary>
    /// ステージ内の全WaveSpawnerにSkillSelectManagerをセット
    /// </summary>
    private void AttachSkillManagerToWaveSpawners(GameObject stage)
    {
        if (skillSelectManager == null)
        {
            Debug.LogWarning("StageSpawnerにSkillSelectManagerが設定されていません。");
            return;
        }

        WaveSpawner[] waveSpawners = stage.GetComponentsInChildren<WaveSpawner>(true);
        foreach (var wave in waveSpawners)
        {
            wave.skillSelectManager = skillSelectManager;
            Debug.Log($"WaveSpawnerにSkillSelectManagerをアタッチ: {wave.name}");
        }
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentStageInstance != null && other.transform.IsChildOf(currentStageInstance.transform))
            {
                NextStage();
            }
        }
    }
}
