using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    // 敵のステータスデータ
    [Header("敵のステータスデータ")]
    [SerializeField] protected EnemyData _enemyData;

    // 敵の定数データ
    [Header("敵の定数データ")]
    [SerializeField] protected EnemyCommonData _enemyCommonData;

    // 攻撃オブジェクトのプレハブ
    protected GameObject _attackObjectPrefab;

    // 現在のHP
    protected float _currentHp;

    // 現在の耐久力
    protected float _currentTrunk;

    // 現在のステート状態
    protected IState _currentState;

    //索敵範囲の二乗
    protected float _detectRangeSqr;

    //攻撃範囲の二乗
    protected float _attackRangeSqr;

    // NavMeshAgentの参照
    protected NavMeshAgent _agent;

    // プレイヤーの参照
    protected GameObject _player;

    // アニメーターの参照
    protected Animator _animator;

    // Rigidbodyの参照
    protected Rigidbody _rigidbody;

    //バトルマネージャーの参照
    protected BattleManager _battleManager;

    // 死んだかどうかのフラグ
    protected bool _isDead = false;

    // 向きたい方向
    protected Vector3 _direction;

    protected bool _isStop = false;

    protected bool _isDamage = false;

    protected Vector3 _shakeVec;

    protected Vector3 _stopPos;

    protected float _animationSpeed = 1.0f;

    // アーマーかどうかのフラグ
    protected bool _isArmor;

    // デバッグ用停止フラグ
    protected bool _isDebugStop;

    //敵のデバフ状態
    public enum EnemyDebuff
    {
        AtkDown,
        DefDown,
        SpeedDown,
        Poison,
        None
    }

    // NavMeshAgentのゲッター
    public NavMeshAgent agent => _agent;
    // Playerのゲッター
    public GameObject player => _player;
    // Animatorのゲッター
    public Animator animator => _animator;
    // 敵のステータスデータのゲッター
    public EnemyData enemyData => _enemyData;
    public EnemyCommonData enemyConstantData => _enemyCommonData;
    public BattleManager battleManager => _battleManager;

    protected virtual void Start()
    {
        //索敵範囲と攻撃範囲の二乗を計算して保存
        _detectRangeSqr = _enemyData.detectionRange * _enemyData.detectionRange;
        _attackRangeSqr = _enemyData.attackRange * _enemyData.attackRange;
        // 体力と耐久力の初期化
        _currentHp = _enemyData.maxHp;
        _currentTrunk = _enemyData.maxTrunk;
        // 攻撃オブジェクトのプレハブを設定
        _attackObjectPrefab = _enemyData.attackPrefab;
        // 敵データでアーマーかどうかを設定
        _isArmor = _enemyData.isArmor;
        // NavMeshAgentコンポーネントを取得
        _agent = GetComponent<NavMeshAgent>();
        // Animatorコンポーネントを取得
        _animator = GetComponent<Animator>();
        // Rigidbodyコンポーネントを取得
        _rigidbody = GetComponent<Rigidbody>();

        //ステータスからNavMeshAgentの速度を設定
        if (_agent != null)
        {
            _agent.speed = _enemyData.moveSpeed;  // 移動速度を設定
            _agent.stoppingDistance = _enemyData.attackRange; // 攻撃範囲を設定
        }

        // プレイヤー参照が空なら自動で探す
        if (_player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj;
            }
            else
            {
                Debug.LogWarning($"{name}: Playerがシーンに見つかりませんでした！");
            }
        }
    }

    protected virtual void Update()
    {
        // 現在のステートのUpdateメソッドを呼び出す
        _currentState?.Update();
    }

    // ステートを変更するメソッド
    public void ChangeState(IState newState)
    {
        // 現在のステートの終了処理を呼び出す
        _currentState?.End();
        // 新しいステートに変更
        _currentState = newState;
        // 新しいステートの開始処理を呼び出す
        _currentState?.Init();
    }

    // 敵がプレイヤーを向く方向を計算して回転
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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _enemyData.rotateSpeed * Time.deltaTime);

        }
    }

    //オブジェクトの移動力をゼロにする
    public void StopMovement()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    // Getterメソッド
    public float GetMaxHp()
    {
        return _enemyData.maxHp;
    }
    public float GetCurrentHp()
    {
        return _currentHp;
    }
    public float GetMaxTrunk()
    {
        return _enemyData.maxTrunk;
    }
    public float GetCurrentTrunk()
    {
        return _currentTrunk;
    }

    //バトルマネージャーの参照をセットするメソッド
    public void SetBattleManager(BattleManager battleManager)
    {
        _battleManager = battleManager;
    }

    public void SetStop(bool flag)
    {
        _isStop = flag;
    }

    public void StopAnimation()
    {
        if (_animator.speed > 0)
        {
            _animationSpeed = _animator.speed;
        }
        _animator.speed = 0;
    }

    public void StartAnimation()
    {
        _animator.speed = _animationSpeed;
    }

    // Gizmosを使って検知範囲と攻撃範囲を表示
    private void OnDrawGizmosSelected()
    {
        // 検知範囲（シアン色のワイヤーフレーム球）
        Gizmos.color = Color.yellow;
        float detectRadius = _enemyData != null ? _enemyData.detectionRange : 0f;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        // 攻撃範囲（赤色のワイヤーフレーム球、必要なら）
        Gizmos.color = Color.red;
        float attackRadius = _enemyData != null ? _enemyData.attackRange : 0f;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
