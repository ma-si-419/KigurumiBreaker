using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    
    /* 変数 */
    private IState _currentState;               // 現在のステート
    [SerializeField] public NavMeshAgent agent; // NavMeshAgentの参照
    [SerializeField] public GameObject player;  // プレイヤーの参照
    public string targetTag = "Player";         // プレイヤーのタグ
    public Transform playerTrans { get; private set; }                    // プレイヤーのTransform


    private void Start()
    {
        playerTrans = GameObject.FindGameObjectWithTag(targetTag).transform;
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeState(new ChaseState(this));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ChangeState(new IdleState(this));
        }
    }

}

