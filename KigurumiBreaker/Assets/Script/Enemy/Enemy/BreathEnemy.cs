using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathEnemy : Enemy
{
    // 敵の頭
    [Header("敵の頭のトランスフォーム")]
    [SerializeField] private GameObject _headPos;

    public override void AttackType1()
    {
        StopMovement();

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
                    EnemyAttackCreate(3.0f, 1.0f, _attackType1ObjectPrefab);
                }
            }

            // 攻撃アニメーション終了後の処理
            if (stateInfo.normalizedTime >= 0.8f)
            {
                // アニメーションが終わったら消す
                Destroy(_attackObj);

                //攻撃フラグをリセット
                _isCreateAttack = false;
                _isStateChange = true;
            }
        }

        if (_attackObj != null)
        {
            _attackObj.transform.position = _headPos.transform.position + _headPos.transform.forward * 2.0f /*- _headPos.transform.up * 0.5f*/;
            _attackObj.transform.rotation = _headPos.transform.rotation * Quaternion.Euler(90, 0, 0); ; ;
        }

        if (_isStateChange)
        {
            //攻撃終了
            _isStateChange = false;
            ChangeState(new IdleState(this));
        }

        StopMovement();

    }
}
