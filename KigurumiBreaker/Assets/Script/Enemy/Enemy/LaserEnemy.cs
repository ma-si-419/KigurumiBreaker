using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class LaserEnemy : Enemy
{
    private float _laserTimer = 0.0f;   // タイマー
    private bool _isAnim = false;      // アニメーション開始フラグ

    //[SerializeField] private GameObject _laserPrefab; // レーザープレハブ
    private TestLaser _test; // レーザー処理クラス

    public override void AttackType1()
    {
        _laserTimer += Time.deltaTime;

        StopMovement();

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if(stateInfo.IsName("AttackType1"))
        {
            // 攻撃サインの表示
            AttackSign(stateInfo.normalizedTime, _enemyData.maxAttackTime - ATTACK_SIGN_DECREASE);

            if (stateInfo.normalizedTime >= _enemyData.maxAttackTime)
            {
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(2.0f, 1.5f, _enemyData.attackPrefab[0]);
                }
            }

            if (stateInfo.normalizedTime >= 1.0f)
            {
                if(!_isAnim)
                {
                    _isAnim = true;

                    animator.ResetTrigger("AttackType1");

                    // 攻撃中のアニメーションを開始
                    animator.SetBool("UnderAttack", true);
                }
            }
        }

        //時間が経ったら攻撃終了
        if (_laserTimer > 5.0f)
        {
            // アニメーションが終わったら消す
            Destroy(_attackObj);

            // 攻撃中のアニメーションを終了
            _animator.SetBool("UnderAttack", false);

            _isAnim = false;

            //攻撃状態へ
            _laserTimer = 0.0f;
            ChangeState(new ChaseState(this));
        }
        else
        {
            LookAtPlayer();
        }

        // レーザー攻撃中の処理
        if (_attackObj != null)
        {
            // レーザー攻撃オブジェクトの位置と回転を更新
            _attackObj.transform.position = this.transform.position + this.transform.forward * 3.5f + this.transform.up * 1.5f;
            _attackObj.transform.rotation = this.transform.rotation * Quaternion.Euler(90, 0, 0);

        }

    }

}
