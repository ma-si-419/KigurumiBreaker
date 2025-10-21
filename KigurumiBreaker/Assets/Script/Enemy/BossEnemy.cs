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
    protected bool _isAttack = false;

    // フェーズチェンジしたかどうかのフラグ
    protected bool _isPhaseChanged = false;

    // ボスの攻撃データリスト
    //[SerializeField] protected BossAttackData _bossAttackData;

    // Getter
    //public BossAttackData bossAttackData => _bossAttackData;

    [SerializeField] public float _meleeAttackRange;
    [SerializeField] public float _specialAttackRange;

    protected override void Start()
    {
        // 親クラスのStart()を呼び出す
        base.Start();

        // ボス専用の初期化処理をここに追加
        ChangeState(new BossIdleState(this));
    }

    protected override void Update()
    {
        // 親クラスのUpdate()を呼び出す
        base.Update();
    }

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
        _isAttack = false;
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

}
