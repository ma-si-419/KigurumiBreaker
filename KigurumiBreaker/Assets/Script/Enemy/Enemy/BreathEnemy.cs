using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathEnemy : Enemy
{
    public override void Attack()
    {
        // 敵の力を止める
        StopMovement();

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Attack"))
        {
            // 攻撃サインの表示
            AttackSign(stateInfo.normalizedTime, 0.2f);

            if (stateInfo.normalizedTime >= 0.3f)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(3.0f, 0.0f, _attackObjectPrefab);
                }
            }

            //敵のアニメーション状態を取得
            if (stateInfo.normalizedTime >= 0.8f)
            {
                //攻撃フラグをリセット
                _isCreateAttack = false;

                //攻撃アニメーションが終了したらIdleStateに遷移
                ChangeState(new IdleState(this));
            }
        }
    }


}
