using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // 現在のステート
    private IState _currentState;

    [SerializeField] public NavMeshAgent _agent; //NavMeshAgentの参照
    [SerializeField] public GameObject _player;  //プレイヤーの参照


    private void Start()
    {
        ChangeState(new IdleState(this));
    }

    private void Update()
    {
        // 現在のステートを更新
        _currentState?.Update();
    }

    public void ChangeState(IState newState)
    {
        _currentState?.End();   // 現在のステートを抜ける
        _currentState = newState;
        _currentState.Init();   // 新しいステートに入る
    }

}
