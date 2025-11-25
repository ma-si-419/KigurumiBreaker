using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MohikanBoss : BossEnemy
{
    // 敵の頭
    [Header("敵の頭のトランスフォーム")]
    [SerializeField] private GameObject _headPos;
    [Header("敵の足のトランスフォーム")]
    [SerializeField] private GameObject _footPos;

    //攻撃オブジェクトを一度だけ生成するフラグ
    private bool _isCreateAttack = false;

    private float CHARGE_SPEED = 35.0f; // 突進速度
    private float CHARGE_TIME = 0.6f; // 突進時間

    private bool _isCharge = false;    // 突進中かどうかのフラグ

    private float _createTimer = 0.0f; // 攻撃オブジェクト生成タイマー

    // タックル攻撃
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
                // 攻撃オブジェクトを破棄
                Destroy(_attackObj);

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

    // 乱れ撃ち攻撃
    public override void AttackType2()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        if(stateInfo.IsName("AttackType2"))
        {
            if (stateInfo.normalizedTime >= 0.6f)
            {
                //攻撃判定を一つ生成させる

            }

            if (!_isCreateAttack)
            {
                _isCreateAttack = true;
                //EnemyAttackCreate(0.0f, 0.5f, _attackType2ObjectPrefab);
                Shoot();
            }

            //敵のアニメーション状態を取得
            if (stateInfo.normalizedTime >= 0.8f)
            {
                // 攻撃オブジェクトを破棄
                Destroy(_attackObj);

                //攻撃フラグをリセット
                _isCreateAttack = false;
                _isStateChange = true;
            }
        }

        if (_isCreateAttack)
        {
            _createTimer++;

            if (_createTimer >= 5)
            {
                _isCreateAttack = false;
                _createTimer = 0;
            }
        }

        if (_isStateChange)
        {
            _isStateChange = false;
            ChangeState(new BossIdleState(this));
        }
    }

    // スタンプ攻撃
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
                // 攻撃オブジェクトを破棄
                Destroy(_attackObj);

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

    // ビーム攻撃
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

            if (stateInfo.normalizedTime >= 0.5f)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    Debug.Log("ビーム");
                    _isCreateAttack = true;
                    EnemyAttackCreate(6.5f, 1.5f, _attackType4ObjectPrefab);
                }
            }

            if (stateInfo.normalizedTime >= 0.7f)
            {
                // 攻撃オブジェクトを破棄
                Destroy(_attackObj);
            }

            //敵のアニメーション状態を取得
            if (stateInfo.normalizedTime >= 0.8f)
            {
                //攻撃フラグをリセット
                _isCharge = false;
                _isStateChange = true;
                _isCreateAttack = false;
            }
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
            Vector3 nextPos = _rigidbody.position + dir * CHARGE_SPEED * Time.fixedDeltaTime;
            _rigidbody.MovePosition(nextPos); // transform.positionの代わりにこれを使う！

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _rigidbody.velocity = Vector3.zero;
        StopMovement();

    }

    //弾の生成
    private void Shoot()
    {
        //弾を生成
        GameObject attackObject = Instantiate(_attackType2ObjectPrefab, _footPos.transform.position + _footPos.transform.up * 0.3f, this.transform.rotation);

        attackObject.GetComponent<EnemyAttackCol>().SetBattleManager(_battleManager);
    }
}
