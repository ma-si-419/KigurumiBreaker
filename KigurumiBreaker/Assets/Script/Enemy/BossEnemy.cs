using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public int _attackCount = 0; // 攻撃回数カウント用


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

        // ボス専用の更新処理をここに追加

        //ヒット数が一定数を超えたらスタン状態へ
        //if (_hitCount >= _enemyData.stanCount)
        //{
        //    _hitCount = 0; // ヒット数をリセット
        //    ChangeState(new BossStanState(this));
        //}

        //HPが一定数を下回ったらフェーズチェンジ状態へ
        //if (_currentHp <= _enemyData.maxHp / 2 && !_isPhaseChanged)
        //{
                //ChangeState(new BossPhaseChangeState(this));
        //}

    }

    public virtual void Idle()
    {
        // ボス専用の待機処理をここに追加
        _agent.isStopped = true; // 追跡を停止
        //プレイヤーの位置を目的地に設定
        _agent.SetDestination(_player.transform.position);

        Debug.Log("待機");

        // 移動を停止
        StopMovement();

        Vector3 diff = _player.transform.position - transform.position; //プレイヤーとの位置差を計算

        //タイマーで追跡状態へ移行
        _stateTimer += Time.deltaTime;

        if (_stateTimer > _enemyData.idleToChaseTime)
        {
            //追跡状態へ
            _stateTimer = 0.0f;
            ChangeState(new BossChaseState(this));
        }

        ////プレイヤーが検知範囲内にいるかチェック
        //if (diff.sqrMagnitude < _detectRangeSqr)
        //{
        //    _stateTimer += Time.deltaTime;

        //    if (_stateTimer > _enemyData.idleToChaseTime)
        //    {
        //        //追跡状態へ
        //        _stateTimer = 0.0f;
        //        ChangeState(new BossChaseState(this));
        //    }
        //}

        //// 攻撃範囲に入ったら攻撃状態に遷移させる？
        //if (diff.sqrMagnitude < _attackRangeSqr)
        //{
        //    _attackTimer += Time.deltaTime;

        //    LookAtPlayer(); // プレイヤーの方向を向く


        //    if (_attackTimer > _enemyData.chaseToAttack)
        //    {
        //        //追跡状態へ
        //        _stateTimer = 0.0f;
        //        ChangeState(new BossAttackState(this));
        //    }
        //}
    }

    public virtual void Chase()
    {
        // ボス専用の追跡処理をここに追加(正味ここはザコ敵と同じ処理)
        Debug.Log("追跡");

        //プレイヤーの位置を目的地に設定
        _agent.SetDestination(_player.transform.position);
        // Rigidbodyの移動を停止(プレイヤーと衝突した際に吹っ飛ばされないため)
        StopMovement(); 

        //プレイヤーとの位置差を計算
        Vector3 diff = _player.transform.position - transform.position;

        //攻撃圏内に入ると攻撃状態へ
        //プレイヤーが検知範囲内にいるかチェック
        if (diff.sqrMagnitude < _attackRangeSqr)
        {
            //プレイヤーの方向を向き続ける
            LookAtPlayer();

            //追跡を停止
            _agent.isStopped = true;

            //攻撃状態へ
            _stateTimer = 0.0f;
            ChangeState(new BossAttackState(this));

            //タイマーを進める(スピード感を出すため一旦除外)
            _stateTimer += Time.deltaTime;
            if (_stateTimer > _enemyData.chaseToAttack)
            {
            }
        }

    }

    // 通常攻撃(ボスによって変えたい場合はオーバライド)
    public virtual void MeleeAttack()
    {
        //animator.SetTrigger("MeleeAttack");

        // 移動を停止
        StopMovement();

        Debug.Log("通常攻撃");
        // ボス専用の近接攻撃処理をここに追加

        ChangeState(new BossIdleState(this));
    }

    // 範囲攻撃(ボスによって変えたい場合はオーバライド)
    public virtual void RangeAttack()
    {
        //animator.SetTrigger("RangeAttack");

        // 移動を停止
        StopMovement();

        Debug.Log("範囲攻撃");
        // ボス専用の遠距離攻撃処理をここに追加

        ChangeState(new BossIdleState(this));
    }

    // 長距離攻撃(ボスによって変えたい場合はオーバライド)
    public virtual void LongRangeAttack()
    {
        //animator.SetTrigger("LongRangeAttack");

        // 移動を停止
        StopMovement();

        Debug.Log("長距離攻撃");
        // ボス専用の長距離攻撃処理をここに追加

        _attackCount = 0; // 攻撃回数リセット

        ChangeState(new BossIdleState(this));
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

    public void AttackReset()
    {
        _isAttack = false;
        _attackTimer = 0.0f;
    }
}
