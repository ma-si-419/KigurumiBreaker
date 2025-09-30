using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeAttackEnemy : Enemy
{
    // 攻撃に関する変数
    private float _longRangeTimer = 0.0f;   // タイマー
    public Transform firePoint;
    public float shootInterval = 0.5f; // 射撃間隔

    
    public override void Attack()
    {
        _longRangeTimer += Time.deltaTime;

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
        GameObject bullet = Instantiate(_attackHitBox, firePoint.position, firePoint.rotation);
        Debug.Log("弾を発射!!");
    }

    //ナビメッシュエージェントでターゲットから逃げる処理
    private void Nav()
    {
        
    }

}
