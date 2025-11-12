using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEnemy : Enemy
{
    private float _timer = 0;    // タイマー
    private bool _isCharge = false; // 突進中かどうかのフラグ

    /* 定数 */
    private const float ATTACK_DISTANCE = 2.0f; // 攻撃判定の距離
    private float CHARGE_SPEED = 0.3f; // 突進速度
    private float CHARGE_TIME = 0.2f; // 突進時間

    public override void AttackType1()
    {
        // NavMeshを消す
        agent.enabled = false;

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
                    EnemyAttackCreate(ATTACK_DISTANCE, 1.0f, _attackObjectPrefab);
                }

                if (!_isCharge)
                {
                    StartCoroutine(DoCharge());
                }
            }

            // 攻撃アニメーション終了後の処理
            if (stateInfo.normalizedTime >= 0.8f)
            {
                //攻撃フラグをリセット
                StopMovement();
                agent.enabled = true;
                _isCreateAttack = false;
                _isStateChange = true;
            }
        }

        if (_isStateChange)
        {
            _isStateChange = false;
            ChangeState(new IdleState(this));
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
            _rigidbody.velocity = dir * CHARGE_SPEED;

            this.transform.position += _rigidbody.velocity;

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _rigidbody.velocity = Vector3.zero; // 終了時に停止
        _isCharge = false;
    }
}
