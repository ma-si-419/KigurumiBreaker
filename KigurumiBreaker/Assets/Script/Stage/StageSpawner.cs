using UnityEngine;

public class StageSpawner : MonoBehaviour
{
    [System.Serializable]
    public class StageSet
    {
        public GameObject[] stagePrefabs; // このステージの候補群
    }

    [SerializeField] private StageSet[] stageSets; // [0]=FirstStage, [1]=SecondStage, [2]=ThirdStage
    private int currentStageIndex = 0;             // いまどのステージか
    private GameObject currentStageInstance;       // いま生成中のステージ

    /// <summary>
    /// ステージをランダム生成
    /// </summary>
    public void SpawnNextStage()
    {
        if (currentStageIndex >= stageSets.Length)
        {
            Debug.Log("全ステージクリア！");
            return;
        }

        // 既存ステージを消す
        if (currentStageInstance != null)
        {
            Destroy(currentStageInstance);
        }

        // 候補からランダムで1つ選ぶ
        var prefabs = stageSets[currentStageIndex].stagePrefabs;
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError("Stage " + currentStageIndex + " にプレハブが設定されていません");
            return;
        }

        int randomIndex = Random.Range(0, prefabs.Length);
        currentStageInstance = Instantiate(prefabs[randomIndex], Vector3.zero, Quaternion.identity);

        Debug.Log($"Stage {currentStageIndex + 1} を生成: {prefabs[randomIndex].name}");

        currentStageIndex++;
    }

    // 仮で Start 時に 1 ステージ目を生成
    private void Start()
    {
        SpawnNextStage();
    }

    private void Update()
    {
        // 仮でにスペースキーで次のステージを生成
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnNextStage();
        }
    }
}
