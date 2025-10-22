using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.VirtualTexturing;

public class Enemy : EnemyBase
{


    protected float _stateTimer = 0.0f; // 状態遷移するまでのタイマー
    protected float _attackTimer = 0.0f; // 状態遷移するまでのタイマー
    protected bool _isAttackRange = false; // プレイヤーを検知したかどうかのフラグ
    protected bool _isCreateAttack = false; // 攻撃オブジェクトを生成したかどうかのフラグ
    protected bool _isSearched = false;     // プレイヤーを一度でも検知したかどうかのフラグ
    protected bool _isStateChange = false;  // 状態遷移フラグ

    //ヒットストップ用
    protected Vector3 _shakeVec;
    protected Vector3 _stopPos;
    protected bool _isStop = false;
    protected bool _isDamage = false;

    protected bool _isHit = false; // 攻撃がヒットしたかどうかのフラグ
    protected float _hitTimer = 0.5f; // ヒットタイマー

    protected override void Start()
    {
        // 親クラスのStart()を呼び出す
        base.Start();

        //待機状態に設定
        ChangeState(new IdleState(this));
    }

    protected override void Update()
    {
        // デバッグ用に線を引く
        DebugLine();

        float a = 0.05f;

        if (_isStop)
        {
            if (_isDamage)
            {
                _shakeVec.x = Random.Range(-a, a);
                _shakeVec.z = Random.Range(-a, a);
            }

            this.transform.position += _shakeVec;
        }
        else
        {
            if (_shakeVec.sqrMagnitude >= 0.001f)
            {
                this.transform.position = _stopPos;
            }

            _shakeVec = Vector3.zero;
        }

        if (_isStop) return;

        // ヒットしたら一定時間ヒット状態を維持
        if (_isHit)
        {
            _hitTimer -= Time.deltaTime;

            if(_hitTimer <= 0.0f)
                _isHit = false;
                return; // ヒット中は他の処理を行わない
        }

        // 親クラスのUpdate()を呼び出す
        base.Update();
    }

    //基本待機処理(オーバーライドで変更可)
    public virtual void Idle()
    {

        //プレイヤーの位置を目的地に設定
        _agent.SetDestination(_player.transform.position);

        // 移動を停止
        StopMovement();

        Vector3 diff = _player.transform.position - transform.position; //プレイヤーとの位置差を計算

        //プレイヤーが検知範囲内にいるかチェック
        if (diff.sqrMagnitude < _detectRangeSqr || _isSearched)
        {
            //プレイヤー発見したら一度だけ呼ばれる
            if (!_isSearched)
            {
                //敵の頭上にビックリマークを出す
            }

            //一度でも攻撃範囲内に入ったらフラグを立て続ける
            _isSearched = true;

            _stateTimer += Time.deltaTime;

            if (_stateTimer > _enemyData.idleToChaseTime)
            {
                //追跡状態へ
                _stateTimer = 0.0f;
                ChangeState(new ChaseState(this));
            }
        }

        // 攻撃範囲に入ったらフラグを立てる
        if (diff.sqrMagnitude < _attackRangeSqr)
        {
            _isSearched = true;
            _isAttackRange = true;
        }
        else
        {
            _attackTimer = 0.0f;
            _isAttackRange = false;
        }

        if (_isAttackRange && _isAttackRange)
        {
            _attackTimer += Time.deltaTime;

            LookAtPlayer(); // プレイヤーの方向を向く

            if (_attackTimer > _enemyData.chaseToAttack)
            {
                //追跡状態へ
                _isAttackRange = false; // フラグをリセット
                _stateTimer = 0.0f;
                ChangeState(new AttackState(this));
            }
        }

    }

