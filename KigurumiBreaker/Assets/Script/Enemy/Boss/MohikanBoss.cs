using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MohikanBoss : BossEnemy
{
    // 敵の頭
    [Header("敵の頭のトランスフォーム")]
    [SerializeField] private GameObject _headPos;

    //攻撃オブジェクトを一度だけ生成するフラグ
    private bool _isCreateAttack = false;

    private float CHARGE_SPEED = 0.55f; // 突進速度
    private float CHARGE_TIME = 0.2f; // 突進時間

    private bool _isCharge = false;    // 突進中かどうかのフラグ

    public override void AttackType1()
    {
        // ここにタックル攻撃の具体的な処理を追加

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //攻撃開始
        if(stateInfo.IsName("AttackType1"))
        {
            if (stateInfo.normalizedTime >= 0.5f)
            {
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(0.0f, 0.5f, _attackType1ObjectPrefab);
                }

                if (!_isCharge)
                {
                    StartCoroutine(DoCharge());
                }
            }
            else
            {
                LookAtPlayer();
            }

            if (stateInfo.normalizedTime >= 0.65f)
            {
                StopMovement();
                _isCreateAttack = false;
                _isStateChange = true;
            }
        }

        if (_attackObj != null)
        {
            // 破棄されていない場合のみ位置を更新
            _attackObj.transform.position = this.transform.position;
        }

        //敵のアニメーションが終わったらIdleStateに遷移
        if(_isStateChange)
        {
            _isStateChange = false;
            ChangeState(new BossIdleState(this));
        }

    }

    public override void AttackType2()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        if(stateInfo.IsName("AttackType2"))
        {
            if (stateInfo.normalizedTime >= 0.6f)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(0.0f, 0.5f, _attackType2ObjectPrefab);
                }
            }

            //敵のアニメーション状態を取得
            if (stateInfo.normalizedTime >= 0.8f)
            {
                //攻撃フラグをリセット
                _isCreateAttack = false;
                _isStateChange = true;
            }
        }

        if (_isStateChange)
        {
            _isStateChange = false;
            ChangeState(new BossIdleState(this));
        }
    }

    public override void AttackType3()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("AttackType3"))
        {
            if (stateInfo.normalizedTime >= 0.6f)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(0.0f, 0.0f, _attackType3ObjectPrefab);
                }
            }

            //敵のアニメーション状態を取得
            if (stateInfo.normalizedTime >= 0.8f)
            {
                //攻撃フラグをリセット
                _isCreateAttack = false;
                _isStateChange = true;
            }
        }

        if (_isStateChange)
        {
            _isStateChange = false;
            ChangeState(new BossIdleState(this));
        }
    }

    public override void AttackType4()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("AttackType4"))
        {
            // プレイヤーの方を向く
            if (stateInfo.normalizedTime <= 0.2f)
            {
                LookAtPlayer();
            }

            if (stateInfo.normalizedTime >= 0.4f)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(5.8f, 2.0f, _attackType4ObjectPrefab);
                }
            }

            //敵のアニメーション状態を取得
            if (stateInfo.normalizedTime >= 0.8f)
            {
                //攻撃フラグをリセット
                _isStateChange = true;
                _isCreateAttack = false;
            }
        }

        // 攻撃判定が生成されている時の処理 
        if (_attackObj != null)
        {
            //_attackObj.transform.position = _headPos.transform.position + _headPos.transform.forward * 2.0f;
            //_attackObj.transform.rotation = _headPos.transform.rotation * Quaternion.Euler(90, 0, 0);
        }

        if (_isStateChange)
        {
            _isStateChange = false;
            ChangeState(new BossIdleState(this));
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

        _isCharge = false;
    }
}
