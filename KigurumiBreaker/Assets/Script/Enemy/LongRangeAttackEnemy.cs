using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeAttackEnemy : Enemy
{
    // 攻撃に関する変数
    private float _longRangeTimer = 0.0f;   // タイマー
    public Transform firePoint;
    public float shootInterval = 2.0f; // 射撃間隔

    public override void Attack()
    {
        _longRangeTimer += Time.deltaTime;
        
        if(_longRangeTimer > shootInterval)
        {
            Shoot();
            _longRangeTimer = 0.0f; // タイマーをリセット
        }

    }

    private void Shoot()
    {
        //弾を生成
        GameObject bullet = Instantiate(attackHitBox, firePoint.position, firePoint.rotation);
        Debug.Log("弾を発射!!");
    }
}
