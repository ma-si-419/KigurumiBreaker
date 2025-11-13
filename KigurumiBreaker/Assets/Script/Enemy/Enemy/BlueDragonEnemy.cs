using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlueDragonEnemy : Enemy
{
    // 敵の頭
    [Header("敵の頭のトランスフォーム")]
    [SerializeField] private Transform _headPos;

    // 攻撃判定の場所
    private Transform _target;

    private bool _isD = false;

    public override void AttackType1()
    {
        // 敵の頭が入っているか
        if (_headPos != null)
        {
            // 頭のTransformを取得
            _target = _headPos;
        }
        else
        {
            Debug.Log("頭のトランス入ってないです。");
        }

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
                    //EnemyAttackCreate(3.0f, 1.0f, _attackType1ObjectPrefab);
                    EnemyBuleDragonAttackCreate(_headPos, _attackType2ObjectPrefab);

                }
            }

            // 攻撃アニメーション終了後の処理
            if (stateInfo.normalizedTime >= 0.8f)
            {
                //攻撃フラグをリセット
                _isCreateAttack = false;
                _isStateChange = true;
            }
        }

        if(_attackObj != null)
        {
            _attackObj.transform.position = _headPos.transform.position + this.transform.forward * 3.5f + this.transform.up * 1.5f;
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

    public override void AttackType2()
    {
        //アニメーションイベントで攻撃判定オブジェクトを生成したい
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("AttackType2"))
        {
            AttackSign(stateInfo.normalizedTime, 0.5f - ATTACK_SIGN_DECREASE);

            if (stateInfo.normalizedTime <= 0.2f && !_isD)
            {
                _isD = true;
            }

            // 攻撃判定生成タイミング
            if (stateInfo.normalizedTime >= 0.4f)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    //EnemyAttackCreate(1.0f, 1.0f, _attackType2ObjectPrefab);
                    EnemyBuleDragonAttackCreate(_target, _attackType2ObjectPrefab);
                }
            }

            // 攻撃アニメーション終了後の処理
            if (stateInfo.normalizedTime >= 0.8f)
            {
                //攻撃フラグをリセット
                _isD = false;
                _isCreateAttack = false;
                _isStateChange = true;
            }
        }

        if (!_isD)
        {
            _target = player.transform;
        }

        if (_isStateChange)
        {
            //攻撃終了
            _isStateChange = false;
            ChangeState(new IdleState(this));
        }

        StopMovement();

    }

    public void EnemyBuleDragonAttackCreate(Transform pos, GameObject attackPrefab)
    {
        // ゲームオブジェクト生成
        _attackObj = Instantiate(attackPrefab, pos);

        // 攻撃オブジェクトにバトルマネージャーをセット
        _attackObj.GetComponent<EnemyAttackCol>().SetBattleManager(_battleManager);
    }
}
