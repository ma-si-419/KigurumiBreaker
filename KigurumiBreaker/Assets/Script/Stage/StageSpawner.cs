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
    [SerializeField] private Transform player;     // プレイヤーを指定

    private int currentStageIndex = 0;             // 今どの大ステージか
    private GameObject currentStageInstance;       // 生成中のステージ

    /// <summary>
    /// 指定インデックスのステージセットからランダムに1つ生成
    /// </summary>
    private void SpawnStage(int index)
    {
        if (index < 0 || index >= stageSets.Length) return;

        // 既存ステージ削除
        if (currentStageInstance != null)
        {
            Destroy(currentStageInstance);
        }

        // 候補からランダムに選択
        var prefabs = stageSets[index].stagePrefabs;
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError($"Stage {index} にプレハブが設定されていません");
            return;
        }

        int randomIndex = Random.Range(0, prefabs.Length);
        currentStageInstance = Instantiate(prefabs[randomIndex]);

        var surface = currentStageInstance.GetComponent<NavMeshSurface>();
        if (surface != null)
        {
            surface.BuildNavMesh();
        }


        // SpawnPoint を探してプレイヤーを移動
        Transform spawn = currentStageInstance.transform.Find("SpawnPoint");
        if (spawn != null && player != null)
        {
            player.position = spawn.position;
            player.rotation = spawn.rotation;
        }

        // GoalPoint を探して Collider を設定
        foreach (Transform child in currentStageInstance.transform)
        {
            if (child.name.Contains("GoalPoint"))
            {
                Collider col = child.GetComponent<Collider>();
                if (col == null) col = child.gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;
            }
        }

        currentStageIndex = index;
        Debug.Log($"Stage {index + 1} を生成: {prefabs[randomIndex].name}");
    }

    /// <summary>
    /// 次の大ステージへ進む
    /// </summary>
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
        // 起動時に First ステージを生成
        SpawnStage(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // その GoalPoint が今のステージの子かチェック
            if (currentStageInstance != null && other.transform.IsChildOf(currentStageInstance.transform))
            {
                NextStage();
            }
        }
    }
}
