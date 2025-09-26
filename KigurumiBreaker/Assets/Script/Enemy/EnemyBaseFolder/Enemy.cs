using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    
    /* 変数 */
    private IState _currentState;               // 現在のステート

    [Header("索敵距離")]
    public float detectRange;             // プレイヤー検知範囲
    public float detectRangeSqr;                // プレイヤー検知範囲の二乗
    [Header("攻撃距離")]
    public float attackRange;              // 攻撃範囲
    public float attackRangeSqr;                // プレイヤー検知範囲の二乗

    [Header("時間設定")]
    public float idleTime;         // 待機時間
    public float chaseTime;        // 追跡時間

    [Header("コンポーネント")]
    [SerializeField] public NavMeshAgent agent; // NavMeshAgentの参照
    [SerializeField] public GameObject player;  // プレイヤーの参照
    [SerializeField] public GameObject attackHitBox; // 攻撃判定のゲームオブジェクト
    public Transform playerTrans;               // プレイヤーのTransform

    private float _stateTimer = 0.0f;                 // 状態遷移するまでのタイマー

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
        _currentState?.End();       // 現在のステートを抜ける
        _currentState = newState;   // 新しいステートに変更
        _currentState.Init();       // 新しいステートに入る
    }

    //基本攻撃処理(オーバーライドで変更可)
    public virtual void Attack()
    {
        _stateTimer += Time.deltaTime;

        if (_stateTimer > 2.0f)
        {
            // 1秒に1回しか攻撃しない
            _stateTimer = 0.0f;
            // 攻撃後、待機状態へ戻る
            ChangeState(new IdleState(this));
        }

        // プレイヤーを攻撃する処理
        Debug.Log("Enemy: Attack");


        //プレイヤーを攻撃したら待機状態へ
        if (_stateTimer > 3.0f)
        {
            //状態を変更する
            Debug.Log("AttackState: Change to IdleState");
            _stateTimer = 0.0f;
            ChangeState(new IdleState(this));
        }

    }

    //基本移動処理(オーバーライドで変更可)
    public virtual void Move()
    {
        //タイマーを進める
        _stateTimer += Time.deltaTime;
        Debug.Log("ChaseState: Update");

        agent.SetDestination(player.transform.position); //プレイヤーの位置を目的地に設定

        Vector3 diff = playerTrans.position - transform.position; //プレイヤーとの位置差を計算

        //攻撃圏内に入ると攻撃状態へ
        //プレイヤーが検知範囲内にいるかチェック
        if (diff.sqrMagnitude < attackRangeSqr/* && _timer > chaseTime*/)
        {
            Debug.Log("IdleState: Change to ChaseState");
            agent.isStopped = true; //追跡を停止

            //攻撃状態へ
            _stateTimer = 0.0f;
            ChangeState(new AttackState(this));
        }
    }

    //基本待機処理(オーバーライドで変更可)
    public virtual void Idle()
    {
        //タイマーを進める
        _stateTimer += Time.deltaTime;
        Debug.Log("IdleState: Update");

        Vector3 diff = playerTrans.position - transform.position; //プレイヤーとの位置差を計算

        //プレイヤーが検知範囲内にいるかチェック
        if (diff.sqrMagnitude < detectRangeSqr && _stateTimer > idleTime)
        {
            Debug.Log("IdleState: Change to ChaseState");

            //追跡状態へ
            _stateTimer = 0.0f;
            ChangeState(new ChaseState(this));
        }

    }

}

