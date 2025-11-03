using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.VirtualTexturing;

public class Enemy : EnemyBase
{
    // 状態遷移するまでのタイマー
    protected float _stateTimer = 0.0f;
    // 状態遷移するまでのタイマー
    protected float _attackTimer = 0.0f;
    // プレイヤーを検知したかどうかのフラグ
    protected bool _isAttackRange = false;
    // 攻撃オブジェクトを生成したかどうかのフラグ
    protected bool _isCreateAttack = false;
    // プレイヤーを一度でも検知したかどうかのフラグ
    protected bool _isSearched = false;
    // 状態遷移フラグ
    protected bool _isStateChange = false;
    // 攻撃がヒットしたかどうかのフラグ
    protected bool _isHit = false;
    // ヒットタイマー
    protected float _hitTimer = 0.5f;
    // 追跡から待機に戻るフラグ
    protected bool _isChasetoIdle = false;
    // デバッグ用の待機フラグ
    protected bool _isDebugIdleFlag = false;

    protected float _dTime;

    // 敵がビックリマークを生成したかどうかのフラグ
    private bool _isDetectionMark;

    // EnemyUiManagerの参照
    protected EnemyBarManager _enemyUiManager;

    // アーマー用の処理のList変数
    [Header("アーマー用の処理(アーマー無しなら関係なし)")]
    [SerializeField] protected SkinnedMeshRenderer[] _armorSkinedMeshRenderer;

    protected override void Start()
    {
        // 親クラスのStart()を呼び出す
        base.Start();

        // 敵のバーUiの管理クラスを取得
        _enemyUiManager = FindObjectOfType<EnemyBarManager>();
        _enemyUiManager.CreateEnemyBar(this);

        // デバッグ用のフラグを取得
        _isDebugIdleFlag = _enemyCommonData.isStopAllAction;

        if (_isDebugIdleFlag)
        {
            //待機状態に設定
            ChangeState(new DebugIdleState(this));
            return;
        }
        else
        {
            //待機状態に設定
            ChangeState(new IdleState(this));
            return;
        }
    }

    protected override void Update()
    {
        // デバッグ用に線を引く
        DebugLine();

        // デバッグ用のフラグを取得
        _isDebugIdleFlag = _enemyCommonData.isStopAllAction;

        // 敵のY座標だけ固定
        Vector3 pos = transform.position;
        pos.y = 0.0f;
        transform.position = pos;

        // アーマー状態の管理
        if (_isArmor && _currentTrunk <= 0)
        {
            _isArmor = false;
        }

        if (!_isArmor)
        {
            if (_armorSkinedMeshRenderer != null)
            {
                for(int i = 0; i < _armorSkinedMeshRenderer.Length; i++)
                {
                    // アーマーが壊れたらアーマーメッシュを非表示にする
                    _armorSkinedMeshRenderer[i].enabled = false;
                }
            }
        }

        // ヒットストップ処理
        float shakeMagnitude = _enemyCommonData.shakeMagnitude;

        if (_isStop)
        {
            if (_isDamage)
            {
                _shakeVec.x = Random.Range(-shakeMagnitude, shakeMagnitude);
                _shakeVec.z = Random.Range(-shakeMagnitude, shakeMagnitude);
            }

            this.transform.position += _shakeVec;
        }
        else
        {
            // ヒットストップ終了後の位置補正
            if (_shakeVec.sqrMagnitude >= 0.001f)
            {
                //ダメージを食らっていた敵だけ位置補正を行う
                if (!_isDamage)
                {
                    this.transform.position = _stopPos;
                }
            }

            _shakeVec = Vector3.zero;
        }

        if (_isStop) return;

        if (_isDead)
        {
            // すでに死亡状態なら変更しない
            if (!(_currentState is DeadState))
            {
                ChangeState(new DeadState(this));
            }
        }

        // 親クラスのUpdate()を呼び出す
        base.Update();
    }

    //基本待機処理(オーバーライドで変更可)
    public virtual void Idle()
    {
        // デバッグ用の待機フラグが立っていたら待機状態にする
        if (_isDebugIdleFlag)
        {
            //待機状態に設定
            ChangeState(new DebugIdleState(this));
            return;
        }

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
                // 敵の検知マークを生成
                OnPlayerDetected();
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

        if (_isAttackRange)
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

        // Rigidbodyの移動を停止(プレイヤーと衝突した際に吹っ飛ばされないため)
        StopMovement(); 

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
                //追跡を停止
                _agent.isStopped = true;

                //攻撃状態へ
                _stateTimer = 0.0f;
                ChangeState(new AttackState(this));
            }

