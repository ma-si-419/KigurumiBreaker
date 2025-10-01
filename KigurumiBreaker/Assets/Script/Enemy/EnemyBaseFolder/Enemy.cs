using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemyStateData
{
    public int maxHp;             // 最大体力
    public int maxTrunk;          // 最大耐久力
    public int enemyType;         // 敵の種類（例：0=チビ、1=デカなど）
    public int attackPower;       // 攻撃力
    public float moveSpeed;       // 移動速度
    public float attackInterval;  // 攻撃間隔
    public float detectionRange;  // 検知範囲
    public float attackRange;     // 攻撃範囲
}


public class Enemy : MonoBehaviour
{
    
    /* 変数 */
    private IState _currentState;               // 現在のステート

    [Header("ステータス")]
    [SerializeField] protected EnemyStateData _currentStateData; // 現在のステータスデータ
    protected int _currentHp;    // 現在の体力
    protected int _currentTrunk; // 現在の耐久力
    protected int _currentEnemyType;    // 敵の種類

    protected float _detectRangeSqr; // プレイヤー検知範囲の二乗
    protected float _attackRangeSqr; // プレイヤー攻撃範囲の二乗

    [Header("敵が次の状態に遷移するまでの時間")]
    [SerializeField] protected float _idleWaitTime; // 待機時間
    [SerializeField] protected float _chaseWaitTime; // 追跡時間

    [Header("コンポーネント")]
    [SerializeField] protected NavMeshAgent _agent; // NavMeshAgentの参照
    [SerializeField] protected GameObject _player; // プレイヤーの参照
    [SerializeField] protected GameObject _attackHitBox; // 攻撃判定のゲームオブジェクト

    private float _stateTimer = 0.0f; // 状態遷移するまでのタイマー
    private float _rotationSpeed = 5.0f; // プレイヤーの方向を向く速度
    private bool _isDetectPlayer = false; // プレイヤーを検知したかどうかのフラグ
    protected Vector3 _direction; // 移動方向

    public NavMeshAgent agent => _agent; // NavMeshAgentのゲッター
    public GameObject player => _player; // プレイヤーのゲッター

    private void Start()
    {
        _detectRangeSqr = _currentStateData.detectionRange * _currentStateData.detectionRange;    // 検知範囲の二乗を計算して保存
        _attackRangeSqr = _currentStateData.attackRange * _currentStateData.attackRange;    // 攻撃範囲の二乗を計算して保存

        // 体力と耐久力の初期化
        _currentHp = _currentStateData.maxHp;
        _currentTrunk = _currentStateData.maxTrunk; 
        _currentEnemyType = _currentStateData.enemyType;


        // NavMeshAgentコンポーネントを取得
        _agent = GetComponent<NavMeshAgent>(); 

        //ステータスからNavMeshAgentの速度を設定
        if (_agent != null)
        {
            _agent.speed = _currentStateData.moveSpeed;  // 移動速度を設定
        }

        ChangeState(new IdleState(this));
    }

    private void Update()
    {
        Debug.Log(_currentHp);

        // 現在のステートを更新
        _currentState?.Update();
    }

    //ステートを変更する関数
    public void ChangeState(IState newState)
    {
        _currentState?.End();       // 現在のステートを抜ける
        _currentState = newState;   // 新しいステートに変更
        _currentState.Init();       // 新しいステートに入る
    }

    //基本攻撃処理(オーバーライドで変更可)
    public virtual void Attack() { }

    //基本移動処理(オーバーライドで変更可)
    public virtual void Move()
    {
        Debug.DrawLine(transform.position, player.transform.position, Color.yellow);

        Debug.Log("ChaseState: Update");

        //プレイヤーの位置を目的地に設定
        _agent.SetDestination(_player.transform.position); 

        //プレイヤーとの位置差を計算
        Vector3 diff = _player.transform.position - transform.position;

        //攻撃圏内に入ると攻撃状態へ
        //プレイヤーが検知範囲内にいるかチェック
        if (diff.sqrMagnitude < _attackRangeSqr)
        {

            //プレイヤーの方向を向き続ける
            LookAtPlayer();

            //タイマーを進める
            _stateTimer += Time.deltaTime;

            Debug.Log("攻撃範囲内に入った！");


            if (_stateTimer > _chaseWaitTime)
            {
                _agent.isStopped = true; //追跡を停止


                Debug.Log("IdleState: Change to ChaseState");

                //攻撃状態へ
                _stateTimer = 0.0f;
                ChangeState(new AttackState(this));
            }
        }
    }

    //基本待機処理(オーバーライドで変更可)
    public virtual void Idle()
    {
        Debug.DrawLine(transform.position, player.transform.position, Color.green);

        //タイマーを進める
        Debug.Log("IdleState: Update");

        Vector3 diff = _player.transform.position - transform.position; //プレイヤーとの位置差を計算

        //プレイヤーが検知範囲内にいるかチェック
        if (diff.sqrMagnitude < _detectRangeSqr || _isDetectPlayer)
        {
            //一度でも攻撃範囲内に入ったらフラグを立て続ける
            _isDetectPlayer = true;
            //プレイヤーの方向を向き続ける
            LookAtPlayer();

            Debug.Log("見つけた！");
            _stateTimer += Time.deltaTime;

            if (_stateTimer > _idleWaitTime)
            {
                Debug.Log("IdleState: Change to ChaseState");

                //追跡状態へ
                _stateTimer = 0.0f;
                ChangeState(new ChaseState(this));
            }
        }

    }

    // プレイヤー方向に向く処理
    public void LookAtPlayer()
    {
        // 向きたい方向を計算
        Vector3 direction = (_player.transform.position - transform.position).normalized;
        _direction = direction;
        // 水平方向のみ回転させる
        direction.y = 0;

        if (direction.sqrMagnitude > 0f)
        {
            // 目標の回転を取得
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // 現在の回転から目標の回転へ補完
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        }
    }

    // 攻撃判定に触れたときの処理
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            //敵が死んでたらダメージを受けない
            if (_currentState is DeadState) return; 

            // プレイヤーにダメージを与える処理
            Debug.Log("プレイヤーに攻撃した");

            // ダメージを受ける(プレイヤーアタックのダメージを取得する)
            _currentHp = other.GetComponent<PlayerAttack>().GetDamage();

            // ダメージエフェクトを生成する
            //Instantiate(_damageEffect, transform.position, Quaternion.identity);

            // Hpが0以下なら死亡処理に遷移
            if (_currentHp <= 0)
            {
                _currentHp = 0;
                ChangeState(new DeadState(this));
            }
            else
            {
                if (_currentEnemyType == 1) return; // でかい敵はダメージ状態に遷移しない

                //ダメージ状態に遷移
                ChangeState(new DamageState(this));
                Destroy(other.gameObject); // 攻撃判定を消す
            }

        }
    }

    // Getterメソッド
    public int GetMaxHp()
    {
        return _currentStateData.maxHp;
    }
    public int GetCurrentHp()
    {
        return _currentHp;
    }
    public int GetMaxTrunk()
    {
        return _currentStateData.maxTrunk;
    }
    public int GetCurrentTrunk()
    {
        return _currentTrunk;
    }
}

