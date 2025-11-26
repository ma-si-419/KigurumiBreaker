using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MohikanBoss : BossEnemy
{
    // 敵の頭
    [Header("敵の頭のトランスフォーム")]
    [SerializeField] private GameObject _headPos;
    [Header("敵の足のトランスフォーム")]
    [SerializeField] private GameObject _footPos;

    private bool _isAnim = false;      // アニメーション開始フラグ

    //攻撃オブジェクトを一度だけ生成するフラグ
    private bool _isCreateAttack = false;

    // 突進速度
    private float CHARGE_SPEED = 25.0f; 

    // 突進時間
    private float CHARGE_TIME = 0.6f; 

    // 突進中かどうかのフラグ
    private bool _isCharge = false;    

    // 攻撃オブジェクト生成タイマー
    private float _createTimer = 0.0f; 

    // 弾の生成位置
    private Transform _bulletSpawnPoint;

    private float _shotInterval = 0.1f;

    private float _rotSpeed = 180.0f;

    private float _angle = 0.0f;

    private float _timer = 0.0f;

    private float _shotTime = 0.0f;

    private Vector3 _dir;

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

            if (stateInfo.normalizedTime >= 0.8f)
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
            //敵のアニメーション状態を取得
            if (stateInfo.normalizedTime >= 1.0f)
            {
                // 攻撃合図のアニメーションが終わったらフラグ
                _isAnim = true;

                animator.ResetTrigger("AttackType2");

                animator.SetBool("UnderAttackType2", true);
            }
        }

        if(stateInfo.IsName("UnderAttackType2"))
        {
            _timer += Time.deltaTime;
            _shotTime += Time.deltaTime;

            if (_timer > _shotInterval)
            {
                _timer = 0f;

                // 発射方向の角度を計算
                _angle += _rotSpeed * _shotInterval;

                // 角度を方向ベクトルに変換
                _dir = new Vector3(Mathf.Cos(_angle * Mathf.Deg2Rad),
                                    0,
                                    Mathf.Sin(_angle * Mathf.Deg2Rad));

                Shoot();
            }
        }

        if (_shotTime > 5.0f)
        {
            // 攻撃中のアニメーションを終了
            animator.SetBool("UnderAttackType2", false);

            _isAnim = false;

            _shotTime = 0.0f;

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
                    _isCreateAttack = true;
                    EnemyAttackCreate(9.5f, 1.5f, _attackType4ObjectPrefab);
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
        Vector3 spawnPos = this.transform.position + _dir;

        //弾を生成
        GameObject attackObject = Instantiate(_attackType2ObjectPrefab, spawnPos + this.transform.up * 0.8f, Quaternion.LookRotation(_dir));

        attackObject.GetComponent<EnemyAttackCol>().SetBattleManager(_battleManager);
    }
}