            _isChasetoIdle = true;
        }
        else
        {
            _isChasetoIdle = false;
        }

        //アニメーションの切り替え
        if (_isChasetoIdle)
        {
            _animator.SetBool("Chase", false);
            _animator.SetBool("Idle", true);
        }
        else
        {
            _animator.SetBool("Chase", true);
            _animator.SetBool("Idle", false);
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

            // アーマーでない場合は通常通りダメージを受ける
            if(!_isArmor)
            {
                // ダメージを受ける(プレイヤーアタックのダメージを取得する)
                _currentHp -= other.GetComponent<PlayerAttack>().GetDamage();
                //_enemyBarUi.SetHp(_currentHp, _enemyData.maxHp);
            }
            // アーマーの場合は耐久力を減らす
            else if (_isArmor)
            {
                // 耐久力を減らす(プレイヤーアタックの耐久力ダメージを取得する)
                _currentTrunk -= other.GetComponent<PlayerAttack>().GetDamage();
                //_enemyBarUi.SetHp(_currentTrunk, _enemyData.maxTrunk);
            }

            //ヒットストップ処理
            PlayerAttack playerAttack = other.gameObject.GetComponent<PlayerAttack>();
            BattleManager manager = _battleManager.GetComponent<BattleManager>();
            manager.SetHitStop(playerAttack.GetHitStopTime());

            //ダメージフラグを立てる
            _isDamage = true;

            // ヒット時の揺れ用の位置を保存
            _stopPos = this.transform.position;

            // ダメージエフェクトを生成する
            //Instantiate(_damageEffect, transform.position, Quaternion.identity);
            if (_currentTrunk <= 0)
            {
                _currentTrunk = 0;
            }


            // Hpが0以下なら死亡処理に遷移
            if (_currentHp <= 0)
            {
                _currentHp = 0;
                _isDead = true;
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

            // アーマーでない場合は通常通りダメージを受ける
            if (!_isArmor)
            {
                // ダメージを受ける(プレイヤーアタックのダメージを取得する)
                _currentHp -= other.GetComponent<PlayerRangedAttack>().GetDamage();
                //_enemyBarUi.SetHp(_currentHp, _enemyData.maxHp);

            }
            // アーマーの場合は耐久力を減らす
            else if (_isArmor)
            {
                // 耐久力を減らす(プレイヤーアタックの耐久力ダメージを取得する)
                _currentTrunk -= other.GetComponent<PlayerRangedAttack>().GetDamage();
                //_enemyBarUi.SetHp(_currentTrunk, _enemyData.maxTrunk);
            }

            //ヒットストップ処理
            PlayerRangedAttack playerRangedAttack = other.gameObject.GetComponent<PlayerRangedAttack>();
            BattleManager manager = _battleManager.GetComponent<BattleManager>();
            manager.SetHitStop(playerRangedAttack.GetHitStopTime());

            //ダメージフラグを立てる
            _isDamage = true;

            // ヒット時の揺れ用の位置を保存
            _stopPos = this.transform.position;

            if (_currentTrunk <= 0)
            {
                _currentTrunk = 0;
            }

            // Hpが0以下なら死亡処理に遷移
            if (_currentHp <= 0)
            {
                _currentHp = 0;
                _isDead = true;
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
        //今のステート状態のアニメーションにダメージアニメーションを重ねる
        _animator.CrossFade("Damage", 0.0f);

        //なんか演出とかあったらいいよね()
        
    }

    //デバッグ用に線を引く
    public void DebugLine()
    {
        //プレイヤーとの位置差を表示
        //Debug.DrawLine(transform.position, player.transform.position, Color.green);

        //敵の検知範囲を球で表示
        Debug.DrawLine(transform.position, transform.position + transform.forward * Mathf.Sqrt(_detectRangeSqr), Color.blue);

        //敵の攻撃範囲を表示
        Debug.DrawLine(transform.position, transform.position + transform.forward * Mathf.Sqrt(_attackRangeSqr), Color.red);
    }

    public bool GetIsSearched()
    {
        return _isSearched;
    }

    private void OnPlayerDetected()
    {
        if (_isDetectionMark) return;   // すでにビックリマークが表示されている場合は処理を行わない

        // 敵の検知マークを生成
        _enemyUiManager.CreateEnemyDetectionMark(this);
        _isDetectionMark = true;
    }

}

