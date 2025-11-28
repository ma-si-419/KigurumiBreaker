using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
    private float _tackleSpeed = 0.8f;

    // 弾の生成位置
    private Transform _bulletSpawnPoint;

    private float _circleInterval = 0.75f;

    private float _shotInterval = 0.025f;

    private float _rotSpeed = 480.0f;

    private float _angle = 0.0f;

    private float _timer = 0.0f;

    private float _attackTime = 0.0f;

    private Vector3 _dir;

    private int _tackleCount = 0;

    public override void PhaseChange()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //フェーズ移行フラグ
        _isPhaseChanged = true;
        _isPhase = true;

        if (stateInfo.IsName("Phase"))
        {
            if (stateInfo.normalizedTime >= 0.5f)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(0.0f, 0.0f, _enemyData.attackPrefab[6]);
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
        else
        {
            // 攻撃オブジェクトを破棄
            Destroy(_attackObj);
        }

        if (_isStateChange)
        {
            _isStateChange = false;
            // 攻撃オブジェクトを破棄
            Destroy(_attackObj);
            ChangeState(new BossIdleState(this));
        }
    }

    // タックル攻撃
    public override void AttackType1()
    {

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);



        //攻撃開始
        if (stateInfo.IsName("AttackType1"))
        {
            if (stateInfo.normalizedTime >= 0.5f)
            {
                animator.ResetTrigger("AttackType1");

                animator.SetBool("UnderAttackType1", true);
            }

            LookAtPlayer();

        }

        if (stateInfo.IsName("UnderAttackType1"))
        {
            _attackTime += Time.deltaTime;

            //攻撃判定を一つ生成させる
            if (!_isCreateAttack)
            {
                _isCreateAttack = true;
                EnemyAttackCreate(0.0f, 0.0f, _enemyData.attackPrefab[0]);
            }

        }

        if (_attackTime > 0.5f)
        {
            StopMovement();

            // 攻撃オブジェクトを破棄
            Destroy(_attackObj);

            _isCreateAttack = false;
            // 攻撃中のアニメーションを終了
            animator.SetBool("UnderAttackType1", false);
            _isAnim = false;
            _attackTime = 0.0f;
            ChangeState(new BossIdleState(this));
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
            LookAtPlayer();

            //敵のアニメーション状態を取得
            if (stateInfo.normalizedTime >= 1.0f)
            {
                // 攻撃合図のアニメーションが終わったらフラグ
                if(!_isAnim)
                {
                    _isAnim = true;

                    animator.ResetTrigger("AttackType2");

                    animator.SetBool("UnderAttackType2", true);
                }
            }
        }

        if(stateInfo.IsName("UnderAttackType2"))
        {
            _timer += Time.deltaTime;
            _attackTime += Time.deltaTime;

            if (_timer > _shotInterval)
            {
                _timer = 0f;

                // 発射方向の角度を計算
                _angle += _rotSpeed * _shotInterval;

                // 角度を方向ベクトルに変換
                _dir = new Vector3(Mathf.Cos(_angle * Mathf.Deg2Rad),
                                    0,
                                    Mathf.Sin(_angle * Mathf.Deg2Rad));

                EnemyShootAttackCreate(_dir);
            }
        }

        if (_attackTime > 5.0f)
        {
            // 攻撃中のアニメーションを終了
            animator.SetBool("UnderAttackType2", false);

            _isAnim = false;

            _attackTime = 0.0f;

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
                    EnemyAttackCreate(0.0f, 0.0f, _enemyData.attackPrefab[2]);
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
                    EnemyAttackCreate(9.5f, 1.5f, _enemyData.attackPrefab[3]);
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

    public override void AttackType5()
    {
        // ここにタックル攻撃の具体的な処理を追加

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("AttackType5"))
        {
            if (stateInfo.normalizedTime >= 0.8f)
            {
                animator.ResetTrigger("AttackType5");

                animator.SetTrigger("UnderAttackType5_1");
            }
        }

        //攻撃開始
        if (stateInfo.IsName("UnderAttackType5_1"))
        {
            _attackTime = 0.0f;

            if (stateInfo.normalizedTime >= 0.5f)
            {
                animator.ResetTrigger("UnderAttackType5_1");

                animator.SetBool("UnderAttackType5_2", true);
            }

            LookAtPlayer();

        }

        if (stateInfo.IsName("UnderAttackType5_2"))
        {
            _attackTime += Time.deltaTime;

            //攻撃判定を一つ生成させる
            if (!_isCreateAttack)
            {
                _isCreateAttack = true;
                EnemyAttackCreate(0.0f, 0.0f, _enemyData.attackPrefab[0]);
            }

            if (!_isWallHit)
            {
                Vector3 dir = transform.forward.normalized;
                Vector3 nextPos = _rigidbody.position + dir * _tackleSpeed;
                _rigidbody.MovePosition(nextPos); // transform.positionの代わりにこれを使う！
            }
            else
            {
                _tackleSpeed = 0.0f;
                StopMovement();
            }
        }

        if (_attackTime > 0.4f)
        {
            StopMovement();

            // 攻撃オブジェクトを破棄
            Destroy(_attackObj);

            _isCreateAttack = false;
            // 攻撃中のアニメーションを終了
            animator.SetBool("UnderAttackType5_2", false);
            animator.SetTrigger("UnderAttackType5_1");
            _isAnim = false;
            _attackTime = 0.0f;
            _tackleCount += 1;
        }

        if(_tackleCount >= 3)
        {
            _tackleCount = 0;
            _isStateChange = true;
        }

        if (_attackObj != null)
        {
            // 破棄されていない場合のみ位置を更新
            _attackObj.transform.position = this.transform.position;
        }

        //敵のアニメーションが終わったらIdleStateに遷移
        if (_isStateChange)
        {
            animator.ResetTrigger("UnderAttackType5_1");
            animator.SetBool("UnderAttackType5_2", false);

            _isStateChange = false;
            ChangeState(new BossIdleState(this));
        }

    }

    public override void AttackType6()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //攻撃開始
        if (stateInfo.IsName("AttackType6"))
        {
            if (stateInfo.normalizedTime >= 0.5f)
            {
                animator.ResetTrigger("AttackType6");

                animator.SetBool("UnderAttackType6", true);
            }

            LookAtPlayer();

        }

        if (stateInfo.IsName("UnderAttackType6"))
        {
            _attackTime += Time.deltaTime;
            _timer += Time.deltaTime;

            if (_timer < _circleInterval * 0.5f)
            {
                // 攻撃ターゲットをプレイヤーの位置に更新
                _attackTarget = _player.transform.position;
            }

            if (_timer > _circleInterval)
            {
                _timer = 0f;
                EnemyTargetAttackCreate(_attackTarget, _enemyData.attackPrefab[5]);
            }
        }

        if (_attackTime > 5.0f)
        {
            // 攻撃中のアニメーションを終了
            animator.SetBool("UnderAttackType6", false);
            _isAnim = false;
            _attackTime = 0.0f;
            _isCreateAttack = false;
            _isStateChange = true;
        }

        //敵のアニメーションが終わったらIdleStateに遷移
        if (_isStateChange)
        {
            _isStateChange = false;
            ChangeState(new BossIdleState(this));
        }
    }
}
