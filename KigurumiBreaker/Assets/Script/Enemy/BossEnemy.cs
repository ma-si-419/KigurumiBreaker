using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEngine.Rendering.VirtualTexturing;

public class BossEnemy : EnemyBase
{
    // 状態遷移するまでのタイマー
    protected float _stateTimer = 0.0f;

    // 攻撃するまでのタイマー
    protected float _attackTimer = 0.0f;

    // プレイヤーが攻撃範囲に入ったら攻撃状態に遷移させるためのフラグ
    public bool isAttack = false;

    // フェーズチェンジしたかどうかのフラグ
    protected bool _isPhaseChanged = false;

    // 攻撃範囲の値の二乗
    protected float _meleeAttackRangeSqr;
    protected float _specialAttackRangeSqr;

    [SerializeField] public float _meleeAttackRange;
    [SerializeField] public float _specialAttackRange;

    [Header("通常攻撃のプレハブ(仮)")]
    [SerializeField] protected GameObject _meleeAttackPrefab; 

    // ボスの攻撃データリスト
    //[SerializeField] protected BossAttackData _bossAttackData;

    // Getter
    //public BossAttackData bossAttackData => _bossAttackData;

    public float meleeAttackRangeSqr => _meleeAttackRangeSqr;
    public float specialAttackRangeSqr => _specialAttackRangeSqr;

    public GameObject meleeAttackPrefab => _meleeAttackPrefab;

    protected override void Start()
    {
        // 親クラスのStart()を呼び出す
        base.Start();

        _meleeAttackRangeSqr = _meleeAttackRange * _meleeAttackRange;
        _specialAttackRangeSqr = _specialAttackRange * _specialAttackRange;

        // ボスのNavMeshの当たり判定の半径を設定（必要に応じて調整）
        agent.radius = 1.0f; 

        // ボス専用の初期化処理をここに追加
        ChangeState(new BossIdleState(this));
    }

    protected override void Update()
    {

        float a = 0.05f;

        if (_isStop)
        {
            if (_isDamage)
            {
                _shakeVec.x = Random.Range(-a, a);
                _shakeVec.z = Random.Range(-a, a);
            }

            this.transform.position += _shakeVec;
        }
        else
        {
            if (_shakeVec.sqrMagnitude >= 0.001f)
            {
                this.transform.position = _stopPos;
            }

            _shakeVec = Vector3.zero;
        }

        if (_isStop) return;

        // 親クラスのUpdate()を呼び出す
        base.Update();



        DebugLine();
    }

    // ボス専用の近接攻撃処理(オーバライド)
    public virtual void MeleeAttack(){}

    // ボス専用の特殊攻撃処理(オーバライド)
    public virtual void Attack(){}

    public virtual void Stan()
    {
        // ボス専用のスタンする処理をここに追加
        // 一定時間動けなくするなど
    }

    public virtual void PhaseChange()
    {
        // ボス専用のフェーズを変える処理をここに追加
        // 攻撃パターンの変更するフラグを立てるなど

    }

    // モデルのリグを取得して攻撃判定を特定のボーンにアタッチする処理


    public void AttackReset()
    {
        isAttack = false;
        _attackTimer = 0.0f;
    }

    // 攻撃判定に触れたときの処理
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            //死んだ状態になっている場合はダメージを受けない
            if (_currentState is BossDeadState) return;

            // ダメージを受ける(プレイヤーアタックのダメージを取得する)
            _currentHp -= other.GetComponent<PlayerAttack>().GetDamage();

            //ヒットストップ処理
            PlayerAttack playerAttack = other.GetComponent<PlayerAttack>();
            BattleManager manager = _battleManager.GetComponent<BattleManager>();
            manager.StopTime(playerAttack.GetHitStopTime());


            // 耐久力を減らす(プレイヤーアタックの耐久力ダメージを取得する)
            //_currentTrunk -= other.GetComponent<PlayerAttack>().GetTrunkDamage();

            // ダメージエフェクトを生成する
            //Instantiate(_damageEffect, transform.position, Quaternion.identity);

            // Hpが0以下なら死亡処理に遷移
            if (_currentHp <= 0)
            {
                _currentHp = 0;
                // すでに死亡状態なら変更しない
                if (!(_currentState is BossDeadState))
                {
                    ChangeState(new BossDeadState(this));
                }
            }

            //攻撃はいったら攻撃判定を速攻消す
            Destroy(other.gameObject);

            //攻撃状態のときはダメージアニメーションを行わない
            if (_currentState is BossAttackState) return;
            if (_currentState is BossMeleeAttackState) return;

            //OnHit();
        }

        if (other.gameObject.CompareTag("PlayerRangedAttack"))
        {
            //死んだ状態になっている場合はダメージを受けない
            if (_currentState is BossDeadState) return;

            // プレイヤーにダメージを与える処理
            _currentHp -= other.GetComponent<PlayerAttack>().GetDamage();

            //ヒットストップ処理
            PlayerAttack playerAttack = other.gameObject.GetComponent<PlayerAttack>();
            BattleManager manager = _battleManager.GetComponent<BattleManager>();
            manager.StopTime(playerAttack.GetHitStopTime());

            // Hpが0以下なら死亡処理に遷移
            if (_currentHp <= 0)
            {
                _currentHp = 0;
                // すでに死亡状態なら変更しない
                if (!(_currentState is BossDeadState))
                {
                    ChangeState(new BossDeadState(this));
                }
            }

            //攻撃はいったら攻撃判定を速攻消す
            Destroy(other.gameObject);

            //攻撃状態のときはダメージアニメーションを行わない
            if (_currentState is BossAttackState) return;
            if (_currentState is BossMeleeAttackState) return;

            //OnHit();
        }

    }

    //デバッグ用に線を引く
    public void DebugLine()
    {
        //プレイヤーとの位置差を表示
        Debug.DrawLine(transform.position, player.transform.position, Color.green);

        //敵の攻撃範囲を表示
        Debug.DrawLine(transform.position, transform.position + transform.forward * Mathf.Sqrt(specialAttackRangeSqr), Color.red);
        
        //敵の検知範囲を球で表示
        Debug.DrawLine(transform.position, transform.position + transform.forward * Mathf.Sqrt(meleeAttackRangeSqr), Color.blue);

    }

}
