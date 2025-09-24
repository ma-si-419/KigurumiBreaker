using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    
    /* 変数 */
    private IState _currentState;               // 現在のステート
    [SerializeField] public NavMeshAgent agent; // NavMeshAgentの参照
    [SerializeField] public GameObject player;  // プレイヤーの参照
    public Transform playerTrans;               // プレイヤーのTransform
    public float detectRange = 20f;             // プレイヤー検知範囲
    public float detectRangeSqr;                // プレイヤー検知範囲の二乗
    public float attackRange = 4f;              // 攻撃範囲
    public float attackRangeSqr;                // プレイヤー検知範囲の二乗
    private float timer = 0.0f;                 // タイマー

    private void Start()
    {
        detectRangeSqr = detectRange * detectRange;    // 検知範囲の二乗を計算して保存
        attackRangeSqr = attackRange * attackRange;    // 攻撃範囲の二乗を計算して保存
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

    public virtual void Attack()
    {
        timer += Time.deltaTime;

        if (timer > 2.0f)
        {
            // 1秒に1回しか攻撃しない
            timer = 0.0f;
            ChangeState(new IdleState(this));
        }

        // プレイヤーを攻撃する処理
        Debug.Log("Enemy: Attack");
    }

}