    //基本移動処理(オーバーライドで変更可)
    public virtual void Chase()
    {

        //プレイヤーの位置を目的地に設定
        _agent.SetDestination(_player.transform.position);
        StopMovement(); // Rigidbodyの移動を停止(プレイヤーと衝突した際に吹っ飛ばされないため)

        //プレイヤーとの位置差を計算
        Vector3 diff = _player.transform.position - transform.position;

        //攻撃圏内に入ると攻撃状態へ
        //プレイヤーが検知範囲内にいるかチェック
        if (diff.sqrMagnitude < _attackRangeSqr)
        {
            //プレイヤーの方向を向き続ける
            LookAtPlayer();
            //タイマーを進める
            _stateTimer += Time.deltaTime;

            if (_stateTimer > _enemyData.chaseToAttack)
            {
                _agent.isStopped = true; //追跡を停止

                //攻撃状態へ
                _stateTimer = 0.0f;
                ChangeState(new AttackState(this));
            }
        }
    }

    //基本攻撃処理(オーバーライドで変更可)
    public virtual void Attack() { }

    // 攻撃判定に触れたときの処理
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            //死んだ状態になっている場合はダメージを受けない
            if (_currentState is DeadState) return;

            // ダメージを受ける(プレイヤーアタックのダメージを取得する)
            _currentHp -= other.GetComponent<PlayerAttack>().GetDamage();

            //ヒットストップ処理
            PlayerAttack playerAttack = other.gameObject.GetComponent<PlayerAttack>();
            BattleManager manager = _battleManager.GetComponent<BattleManager>();
            manager.StopTime(playerAttack.GetHitStopTime());

            // 耐久力を減らす(プレイヤーアタックの耐久力ダメージを取得する)
            //_currentTrunk -= other.GetComponent<PlayerAttack>().GetTrunkDamage();

            // ダメージエフェクトを生成する
            //Instantiate(_damageEffect, transform.position, Quaternion.identity);

            // Hpが0以下なら死亡処理に遷移
            if (_currentHp <= 0)
            {
                _currentHp = 0;
                // すでに死亡状態なら変更しない
                if (!(_currentState is DeadState))
                {
                    ChangeState(new DeadState(this));
                }
            }

            //攻撃はいったら攻撃判定を速攻消す
            Destroy(other.gameObject);

            //攻撃状態のときはダメージアニメーションを行わない
            if (_currentState is AttackState) return;

            OnHit();
        }

        if (other.gameObject.CompareTag("PlayerRangedAttack"))
        {
            //死んだ状態になっている場合はダメージを受けない
            if (_currentState is DeadState) return;

            // プレイヤーにダメージを与える処理
            _currentHp -= other.GetComponent<PlayerAttack>().GetDamage();

            //ヒットストップ処理
            PlayerAttack playerAttack = other.gameObject.GetComponent<PlayerAttack>();
            BattleManager manager = _battleManager.GetComponent<BattleManager>();
            manager.StopTime(playerAttack.GetHitStopTime());

            // Hpが0以下なら死亡処理に遷移
            if (_currentHp <= 0)
            {
                _currentHp = 0;
                // すでに死亡状態なら変更しない
                if (!(_currentState is DeadState))
                {
                    ChangeState(new DeadState(this));
                }
            }

            //攻撃はいったら攻撃判定を速攻消す
            Destroy(other.gameObject);

            //攻撃状態のときはダメージアニメーションを行わない
            if (_currentState is AttackState) return;

            OnHit();
        }

    }

    public void AttackReset()
    {
        _isCreateAttack = false;
        _stateTimer = 0.0f;
        _attackTimer = 0.0f;
    }

    public void OnHit()
    {
        //一回だけヒット処理を行う
        if (_isHit) return;

        _isHit = true;
        _hitTimer = 0.5f;

        //今のステート状態のアニメーションにダメージアニメーションを重ねる
        _animator.CrossFade("Damage", 0.01f);

        //なんか演出とかあったらいいよね

    }

    //デバッグ用に線を引く
    public void DebugLine()
    {
        //プレイヤーとの位置差を表示
        Debug.DrawLine(transform.position, player.transform.position, Color.green);

        //敵の検知範囲を球で表示
        Debug.DrawLine(transform.position, transform.position + transform.forward * Mathf.Sqrt(_detectRangeSqr), Color.blue);

        //敵の攻撃範囲を表示
        Debug.DrawLine(transform.position, transform.position + transform.forward * Mathf.Sqrt(_attackRangeSqr), Color.red);
    }



}

