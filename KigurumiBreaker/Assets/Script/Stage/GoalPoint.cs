using UnityEngine;

public class GoalPoint : MonoBehaviour
{
    private StageSpawner spawner;
    public int goalIndex;
    public WaveSpawner waveSpawner;
    void Start()
    {
        spawner = FindObjectOfType<StageSpawner>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && spawner != null)
        {
            spawner.NextStage();
            waveSpawner.OnGoalReached(goalIndex);
        }
    }
}
