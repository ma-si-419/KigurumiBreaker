using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEngine.Rendering.VirtualTexturing;

public class BossEnemy : EnemyBase
{
    // 状態遷移するまでのタイマー
    protected float _stateTimer = 0.0f;

    // 攻撃するまでのタイマー
    protected float _attackTimer = 0.0f;

    // フェーズチェンジしたかどうかのフラグ
    protected bool _isPhaseChanged = false;

    // フェーズフラグ
    protected bool _isPhase = false;

    // 攻撃範囲の値の二乗
    protected float _meleeAttackRangeSqr;
    protected float _specialAttackRangeSqr;

    // フェーズエフェクトオブジェクト
    protected GameObject _phaseObj;

    // ボスの全攻撃データ
    [SerializeField] protected BossAttackData _attackData;

    // ボスの情報
    [SerializeField] protected BossData _bossData;

    // ランタイム用のクールダウン管理クラスのリスト
    private List<BossAttackRuntime> _runtimesAttacks = new();

    protected float testRange;
    protected float testRangeSqr;

    protected GameObject _phaseEffectObj;

    public float meleeAttackRangeSqr => _meleeAttackRangeSqr;
    public float specialAttackRangeSqr => _specialAttackRangeSqr;

    protected bool _isWallHit;

    // 攻撃中に壁に当たったかどうかのフラグ
    protected bool _isAttackWallHit;   

    protected float _maxHp;

    protected bool _isPhaseEffect;

    public BossAttackData attackData => _attackData;
    public BossData bossData => _bossData;
    public GameObject phaseEffectObj => _phaseEffectObj;

    protected override void Start()
    {
        // 親クラスのStart()を呼び出す
        base.Start();

        _phaseEffectObj = bossData.phaseEffect;

        _maxHp = _currentHp;

        // ボス専用のUIバーを作成
        _enemyUiManager.CreateBossEnemyBar(this);

        // 全攻撃データから攻撃データを1つずつ取り出し、管理クラスに変換してリスト化する
        foreach (var atk in _attackData.bossAttackDataList)
        {
            // _runtimesAttacksに攻撃データを追加
            _runtimesAttacks.Add(new BossAttackRuntime()
            {
                bossAttackData = atk,
                lastUsedTime = -atk.cooldown,
            });
        }

        // ボス専用の初期化処理をここに追加
        ChangeState(new BossIdleState(this));
    }

    protected override void FixedUpdate()
    {
        float a = _enemyCommonData.shakeMagnitude;

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
            // ヒットストップ終了後の位置補正
            if (_shakeVec.sqrMagnitude >= 0.001f)
            {
                //ダメージを食らっていた敵だけ位置補正を行う
                if(!_isDamage)
                {
                    this.transform.position = _stopPos;
                }
            }

            _shakeVec = Vector3.zero;
        }

        if (_isStop) return;

        _isDamage = false; // ダメージフラグをリセット

        // 親クラスのUpdate()を呼び出す
        base.FixedUpdate();

        DebugLine();

        // フェード中の時に敵を削除するステートに遷移
        if (_playerState.GetNowHp() <= 0)
        {
            if (BaseSceneController.instance.isFadeing)
            {
                if (!(_currentState is BossDestoryState))
                {
                    ChangeState(new BossDestoryState(this));
                }
            }
        }


