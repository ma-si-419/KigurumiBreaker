using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemy : Enemy
{
    private float _laserTimer = 0.0f;   // タイマー

    public override void Attack()
    {
        _laserTimer += Time.deltaTime;

        //カプセルを設定している最大距離まで伸ばす

        //時間が経ったら攻撃終了
        if (_laserTimer > 3.0f)
        {
            //攻撃状態へ
            _laserTimer = 0.0f;
            ChangeState(new ChaseState(this));
        }


    }

}
