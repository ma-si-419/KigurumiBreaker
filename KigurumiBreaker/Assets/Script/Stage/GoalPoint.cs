using UnityEngine;
using UnityEngine.ProBuilder;

public class GoalPoint : MonoBehaviour
{
    private StageSpawner _stageSpawner;
    [SerializeField] private int goalIndex;
    [SerializeField] private WaveSpawner waveSpawner; // Å© SerializeField Ç…Ç∑ÇÈ
    private bool _isGoal;

    void Start()
    {
        _isGoal = false;

        _stageSpawner = FindObjectOfType<StageSpawner>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isGoal) return;

        if (other.CompareTag("Player") && _stageSpawner != null)
        {
            waveSpawner.OnGoalReached(goalIndex);
            _stageSpawner.NextStage();
            _isGoal = true; // ÉSÅ[ÉãÇ…ìûíBÇµÇΩÇ±Ç∆ÇãLò^
        }
    }
}
