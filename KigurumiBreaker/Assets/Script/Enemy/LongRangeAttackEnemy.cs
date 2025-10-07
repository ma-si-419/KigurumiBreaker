using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LongRangeAttackEnemy : Enemy
{
    // 攻撃に関する変数
    private float _longRangeTimer = 0.0f;   // タイマー
    [SerializeField] public float shootInterval = 0.5f; // 射撃間隔

    [SerializeField] private float _fleeDistance; // 逃げる距離
    [SerializeField] private float _shootCount;  // 弾を撃つ回数

    [SerializeField] private float _maxChaseRange; // 追跡の上限距離
    private float _maxChaseRangeSqr; // 追跡の上限距離の二乗

    /* 定数 */
    private const int MAX_SHOOT_COUNT = 3; // 弾を撃つ最大回数
    private const float MIN_FLEE_DISTANCE = 20.0f; // 逃げる距離の最小値

    public override void Chase()
    {
        //逃げる処理
        Nav();

        _maxChaseRangeSqr = _maxChaseRange * _maxChaseRange;

        this.GetComponent<Renderer>().material.color = Color.yellow;
        Debug.DrawLine(transform.position, player.transform.position, Color.yellow);

        //タイマーを進める
        _longRangeTimer += Time.deltaTime;


        if (_longRangeTimer > _chaseWaitTime)
        {
            _agent.isStopped = true; //追跡を停止

            //攻撃状態へ
            _longRangeTimer = 0.0f;
            ChangeState(new AttackState(this));
        }

        //プレイヤーとの上限距離を計算
        Vector3 diff = _player.transform.position - transform.position;

        if (diff.sqrMagnitude > _maxChaseRangeSqr)
        {
            _agent.isStopped = true; //追跡を停止
            //視界外に出たのでIdle状態へ
            ChangeState(new AttackState(this));
        }


    }

    public override void Attack()
    {
        _longRangeTimer += Time.deltaTime;

        //プレイヤーの方向を向き続ける
        LookAtPlayer();

        base.Attack();


        // 一定時間ごとに弾を発射(3発連射するみたいな間隔にしたい)
        if (_longRangeTimer > shootInterval)
        {
            Shoot();

            // タイマーをリセット
            _longRangeTimer = 0.0f;
            _shootCount += 1;

        }

        if (_shootCount >= MAX_SHOOT_COUNT)
        {
            _shootCount = 0;
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
        Vector3 fleePos = transform.position + fleeDir * _fleeDistance;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(fleePos, out hit, MIN_FLEE_DISTANCE, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }

    }

}
