using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LongRangeAttackEnemy : Enemy
{

    // 攻撃に関する変数
    private float _longRangeTimer = 0.0f;   // タイマー

    [SerializeField] private float _shootCount;  // 弾を撃つ回数
    [SerializeField] private float _maxChaseRange; // 追跡の上限距離

    /* 定数 */
    [Header("定数")]
    [SerializeField] private float MIN_FLEE_DISTANCE = 20.0f; // 逃げる距離の最小値
    [SerializeField] private float FLEE_DISTANCE = 0.0f; // 逃げる距離

    public override void Idle()
    {
        //_agent.isStopped = true; // 追跡を停止

        //プレイヤーの位置を目的地に設定
        _agent.SetDestination(_player.transform.position);

        // 移動を停止
        StopMovement();

        Vector3 diff = _player.transform.position - transform.position; //プレイヤーとの位置差を計算

        //プレイヤーが検知範囲内にいるかチェック
        if (diff.sqrMagnitude < _detectRangeSqr || _isSearched)
        {
            //プレイヤー発見したら一度だけ呼ばれる
            if (!_isSearched)
            {
                //敵の頭上にビックリマークを出す

            }

            //一度でも攻撃範囲内に入ったらフラグを立て続ける
            _isSearched = true;

            _stateTimer += Time.deltaTime;

            if (_stateTimer > IDLE_WAIT_TIME)
            {
                //追跡状態へ
                _stateTimer = 0.0f;
                ChangeState(new ChaseState(this));
            }
        }
    }

    public override void Chase()
    {

        _longRangeTimer += Time.deltaTime;
        
        //逃げる処理
        Nav();

        //プレイヤーとの位置差を計算
        Vector3 diff = _player.transform.position - transform.position;
        float attackRange = _maxChaseRange * _maxChaseRange;

        Debug.DrawLine(transform.position, transform.position + transform.forward * Mathf.Sqrt(_maxChaseRange), Color.black);

        if (_longRangeTimer > 2.0f)
        {
            _agent.isStopped = true; //追跡を停止
            LookAtPlayer();
        }

        if (diff.sqrMagnitude > attackRange)
        {
            _agent.isStopped = true; //追跡を停止
            LookAtPlayer();
        }

        if (_longRangeTimer > CHASE_WAIT_TIME)
        {
            //攻撃状態へ
            _longRangeTimer = 0.0f;
            ChangeState(new AttackState(this));
        }
        //else if (diff.sqrMagnitude > attackRange)
        //{
        //    //プレイヤーが追跡範囲外に出たら
        //    LookAtPlayer();
        //    _longRangeTimer = 0.0f;
        //    ChangeState(new AttackState(this));
        //}
    }

    public override void Attack()
    {
        //プレイヤーの方向を向き続ける
        LookAtPlayer();

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 0.5f)
        {
            //攻撃判定を一つ生成させる
            if (!_isCreateAttack)
            {
                _isCreateAttack = true;
                Shoot();
            }

            _isStateChange = true;
            //攻撃フラグをリセット
            _isCreateAttack = false;
        }

        if (_isStateChange)
        {
            _isStateChange = false;
            ChangeState(new IdleState(this));
        }

    }

    //弾の生成
    private void Shoot()
    {
        //弾を生成
        GameObject bullet = Instantiate(_attackObjectPrefab, this.transform.position, this.transform.rotation);
    }

    //ナビメッシュエージェントでターゲットから逃げる処理
    private void Nav()
    {
        //ターゲットから敵への方向
        Vector3 dirTarget = transform.position - _player.transform.position;

        //正規化して逃げる方向に
        Vector3 fleeDir = dirTarget.normalized;

        //一定距離先を目標に設定
        Vector3 fleePos = transform.position + fleeDir * FLEE_DISTANCE;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(fleePos, out hit, MIN_FLEE_DISTANCE, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }

    }

}
