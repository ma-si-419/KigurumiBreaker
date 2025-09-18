using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    
    /* 変数 */
    private IState _currentState;               // 現在のステート
    [SerializeField] public NavMeshAgent agent; // NavMeshAgentの参照
    [SerializeField] public GameObject player;  // プレイヤーの参照
    public string targetTag = "Player";         // プレイヤーのタグ
    public Transform target;         // プレイヤーのTransform


    private void Start()
    {
        ChangeState(new IdleState(this));
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
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

