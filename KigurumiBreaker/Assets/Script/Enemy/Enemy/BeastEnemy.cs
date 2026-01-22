using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastEnemy : Enemy
{
    // 突進中かどうかのフラグ
    private bool _isCharge = false;
    private Vector3 _chargeDir;

    /* 定数 */
    private const float ATTACK_DISTANCE = 3.0f; // 攻撃判定の距離
    private const float ATTACK_UP = 1.0f; // 攻撃判定の距離
    private float CHARGE_SPEED = 8.0f; // 突進速度
    private const float CHARGE_TIME = 0.8f; // 突進時間


    public override void AttackType1()
    {
        //アニメーションイベントで攻撃判定オブジェクトを生成したい
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("AttackType1"))
        {
            AttackSign(stateInfo.normalizedTime, _enemyData.maxAttackTime - ATTACK_SIGN_DECREASE);

            if (stateInfo.normalizedTime >= _enemyData.maxAttackTime - 0.2f)
            {
                if (!_isCreateEffect[0])
                {
                    _isCreateEffect[0] = true;
                    //エフェクト生成
                    _effectObj[0] = EffectCreate(this.transform.position, _enemyData.effectPrefab[0]);
                }
            }

            if (stateInfo.normalizedTime >= _enemyData.maxAttackTime - 0.1f)
            {
                if (!_isCreateEffect[1])
                {
                    _isCreateEffect[1] = true;
                    //エフェクト生成
                    _effectObj[1] = EffectCreate(this.transform.position, _enemyData.effectPrefab[1]);
                }
            }

            // 攻撃判定生成タイミング
            if (stateInfo.normalizedTime >= _enemyData.maxAttackTime)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(0.0f, 0.0f, _enemyData.attackPrefab[0]);
                }
            }

            // 攻撃アニメーション終了後の処理
            if (stateInfo.normalizedTime >= 0.8f)
            {
                // アニメーションが終わったら消す
                Destroy(_attackObj);
                Destroy(_effectObj[1]);

                _isCreateEffect[0] = false;
                _isCreateEffect[1] = false;

                //攻撃フラグをリセット
                _isCreateAttack = false;
                _isStateChange = true;
            }
        }


        // 状態遷移
        if(_isStateChange)
        {
            //攻撃終了
            _isStateChange = false;
            ChangeState(new IdleState(this));
        }

        StopMovement();

    }

    public override void AttackType2()
    {

        //アニメーションイベントで攻撃判定オブジェクトを生成したい
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("AttackType2"))
        {
            AttackSign(stateInfo.normalizedTime, 0.5f - ATTACK_SIGN_DECREASE);

            if (stateInfo.normalizedTime >= 0.3f)
            {
                if (!_isCharge)
                {
                    StartCoroutine(DoCharge());
                }
            }

            // 攻撃判定生成タイミング
            if (stateInfo.normalizedTime >= 0.5f)
            {
                StopMovement();

                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(ATTACK_DISTANCE, ATTACK_UP, _enemyData.attackPrefab[1]);
                }
            }

            // 攻撃アニメーション終了後の処理
            if (stateInfo.normalizedTime >= 0.8f)
            {
                // アニメーションが終わったら消す
                Destroy(_attackObj);

                //攻撃フラグをリセット
                _isCharge = false;
                _isCreateAttack = false;
                _isStateChange = true;
            }
        }

        if (_attackObj != null)
        {
            // 破棄されていない場合のみ位置を更新
            _attackObj.transform.position = this.transform.position + this.transform.forward * ATTACK_DISTANCE + this.transform.up * ATTACK_UP;
        }

        if (_isStateChange)
        {
            //攻撃終了
            _isStateChange = false;
            ChangeState(new IdleState(this));
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

        _rigidbody.velocity = Vector3.zero;
        StopMovement();

    }
}
