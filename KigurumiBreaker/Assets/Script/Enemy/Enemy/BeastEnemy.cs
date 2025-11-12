using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastEnemy : Enemy
{
    private float _timer = 0;    // タイマー

    public override void AttackType1()
    {
        //アニメーションイベントで攻撃判定オブジェクトを生成したい
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("AttackType1"))
        {
            AttackSign(stateInfo.normalizedTime, _enemyData.maxAttackTime - ATTACK_SIGN_DECREASE);

            // 攻撃判定生成タイミング
            if (stateInfo.normalizedTime >= _enemyData.maxAttackTime)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    //EnemyAttackCreate(1.0f, 1.0f, _attackObjectPrefab);
                }
            }

            // 攻撃アニメーション終了後の処理
            if (stateInfo.normalizedTime >= 0.9f)
            {
                //攻撃フラグをリセット
                _isCreateAttack = false;
                _isStateChange = true;
            }
        }

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
