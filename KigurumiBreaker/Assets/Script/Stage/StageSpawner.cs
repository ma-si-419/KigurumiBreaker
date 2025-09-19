using UnityEngine;

public class StageSpawner : MonoBehaviour
{
    [System.Serializable]
    public class StageSet
    {
        public GameObject[] stagePrefabs; // ���̃X�e�[�W�̌��Q
    }

    [SerializeField] private StageSet[] stageSets; // [0]=FirstStage, [1]=SecondStage, [2]=ThirdStage
    private int currentStageIndex = 0;             // ���܂ǂ̃X�e�[�W��
    private GameObject currentStageInstance;       // ���ܐ������̃X�e�[�W

    /// <summary>
    /// �X�e�[�W�������_������
    /// </summary>
    public void SpawnNextStage()
    {
        if (currentStageIndex >= stageSets.Length)
        {
            Debug.Log("�S�X�e�[�W�N���A�I");
            return;
        }

        // �����X�e�[�W������
        if (currentStageInstance != null)
        {
            Destroy(currentStageInstance);
        }

        // ��₩�烉���_����1�I��
        var prefabs = stageSets[currentStageIndex].stagePrefabs;
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError("Stage " + currentStageIndex + " �Ƀv���n�u���ݒ肳��Ă��܂���");
            return;
        }

        int randomIndex = Random.Range(0, prefabs.Length);
        currentStageInstance = Instantiate(prefabs[randomIndex], Vector3.zero, Quaternion.identity);

        Debug.Log($"Stage {currentStageIndex + 1} �𐶐�: {prefabs[randomIndex].name}");

        currentStageIndex++;
    }

    // ���� Start ���� 1 �X�e�[�W�ڂ𐶐�
    private void Start()
    {
        SpawnNextStage();
    }

    private void Update()
    {
        // ���łɃX�y�[�X�L�[�Ŏ��̃X�e�[�W�𐶐�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnNextStage();
        }
    }
}
