using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastEnemy : Enemy
{
    private float _timer = 0;    // タイマー

    public override void Attack()
    {
        _timer += Time.deltaTime;

        if(_timer >= 2.0f)
        {
            //攻撃終了
            _timer = 0.0f;
            ChangeState(new IdleState(this));
        }

        StopMovement();

    }



}
