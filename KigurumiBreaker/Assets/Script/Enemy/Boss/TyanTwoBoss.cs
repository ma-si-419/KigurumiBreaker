using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyanTwoBoss : BossEnemy
{
    // 突進速度
    private float _tackleSpeed = 0.8f;

    private float _circleInterval = 0.75f;

    private float _timer = 0.0f;

    private float _attackTime = 0.0f;

    private int _tackleCount = 0;

    private Vector3[] _dir = new Vector3[7];

    private float[] _angle = { 0, 45, 90, 135, 180, 225, 270, 315 };


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
            LookAtPlayer();

            if (stateInfo.normalizedTime <= 0.3f)
            {
                // 攻撃ターゲットをプレイヤーの位置に更新
                _attackTarget = _player.transform.position;
            }

            if (stateInfo.normalizedTime >= 0.5f)
            {
                if(!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyTargetAttackCreate(_attackTarget, _enemyData.attackPrefab[0]);
                }
            }

            if(stateInfo.normalizedTime >= 0.8f)
            {
                _isStateChange = true;
            }

        }

        //敵のアニメーションが終わったらIdleStateに遷移
        if (_isStateChange)
        {
            _isCreateAttack = false;
            _isStateChange = false;
            ChangeState(new BossIdleState(this));
        }

    }

    // 乱れ撃ち攻撃
    public override void AttackType2()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("AttackType2"))
        {
            LookAtPlayer();

            if (stateInfo.normalizedTime <= 0.2f)
            {
                // 向き（dirベクトル）を計算
                Vector3 dir = player.transform.position - this.transform.position;
                Vector3 dirNorm = dir.normalized;
                _dir[0] = dirNorm;
            }

            if (stateInfo.normalizedTime >= 0.3f)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyShootAttackCreate(_dir[0], _enemyData.attackPrefab[1]);
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
            Destroy(_attackObj);

            _isStateChange = false;
            ChangeState(new BossIdleState(this));
        }
    }

    // 六面レーザー攻撃
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

                    //ここに6方面の弾を撃つ処理を追加したい                    
                    foreach (var angle in _angle)
                    {
                        // 向き（dirベクトル）を計算
                        Quaternion rot = Quaternion.Euler(0f, angle, 0f);

                        Vector3 dir = rot * Vector3.forward; // forward を角度で回転させて方向を作る

                        // 弾生成関数を呼ぶ（あなたの関数）
                        EnemyShootAttackCreate(dir, _enemyData.attackPrefab[2]);
                    }
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

            if (stateInfo.normalizedTime >= 0.4f)
            {
                //攻撃判定を一つ生成させる
                if (!_isCreateAttack)
                {
                    _isCreateAttack = true;
                    EnemyAttackCreate(9.5f, 1.5f, _enemyData.attackPrefab[3]);
                }
            }
            else
            {
                LookAtPlayer();
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
            _attackTime = 0.0f;
            _tackleCount += 1;
        }

        if (_tackleCount >= 3)
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