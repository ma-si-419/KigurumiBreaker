using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchEnemy : Enemy
{

    private GameObject _attackObject; // 攻撃オブジェクト


    [SerializeField] private float _dashSpeed; // 前進速度
    [SerializeField] private float _dashTime; // 前進時間
    [SerializeField] private float _attackDistance; // 攻撃判定の距離

    private bool _isDash = false; // 前進したかどうかのフラグ

    /* 定数 */
    private const float ATTACK_DISTANCE = 1.0f; // 攻撃判定の距離

    public override void AttackType1()
    {
        // 前進動作
        if (!_isDash)
        {
            StartCoroutine(DoDash());
        }
        else
        {
            StopMovement();
        }

        //アニメーションイベントで攻撃判定オブジェクトを生成したい
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("AttackType1"))
        {
            AttackSign(stateInfo.normalizedTime, _enemyData.maxAttackTime - ATTACK_SIGN_DECREASE);

            if (stateInfo.normalizedTime >= _enemyData.maxAttackTime - 0.2f)
            {
                if (!_isCreateEffect[1])
                {
                    _isCreateEffect[1] = true;
                    _effectObj[1] = RotationEffectCreate(this.transform.position + this.transform.up * 0.5f, enemyData.effectPrefab[1]);
                }
            }

            // 攻撃判定生成タイミング
            if (stateInfo.normalizedTime >= _enemyData.maxAttackTime)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(1.0f, 1.0f, _enemyData.attackPrefab[0]);
                }
            }

            // 攻撃アニメーション終了後の処理
            if (stateInfo.normalizedTime >= 0.9f)
            {
                // アニメーションが終わったら消す
                Destroy(_attackObj);
                Destroy(_effectObj[0]);
                Destroy(_effectObj[1]);

                _isCreateEffect[0] = false;
                _isCreateEffect[1] = false;

                //攻撃フラグをリセット
                _isCreateAttack = false;
                _isDash = false;
                _isStateChange = true;
            }

        }

        if (_isStateChange)
        {
            _isStateChange = false;
            ChangeState(new IdleState(this));
        }
    }

    private IEnumerator DoDash()
    {
        _isDash = true;
        float timer = 0f;

        while (timer < _dashTime && _isDash)
        {
            // 前進方向を計算
            Vector3 dir = (transform.forward).normalized;

            // Rigidbodyを使って突進
            transform.position += dir * _dashSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

    }
}


