using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAttackEnemy : Enemy
{
    public override void AttackType1()
    {
        
        StopMovement();

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("AttackType1"))
        {
            // 攻撃サインの表示
            AttackSign(stateInfo.normalizedTime, _enemyData.maxAttackTime - ATTACK_SIGN_DECREASE);

            if(stateInfo.normalizedTime >= _enemyData.maxAttackTime - 0.25f)
            {
                if(!_isCreateEffect[0])
                {
                    _isCreateEffect[0] = true;
                    //エフェクト生成
                    _effectObj[0] = EffectCreate(this.transform.position, _enemyData.effectPrefab[0]);
                }
            }

            if (stateInfo.normalizedTime >= _enemyData.maxAttackTime)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(0.0f, 0.0f, _enemyData.attackPrefab[0]);
                }
            }

            //敵のアニメーション状態を取得
            if (stateInfo.normalizedTime >= 0.8f)
            {
                // アニメーションが終わったら消す
                Destroy(_attackObj);
                Destroy(_effectObj[0]);

                //攻撃フラグをリセット
                _isCreateAttack = false;
                _isCreateEffect[0] = false;

                //攻撃アニメーションが終了したらIdleStateに遷移
                ChangeState(new IdleState(this));
            }
        }

        Debug.Log("True");
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);

    }
}
