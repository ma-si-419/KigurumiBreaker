using UnityEngine;

public class GoalPoint : MonoBehaviour
{
    private StageSpawner _stageSpawner;
    [SerializeField] private int goalIndex;
    [SerializeField] private WaveSpawner waveSpawner; // Å© SerializeField Ç…Ç∑ÇÈ

    void Start()
    {
        _stageSpawner = FindObjectOfType<StageSpawner>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _stageSpawner != null)
        {
            waveSpawner.OnGoalReached(goalIndex);
            _stageSpawner.NextStage();
        }
    }
}
