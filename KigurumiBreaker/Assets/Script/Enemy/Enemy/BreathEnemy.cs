using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathEnemy : Enemy
{
    public override void Attack()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Attack"))
        {
            // 攻撃サインの表示
            AttackSign(stateInfo.normalizedTime, 0.5f);

            if (stateInfo.normalizedTime >= 0.6f)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(2.0f, 2.0f, _attackObjectPrefab);
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
