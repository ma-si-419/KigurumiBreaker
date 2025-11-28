using UnityEngine;
using UnityEngine.ProBuilder;

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
            waveSpawner.FadeOut(10f);
            waveSpawner.FadeIn(100f);
            _stageSpawner.NextStage();
        }
    }
}
