using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UI;

public class Enemy : EnemyBase
{
    // 状態遷移するまでのタイマー
    protected float _stateTimer = 0.0f;

    // 状態遷移するまでのタイマー
    protected float _attackTimer = 0.0f;

    // プレイヤーを検知したかどうかのフラグ
    protected bool _isAttack = false;

    // 攻撃オブジェクトを生成したかどうかのフラグ
    protected bool _isCreateAttack = false;

    // プレイヤーを一度でも検知したかどうかのフラグ
    protected bool _isSearched = false;

    // 攻撃がヒットしたかどうかのフラグ
    protected bool _isHit = false;

    // ヒットタイマー
    protected float _hitTimer = 0.5f;

    // 追跡から待機に戻るフラグ
    protected bool _isChasetoIdle = false;

    // デバッグ用の待機フラグ
    protected bool _isDebugIdleFlag = false;

    // 攻撃サインの経過時間
    protected float _attackSignTime;

    // 敵がビックリマークを生成したかどうかのフラグ
    private bool _isDetectionMark;



    // ヒットストップ用の揺れベクトル
    protected float _idleTime;

    // 定数
    protected const float ATTACK_SIGN_DECREASE = 0.1f;
    protected const int HIT_ATTACK_ID_MAX = 10;

    // 受けた攻撃のIDリスト
    protected List<int> _hitAttackIds = new List<int>();

    // アーマー用のSkinnedMeshRenderer変数
    [Header("アーマー用の処理(アーマー無しなら関係なし)")]
    [SerializeField] protected SkinnedMeshRenderer[] _armorSkinedMeshRenderer;
    // アウトライン用のマテリアル変数
    [Header("アウトライン用のマテリアル")]
    [SerializeField] protected SkinnedMeshRenderer[] _outlineSkinedMeshRenderer;
    // 攻撃サイン用のSkinnedMeshRenderer変数
    [Header("全キャラ共通の攻撃サイン用のメッシュ")]
    [SerializeField] protected SkinnedMeshRenderer[] _attackSignSkinedMeshRenderer;

    public bool isChasetoIdle => _isChasetoIdle;

