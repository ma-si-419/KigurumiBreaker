using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEnemy : Enemy
{
    public override void AttackType1()
    {
        StopMovement();

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("AttackType1"))
        {
            // 攻撃サインの表示
            AttackSign(stateInfo.normalizedTime, _enemyData.maxAttackTime - ATTACK_SIGN_DECREASE);

            if (stateInfo.normalizedTime >= _enemyData.maxAttackTime)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(0.0f, 0.0f, _enemyData.attackPrefab[0]);
                }

                _isStateChange = true;
                //攻撃フラグをリセット
                _isCreateAttack = false;
            }
        }

        //状態遷移
        if (_isStateChange)
        {
            _isStateChange = false;
            _currentHp = 0; // 自分の体力を0にする
            _currentTrunk = 0; // 自分の耐久力を0にする
            ChangeState(new DeadState(this));
        }

    }
}