        if (_isPhase)
        {
            DirectionUpdate();
        }
    }



    // 攻撃タイプ1処理(オーバライド)
    public virtual void AttackType1(){}
    // 攻撃タイプ2処理(オーバライド)
    public virtual void AttackType2(){}
    // 攻撃タイプ3処理(オーバライド)
    public virtual void AttackType3() { }
    // 攻撃タイプ4処理(オーバライド)
    public virtual void AttackType4() { }
    // 攻撃タイプ5処理(オーバライド)
    public virtual void AttackType5() { }
    // 攻撃タイプ6処理(オーバライド)
    public virtual void AttackType6() { }

    public virtual void Stan()
    {
        // ボス専用のスタンする処理をここに追加
        // 一定時間動けなくするなど
    }

    // ボス専用のフェーズを変える処理をここに追加
    // 攻撃パターンの変更するフラグを立てるなど
    public virtual void PhaseChange() { }

    // モデルのリグを取得して攻撃判定を特定のボーンにアタッチする処理
    public void AttackReset()
    {
        _attackTimer = 0.0f;
    }

    // 攻撃判定に触れたときの処理
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            _isWallHit = true;
        }

        if (other.CompareTag("PlayerAttack"))
        {
            // 死んだ状態になっている場合はダメージを受けない
            if (_currentState is BossDeadState) return;
            // フェーズに入っている間は無敵時間
            if (_currentState is BossPhaseState) return;

            // ダメージを受ける(プレイヤーアタックのダメージを取得する)
            _currentHp -= other.GetComponent<PlayerAttack>().GetDamage();

            //ヒットストップ処理
            PlayerAttack playerAttack = other.GetComponent<PlayerAttack>();
            BattleManager manager = _battleManager.GetComponent<BattleManager>();
            manager.SetHitStop(playerAttack.GetHitStopTime());

            //ダメージフラグを立てる
            _isDamage = true;

            // ダメージエフェクトを生成する
            //Instantiate(_damageEffect, transform.position, Quaternion.identity);

            // Hpが0以下なら死亡処理に遷移
            if (_currentHp <= 0)
            {
                // すでに死亡状態なら変更しない
                if (!(_currentState is BossDeadState))
                {
                    ChangeState(new BossDeadState(this));
                }

                _currentHp = 0;
                _isDead = true;
            }

            // Hpが半分以下ならフェーズに入る
            if (_currentHp <= _maxHp * 0.5f && !_isPhase)
            {
                if (!(_currentState is BossPhaseState))
                {
                    ChangeState(new BossPhaseState(this));
                }
            }

            //攻撃はいったら攻撃判定を速攻消す
            Destroy(other.gameObject);

            if (_currentState is BossAttackType1State ||
                _currentState is BossAttackType2State ||
                _currentState is BossAttackType3State ||
                _currentState is BossAttackType4State ||
                _currentState is BossAttackType5State ||
                _currentState is BossAttackType6State) return;

            //ヒット処理
            OnHit();

        }

        if (other.CompareTag("PlayerRangedAttack"))
        {
            //死んだ状態になっている場合はダメージを受けない
            if (_currentState is BossDeadState) return;
            // フェーズに入っている間は無敵時間
            if (_currentState is BossPhaseState) return;

            // プレイヤーにダメージを与える処理
            _currentHp -= other.GetComponent<PlayerAttack>().GetDamage();

            //ヒットストップ処理
            PlayerAttack playerAttack = other.gameObject.GetComponent<PlayerAttack>();
            BattleManager manager = _battleManager.GetComponent<BattleManager>();
            manager.SetHitStop(playerAttack.GetHitStopTime());

            //ダメージフラグを立てる
            _isDamage = true;

            // Hpが0以下なら死亡処理に遷移
            if (_currentHp <= 0)
            {
                // すでに死亡状態なら変更しない
                if (!(_currentState is BossDeadState))
                {
                    ChangeState(new BossDeadState(this));
                }

                _currentHp = 0;
                _isDead = true;
            }

            // Hpが半分以下ならフェーズに入る
            if (_currentHp <= _maxHp * 0.5f && !_isPhase)
            {
                if (!(_currentState is BossPhaseState))
                {
                    ChangeState(new BossPhaseState(this));
                }
            }

            //攻撃はいったら攻撃判定を速攻消す
            Destroy(other.gameObject);

            _dropBullets.Add(_enemyCommonData.dropBulletTime);
            
            if (_currentState is BossAttackType1State ||
                _currentState is BossAttackType2State ||
                _currentState is BossAttackType3State ||
                _currentState is BossAttackType4State ||
                _currentState is BossAttackType5State ||
                _currentState is BossAttackType6State) return;

            //ヒット処理
            OnHit();

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            _isWallHit = false;
        }
    }

    //デバッグ用に線を引く
    public void DebugLine()
    {
        //プレイヤーとの位置差を表示
        Debug.DrawLine(transform.position, player.transform.position, Color.green);

        //敵の攻撃範囲を表示
        //Debug.DrawLine(transform.position, transform.position + transform.forward * Mathf.Sqrt(specialAttackRangeSqr), Color.red);

        //敵の検知範囲を球で表示
        Debug.DrawLine(transform.position, transform.position + transform.forward * Mathf.Sqrt(testRangeSqr), Color.blue);
    }

    // どの攻撃を行うか判別する処理
    public void AttackSelect()
    {
        // フェーズチェンジの確認
        CheckPhase();

        // その攻撃のステートに移動する
        Vector3 diff = player.transform.position - transform.position;

        // プレイヤーとの距離を計算
        testRange = diff.magnitude;

        // 距離の二乗を計算
        testRangeSqr = testRange * testRange;

        // 秒数
        float time = Time.time;

        // 使用可能な攻撃リスト
        List<BossAttackRuntime> available = new();

        // 現在のフェーズを取得
        var currentPhase = GetCurrentPhase();

        foreach (var atk in _runtimesAttacks)
        {
            // フェーズ条件を満たしているか確認
            if (atk.bossAttackData.bossPhase != currentPhase) continue;

            // 射程条件を満たしているか確認
            if (testRangeSqr > atk.bossAttackData.rangeSqr) continue;

            // クールダウン中か確認
            if (time - atk.lastUsedTime < atk.bossAttackData.cooldown) continue;

            // 使用可能な攻撃リストに追加
            available.Add(atk);
        }

        // 全ての攻撃が出来なかった場合、待機状態に戻る
        if (available.Count == 0) return;

        // 攻撃条件が当てはまっているデータ達でランダムに選ぶ
        var selected = ChooseWeightedRandom(available);

        // 選ばれた攻撃データの最後に使った時間を更新
        selected.lastUsedTime = time;

        // 次の状態を作成
        var next = CreateState(selected.bossAttackData.bossAttackType);

        // 攻撃状態に遷移
        ChangeState(next);
    }

    private void CheckPhase()
    {
        // Hpが半分以下ならフェーズに入る
        if (!_isPhase && _currentHp <= _maxHp * 0.5f)
        {
            _isPhase = true;
        }
    }

    private BossPhase GetCurrentPhase()
    {
        // 条件に応じてフェーズを返す
        return _isPhase ? BossPhase.Phase2 : BossPhase.Phase1;
    }


    BossAttackRuntime ChooseWeightedRandom(List<BossAttackRuntime> list)
    {
        // 合計値の変数
        float total = 0;

        // 全攻撃データの重さを計算
        foreach (var atk in list) total += atk.bossAttackData.weight;

        // ランダム値の計算
        float random = Random.value * total;

        // 重さに応じて攻撃データを選択
        foreach (var atk in list)
        {
            // 重さで判定
            if (random < atk.bossAttackData.weight) return atk;

            // 重さを引く
            random -= atk.bossAttackData.weight;
        }

        return null;
    }

    // クラス名で状態クラスを見つける関数
    private IState CreateState(BossAttackType type)
    {
        // 攻撃タイプに応じて状態クラスを生成して返す
        switch (type)
        {
            case BossAttackType.Attack1:
                return new BossAttackType1State(this);
            case BossAttackType.Attack2:
                return new BossAttackType2State(this);
            case BossAttackType.Attack3:
                return new BossAttackType3State(this);
            case BossAttackType.Attack4:
                return new BossAttackType4State(this);
            case BossAttackType.Attack5:
                return new BossAttackType5State(this);
            case BossAttackType.Attack6:
                return new BossAttackType6State(this);
        }

        return null;
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
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // ボスの各攻撃範囲を球で表示
        if (_runtimesAttacks != null)
        {
            foreach (var atk in _runtimesAttacks)
            {
                Gizmos.color = Color.cyan;
                float detectRadius = atk.bossAttackData != null ? Mathf.Sqrt(atk.bossAttackData.rangeSqr) : 0f;
                Gizmos.DrawWireSphere(transform.position, detectRadius);
            }
        }
    }

    protected virtual void DirectionUpdate()
    {
        //ここにボス個別のエフェクト演出とか入れる
        //例：フェーズが切り替わったら、エフェクトをまとわせる
        //例：チャンツーの手に呪術回線みたいなエフェクトをまとわせる

    }

}

// 
public class BossAttackRuntime
{
    // ボスの攻撃情報
    public BossAttack bossAttackData;
    // 最後に使った時間
    public float lastUsedTime;
}


