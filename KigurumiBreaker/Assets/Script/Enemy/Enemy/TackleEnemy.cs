using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TackleEnemy : Enemy
{
    private bool _isCharge = false; // 突進中かどうかのフラグ

    /* 定数 */
    private const float ATTACK_DISTANCE = 1.0f; // 攻撃判定の距離
    private float CHARGE_SPEED = 10.0f; // 突進速度
    private float CHARGE_TIME = 1.5f; // 突進時間

    public override void AttackType1()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //攻撃開始
        if (stateInfo.IsName("AttackType1"))
        {
            AttackSign(stateInfo.normalizedTime, _enemyData.maxAttackTime - ATTACK_SIGN_DECREASE);

            if (stateInfo.normalizedTime >= _enemyData.maxAttackTime)
            {
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(1.0f, 0.5f, _enemyData.attackPrefab[0]);
                }

                if (!_isCharge)
                {
                    StartCoroutine(DoCharge());
                }
            }

            //敵のアニメーションが終わったらIdleStateに遷移
            if (stateInfo.normalizedTime >= 0.6f)
            {
                // アニメーションが終わったら消す
                Destroy(_attackObj);

                StopMovement();
                _isCreateAttack = false;
                _isCharge = false;
                ChangeState(new IdleState(this));
            }
        }
        else
        {
            LookAtPlayer();
        }

        if (_attackObj != null)
        {
            // 破棄されていない場合のみ位置を更新
            _attackObj.transform.position = this.transform.position + this.transform.forward * ATTACK_DISTANCE + this.transform.up * 0.5f;
        }
    }

    private IEnumerator DoCharge()
    {
        _isCharge = true;
        float timer = 0f;
        Vector3 dir = transform.forward.normalized;

        while (timer < CHARGE_TIME && _isCharge)
        {
            Vector3 nextPos = _rigidbody.position + dir * CHARGE_SPEED * Time.fixedDeltaTime;
            _rigidbody.MovePosition(nextPos); // transform.positionの代わりにこれを使う！

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        StopMovement();
    }

}
