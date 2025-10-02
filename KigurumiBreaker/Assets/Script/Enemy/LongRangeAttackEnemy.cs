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

    public override void Move()
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
            Debug.Log("IdleState: Change to ChaseState");

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

        if (_shootCount >= 3)
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
        Debug.Log("弾を発射!!");
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
        if(NavMesh.SamplePosition(fleePos, out hit, 20.0f, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }

    }

}
