using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEnemy : Enemy
{
    private bool _isCharge = false; // 突進中かどうかのフラグ

    /* 定数 */
    private const float ATTACK_DISTANCE = 2.0f; // 攻撃判定の距離
    private float CHARGE_SPEED = 23.0f; // 突進速度
    private float CHARGE_TIME = 0.3f; // 突進時間

    public override void AttackType1()
    {
        //アニメーションイベントで攻撃判定オブジェクトを生成したい
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("AttackType1"))
        {
            AttackSign(stateInfo.normalizedTime, _enemyData.maxAttackTime - ATTACK_SIGN_DECREASE);

            if (stateInfo.normalizedTime >= _enemyData.maxAttackTime - 0.1f)
            {
                if (!_isCreateEffect[0])
                {
                    _isCreateEffect[0] = true;
                    _effectObj[0] = RotationEffectCreate(this.transform.position + this.transform.forward * 0.7f, enemyData.effectPrefab[0]);
                }
            }

            // 攻撃判定生成タイミング
            if (stateInfo.normalizedTime >= _enemyData.maxAttackTime)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(ATTACK_DISTANCE, 1.0f, _enemyData.attackPrefab[0]);
                }

                if (!_isCharge)
                {
                    StartCoroutine(DoCharge());
                }
            }

            // 攻撃アニメーション終了後の処理
            if (stateInfo.normalizedTime >= 0.8f)
            {
                // アニメーションが終わったら消す
                Destroy(_attackObj);
                Destroy(_effectObj[0]);

                //攻撃フラグをリセット
                StopMovement();
                _isCharge = false;
                _isCreateAttack = false;
                _isCreateEffect[0] = false;
                _isStateChange = true;
            }
        }

        if (_isStateChange)
        {
            _isStateChange = false;
            ChangeState(new IdleState(this));
        }

        if (_effectObj[0] != null)
        {
            _effectObj[0].transform.position = this.transform.position + this.transform.forward * 0.7f;
        }

        if (_attackObj != null)
        {
            // 破棄されていない場合のみ位置を更新
            _attackObj.transform.position = this.transform.position + this.transform.forward * ATTACK_DISTANCE + this.transform.up * 0.5f;
        }
    }

    public override void AttackType2()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        StopMovement();

        if (stateInfo.IsName("AttackType2"))
        {
            // 攻撃サインの表示
            AttackSign(stateInfo.normalizedTime, _enemyData.maxAttackTime - ATTACK_SIGN_DECREASE);

            if (stateInfo.normalizedTime >= _enemyData.maxAttackTime)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    Shoot();
                }

                _isStateChange = true;
                //攻撃フラグをリセット
                _isCreateAttack = false;
            }
        }

        if (_isStateChange)
        {
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

        StopMovement();
    }

    private void Shoot()
    {
        //弾を生成
        GameObject attackObject = Instantiate(_enemyData.attackPrefab[1], this.transform.position + this.transform.up * 0.5f, this.transform.rotation);

        attackObject.GetComponent<EnemyAttackCol>().SetBattleManager(_battleManager);
    }
}
