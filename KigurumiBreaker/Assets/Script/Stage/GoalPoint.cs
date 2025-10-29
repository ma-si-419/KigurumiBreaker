using UnityEngine;

public class GoalPoint : MonoBehaviour
{
    private StageSpawner _stageSpawner;
    [SerializeField]private int goalIndex;
    private WaveSpawner waveSpawner;

    void Start()
    {
        _stageSpawner = FindObjectOfType<StageSpawner>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _stageSpawner != null)
        {
            _stageSpawner.NextStage();
            waveSpawner.OnGoalReached(goalIndex);
        }
    }
}
