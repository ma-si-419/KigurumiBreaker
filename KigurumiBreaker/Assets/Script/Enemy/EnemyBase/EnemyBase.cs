using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    //敵のステータスデータ
   [SerializeField] protected EnemyData _enemyData;

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

    // 攻撃オブジェクトのプレハブ
    [Header("コンポーネント")]
    [SerializeField] protected GameObject _attackObjectPrefab; 

    // NavMeshAgentの参照
    protected NavMeshAgent _agent;

    // プレイヤーの参照
    protected GameObject _player;

    // アニメーターの参照
    protected Animator _animator;

    // Rigidbodyの参照
    protected Rigidbody _rigidbody;

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

    protected virtual void Start()
    {
        //索敵範囲と攻撃範囲の二乗を計算して保存
        _detectRangeSqr = _enemyData.detectionRange * _enemyData.detectionRange;   
        _attackRangeSqr = _enemyData.attackRange * _enemyData.attackRange;    

        // 体力と耐久力の初期化
        _currentHp = _enemyData.maxHp;
        _currentTrunk = _enemyData.maxTrunk;

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

        //待機状態に設定
        //ここは継承先で設定
        //ChangeState(new IdleState(this));
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
}
