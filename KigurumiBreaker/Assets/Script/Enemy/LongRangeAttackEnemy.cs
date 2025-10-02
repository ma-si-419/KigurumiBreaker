using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LongRangeAttackEnemy : Enemy
{
    // 攻撃に関する変数
    private float _longRangeTimer = 0.0f;   // タイマー
    public Transform firePoint;
    public float shootInterval = 0.5f; // 射撃間隔

    [SerializeField] private float fleeDistance; // 逃げる距離

    public override void Attack()
    {
        _longRangeTimer += Time.deltaTime;

        LookAtPlayer();

        base.Attack();


        // 一定時間ごとに弾を発射(3発連射するみたいな間隔にしたい)
        if (_longRangeTimer > shootInterval)
        {
            Shoot();

            // タイマーをリセット
            _longRangeTimer = 0.0f;
        }

    }

    public override void Move()
    {
        //逃げる処理
        Nav();

    }

    //弾の生成
    private void Shoot()
    {
        //弾を生成
        GameObject bullet = Instantiate(_attackObjectPrefab, firePoint.position, firePoint.rotation);
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
        Vector3 fleePos = transform.position + fleeDir * fleeDistance;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(fleePos, out hit, 1.0f, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }

    }

}