    protected override void Start()
    {
        // 親クラスのStart()を呼び出す
        base.Start();

        // アウトラインの色を設定
        OutLine();

        // 初速を最高速度
        _agent.acceleration = 999f;

        // 敵のバーUiの管理クラスを取得
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

    protected override void FixedUpdate()
    {
        // デバッグ用に線を引く
        DebugLine();

        // アウトラインの色を設定
        OutLine();

        // デバッグ用のフラグを取得
        _isDebugIdleFlag = _enemyCommonData.isStopAllAction;

        // アーマー状態の管理
        if (_isArmor && _currentTrunk <= 0)
        {
            _isArmor = false;
        }

        if (!_isArmor)
        {
            if (_armorSkinedMeshRenderer != null)
            {
                for (int i = 0; i < _armorSkinedMeshRenderer.Length; i++)
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

        _isDamage = false; // ダメージフラグをリセット

        if (_isDead)
        {
            // 死んだら攻撃サインを非表示にする
            for (int i = 0; i < _attackSignSkinedMeshRenderer.Length; i++)
            {
                // 攻撃サインを表示してフェード値を設定
                _attackSignSkinedMeshRenderer[i].material.SetFloat("_Alpha", 0.0f);
            }

            // すでに死亡状態なら変更しない
            if (!(_currentState is DeadState))
            {
                ChangeState(new DeadState(this));
            }
        }

        // 親クラスのUpdate()を呼び出す
        base.FixedUpdate();
    }

    //基本待機処理(オーバーライドで変更可)
    public virtual void Idle()
    {
        // プレイヤーの座標を代入し続ける
        _attackTarget = _player.transform.position;

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
            _isAttack = true;
        }
        else
        {
            _attackTimer = 0.0f;
            _isAttack = false;
        }

        // 攻撃範囲に入っていたらタイマーを進める
        if (_isAttack)
        {
            _attackTimer += Time.deltaTime;
            LookAtPlayer(); // プレイヤーの方向を向く

            if (_attackTimer > _enemyData.chaseToAttack)
            {
                //追跡状態へ
                _isAttack = false; // フラグをリセット
                _stateTimer = 0.0f;

                if (_enemyData.isStrongEnemy)
                {
                    if (diff.sqrMagnitude < _attackSwitchRangeSqr)
                    {
                        ChangeState(new AttackType1State(this));
                    }
                    else
                    {
                        ChangeState(new AttackType2State(this));
                    }
                }
                else
                {
                    //攻撃状態へ
                    ChangeState(new AttackType1State(this));
                }
            }
        }

    }

    //基本移動処理(オーバーライドで変更可)
    public virtual void Chase()
    {
        // プレイヤーの座標を代入し続ける
        _attackTarget = _player.transform.position;

        ////アニメーションイベントで攻撃判定オブジェクトを生成したい
        //var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //if (stateInfo.IsName("Chase"))
        //{

        //}

        _agent.SetDestination(_player.transform.position);
        LookAtPlayer();

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

                //攻撃状態へ
                _stateTimer = 0.0f;
                _idleTime = 0.0f;

                if (_enemyData.isStrongEnemy)
                {

                    if (diff.sqrMagnitude < _attackSwitchRangeSqr)
                    {
                        ChangeState(new AttackType1State(this));
                    }
                    else
                    {
                        ChangeState(new AttackType2State(this));
                    }
                }
                else
                {
                    //攻撃状態へ
                    ChangeState(new AttackType1State(this));
                }
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
    public virtual void AttackType1() { }

    public virtual void AttackType2() { }

    // 攻撃判定に触れたときの処理
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            // 同じ攻撃判定に何度も触れないようにする
            if (_hitAttackIds.Contains(other.GetInstanceID())) return;

            //死んだ状態になっている場合はダメージを受けない
            if (_currentState is DeadState) return;


            // アーマーでない場合は通常通りダメージを受ける
            if (!_isArmor)
            {
                // ダメージを受ける(プレイヤーアタックのダメージを取得する)
                _currentHp -= other.GetComponent<PlayerAttack>().GetDamage();
            }
            // アーマーの場合は耐久力を減らす
            else if (_isArmor)
            {
                // 耐久力を減らす(プレイヤーアタックの耐久力ダメージを取得する)
                _currentTrunk -= other.GetComponent<PlayerAttack>().GetDamage();
            }

            //ヒットストップ処理
            PlayerAttack playerAttack = other.gameObject.GetComponent<PlayerAttack>();
            BattleManager manager = _battleManager.GetComponent<BattleManager>();
            manager.SetHitStop(playerAttack.GetHitStopTime());

            if(_hitAttackIds.Count > HIT_ATTACK_ID_MAX)
            {
                // リストのサイズが固定値を超えたらリストをクリアする
                _hitAttackIds.Clear();
            }

            // 受けた攻撃のIDを記録
            _hitAttackIds.Add(other.GetInstanceID());

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

            if (playerAttack.GetIsHitDelete())
            {
                //攻撃はいったら攻撃判定を速攻消す
                Destroy(other.gameObject);
            }

            //攻撃状態のときはダメージアニメーションを行わない
            if (_currentState is AttackType1State) return;
            if (_currentState is AttackType2State) return;

            // 弱攻撃の場合はダメージアニメーションを行わない
            if (_isWeakAttack) return;

            //ヒット処理
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
            }
            // アーマーの場合は耐久力を減らす
            else if (_isArmor)
            {
                // 耐久力を減らす(プレイヤーアタックの耐久力ダメージを取得する)
                _currentTrunk -= other.GetComponent<PlayerRangedAttack>().GetDamage();
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

            // ドロップする弾を一つ増やす
            _dropBullets.Add(_enemyCommonData.dropBulletTime);

            //攻撃状態のときはダメージアニメーションを行わない
            if (_currentState is AttackType1State) return;
            if (_currentState is AttackType2State) return;

            // 弱攻撃の場合はダメージアニメーションを行わない
            if (_isWeakAttack) return;

            //攻撃はいったら攻撃判定を速攻消す
            Destroy(other.gameObject);

            //ヒット処理
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
        // プレイヤー→敵 の方向ベクトル
        Vector3 diff = transform.position - _player.transform.position;

        // 正規化（方向だけ欲しいので）
        Vector3 knockDir = diff.normalized;

        // ノックバック距離
        float knockPower = 0.5f;

        // ノックバック（瞬間移動系）
        transform.position += knockDir * knockPower;

        //今のステート状態のアニメーションにダメージアニメーションを重ねる
        _animator.CrossFade("Damage", 0.0f);

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

        //敵の攻撃を切り替える範囲を表示
        Debug.DrawLine(transform.position, transform.position + transform.forward * Mathf.Sqrt(_attackSwitchRangeSqr), Color.green);
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

    private void OutLine()
    {
        // アウトラインの色を設定
        for (int i = 0; i < _outlineSkinedMeshRenderer.Length; i++)
        {
            if (_isArmor)
            {
                _outlineSkinedMeshRenderer[i].material.color = Color.yellow;
            }
            else
            {
                _outlineSkinedMeshRenderer[i].material.color = Color.black;
            }
        }
    }

    // 攻撃サインの表示処理(現在のアニメーション,攻撃する瞬間の値)
    public void AttackSign(float currentAnim, float maxAnim)
    {
        // 現在のアニメーションが攻撃する瞬間の値以下なら
        if (currentAnim < maxAnim)
        {
            // 経過時間を加算
            _attackSignTime += Time.deltaTime;

            // 現在のアニメーションから攻撃する瞬間の値までの割合を計算
            float fade = Mathf.InverseLerp(0.0f, maxAnim, currentAnim);

            for (int i = 0; i < _attackSignSkinedMeshRenderer.Length; i++)
            {
                // 攻撃サインを表示してフェード値を設定
                _attackSignSkinedMeshRenderer[i].material.SetFloat("_Alpha", fade);
            }
        }
        else
        {
            for (int i = 0; i < _attackSignSkinedMeshRenderer.Length; i++)
            {
                // 攻撃サインを表示してフェード値を設定
                _attackSignSkinedMeshRenderer[i].material.SetFloat("_Alpha", 0.0f);
            }
        }
    }
}

