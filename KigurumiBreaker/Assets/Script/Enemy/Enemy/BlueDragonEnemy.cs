using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlueDragonEnemy : Enemy
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
                    EnemyAttackCreate(9.0f, 1.0f, _enemyData.attackPrefab[0]);
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

        if(_attackObj != null)
        {
            _attackObj.transform.position = _headPos.transform.position + _headPos.transform.forward * 3.0f;
            _attackObj.transform.rotation = _headPos.transform.rotation * Quaternion.Euler(90, 0, 0);
        }
        
        if (_isStateChange)
        {
            //攻撃終了
            _isStateChange = false;
            ChangeState(new IdleState(this));
        }
    }

    public override void AttackType2()
    {
        StopMovement();

        //アニメーションイベントで攻撃判定オブジェクトを生成したい
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("AttackType2"))
        {
            AttackSign(stateInfo.normalizedTime, 0.5f - ATTACK_SIGN_DECREASE);

            if (stateInfo.normalizedTime >= 0.1f)
            {
                if (!_isCreateEffect[1])
                {
                    _isCreateEffect[1] = true;
                    //エフェクト生成
                    _effectObj[1] = EffectCreate(_attackTarget, _enemyData.effectPrefab[1]);
                }
            }

            if (stateInfo.normalizedTime >= 0.3f)
            {
                if (!_isCreateEffect[0])
                {
                    _isCreateEffect[0] = true;
                    _effectObj[0] = EffectCreate(_attackTarget, _enemyData.effectPrefab[0]);
                }
            }

            // 攻撃判定生成タイミング
            if (stateInfo.normalizedTime >= 0.4f)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyBuleDragonAttackCreate(_attackTarget, _enemyData.attackPrefab[1], 3);
                }
            }

            // 攻撃アニメーション終了後の処理
            if (stateInfo.normalizedTime >= 0.8f)
            {
                // アニメーションが終わったら消す
                Destroy(_attackObj);
                Destroy(_effectObj[0]);
                Destroy(_effectObj[1]);

                //攻撃フラグをリセット
                _isCreateEffect[0] = false;
                _isCreateEffect[1] = false;
                _isCreateAttack = false;
                _isStateChange = true;
            }
        }

        if (_isStateChange)
        {
            //攻撃終了
            _isStateChange = false;
            ChangeState(new IdleState(this));
        }
    }

    public void EnemyBuleDragonAttackCreate(Vector3 pos, GameObject attackPrefab, int createNum)
    {
        // ゲームオブジェクト生成
        _attackObj = Instantiate(attackPrefab);
        _attackObj.transform.position = pos;

        // 攻撃オブジェクトにバトルマネージャーをセット
        _attackObj.GetComponent<EnemyAttackCol>().SetBattleManager(_battleManager);
    }
}
