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

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if(stateInfo.IsName("AttackSign"))
        {
            // 攻撃サインの表示
            AttackSign(stateInfo.normalizedTime, 0.1f);

            if (stateInfo.normalizedTime >= 0.2f)
            {
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(1.0f, 1.0f, _attackObjectPrefab);
                }
            }
        }

        //時間が経ったら攻撃終了
        if (_laserTimer > 15.0f)
        {
            Destroy(_attackObj);

            //攻撃状態へ
            _laserTimer = 0.0f;
            ChangeState(new ChaseState(this));
        }

        // レーザー攻撃中の処理
        if (_attackObj != null)
        {

        }

    }

}
