using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;

public class PlayerState : Player<PlayerState>
{
    // 状態のenum
    public enum StateKind
    {
        IDLE,           // 待機
        MOVE,           // 移動
        DODGE,          // 回避
        MELEEATTACK,    // 近接攻撃
        RANGEDATTACK,   // 遠距離攻撃
        CHARGE,         // チャージ
        CHARGEATTACK,   // チャージ攻撃
        SPECIALATTACK,  // 特殊攻撃
        DAMAGE,         // ダメージ
        DEAD            // 死亡
    }

    public enum DamageKind
    {
        LOW,
        MIDDLE,
        HIGH
    }

    public struct PlayerSkill
    {
        public LowAttackSkillData lowAttackSkillData;

        public ChargeAttackSkillData chargeAttackSkillData;

        public DashSkillData dashSkillData;

        public RangedAttackSkillData rangedAttackSkillData;

        public SpecialChargeSkillData specialChargeSkill;

        public List<PassiveSkillData> passiveSkillDataList;
    }

    private struct PassiveGameObject
    {
        public PassiveSkillData.GameObjectPopTiming popTiming;
        public GameObject gameObject;
    }

    private struct PassiveStatus
    {
        // パッシブスキルによるステータス上昇値

        // 最大体力上昇値
        public int maxHpAddNum;
        // 攻撃力上昇値
        public float attackPowerAddRate;
        // 移動速度上昇率(%)
        public float moveSpeedAddRate;
        // 回避可能回数増加数(回)
        public int dashCountAddNum;
        // 被ダメージ軽減率(%)
        public float damageCutRateAddRate;
        // 回避率上昇率(%)
        public int dodgeRateAddRate;

        // パッシブスキルで出すゲームオブジェクト
        public List<PassiveGameObject> passiveGameObjects;
    }

    private class ScallingAttackPart
    {
        public AttackPart.AttackPartKind attackPartKind;
        public GameObject attackObj;
        public float scale;
        public Vector3 defaultPos;
        public Vector3 currentPos;
    }

    // プレイヤーのステータス
    [SerializeField] private PlayerStatus _playerStatus;

    // 攻撃データ
    [SerializeField] private AttackData _attackData;

    // プレイヤーが使用する定数データ
    [SerializeField] private PlayerData _playerData;

    // プレイヤーの被弾時のデータ
    [SerializeField] private DamageData _damageData;

    // プレイヤーのエフェクトデータ
    [SerializeField] private PlayerEffectData _playerEffectData;

    // プレイヤーのStateごとのデータ
    [SerializeField] private PlayerStateDataList _playerStateDataList;

    // バトルマネージャー
    [SerializeField] private BattleManager _battleManager;

    // カメラ
    [SerializeField] private GameObject _camera;

    // 現在持っているスキル
    PlayerSkill _playerSkill;

    // パッシブスキルによるステータス
    PassiveStatus _passiveStatus;

    // 一つ前のパッシブスキルのステータス上昇値
    PassiveStatus _lastPassiveStatus;

    // デバッグ用：特殊攻撃のチャージ量
 //   [Range(0.0f,100.0f)]public float DEBUG_SpecialAttackGauge;

    // 入力情報
    private GameInputs _input;

    // 現在の状態
    private StateKind _stateKind;

    // プレイヤーの体力
    private int _nowHp;

    // 現在持っている弾の数
    private int _nowBulletNum;

    // 移動入力
    private Vector2 _moveInput;

    // 現在向いている方向
    private Vector3 _currentDirection;

    // リギッドボディ
    private Rigidbody _rigidbody;

    // 現在回避できるかどうか
    private bool _isAbleToDodge;

    // 現在攻撃できるかどうか
    private bool _isAbleToAttack;

    // 現在特殊攻撃ができるかどうか
    private bool _isAbleToSpecialAttack;

    // 攻撃入力がされたかどうか
    private bool _isAttackInput;

    // 回避入力がされたか
    private bool _isDodgeInput;

    // 攻撃ボタンを長押ししている時間
    private int _normalChargeTime;

    // 特殊攻撃のチャージ量
    private float _specialChargeNum;

    // アニメーター
    private Animator _animator;

    // 出している攻撃オブジェクト
    private GameObject _currentAttack;

    // 受けたダメージの種類
    private DamageKind _damageKind;

    // 攻撃を前から受けたかどうか
    private bool _isFrontDamage;

    // アイテム取得できる範囲にいるか
    private bool _isInItemRange;

    // 動きを止めるフラグ
    private bool _isStop;

    // アニメーションの速度を保存しておく
    private float _animationSpeed;

    // プレイヤーメッシュレンダラー
    private SkinnedMeshRenderer _playerMeshRenderer;

    // プレイヤーのマテリアル
    private Material _playerMaterial;

    // 攻撃時に拡大している攻撃部位
    private List<ScallingAttackPart> _scallingAttackParts = new List<ScallingAttackPart>();

    // エラーをとるためのダミー変数
    private ScallingAttackPart _errorDeleterPart;

    void Start()
    {
        // Animatorコンポーネントを取得
        _animator = GetComponent<Animator>();

        // Rigidbodyコンポーネントを取得
        _rigidbody = GetComponent<Rigidbody>();

        // PlayerInputコンポーネントを取得
        _input = new GameInputs();

        // InputActionの設定
        _input.Player.Move.started += Move;
        _input.Player.Move.performed += Move;
        _input.Player.Move.canceled += Move;
        _input.Player.Dodge.performed += Dodge;
        _input.Player.MeleeAttack.started += LowAttack;
        _input.Player.ChargeAttack.started += NormalCharge;
        _input.Player.ChargeAttack.canceled += ChargeAttack;
        _input.Player.SpecialAttack.started += SpecialAttack;

        // InputActionを有効化
        _input.Enable();

        // 初期状態は待機状態に設定
        ChangeState(new IdleState(this));

        // 体力を最大体力に設定
        _nowHp = _playerStatus.maxHp;

        // 弾の数を最大弾数に設定
        _nowBulletNum = _playerStatus.maxBulletNum;

        // プレイヤーメッシュレンダラーを取得
        _playerMeshRenderer = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();

        // プレイヤーのマテリアルを取得
        _playerMaterial = _playerMeshRenderer.material;

        // ダミー変数の初期化
        _errorDeleterPart = new ScallingAttackPart();
    }

    // 各Stateクラス

    // 待機状態
    public class IdleState : StateBase<PlayerState>
    {
        public IdleState(PlayerState next) : base(next)
        {

        }
        public override void OnEnterState()
        {
            // Stateの情報を設定
            state.SetStateKind(PlayerState.StateKind.IDLE);

            // 待機アニメーションを再生
            state._animator.SetBool("Idle", true);

            // 移動ベクトルをリセット
            state._rigidbody.velocity = Vector3.zero;

            // 現在の向きを保存
            state._currentDirection = state.transform.forward;
        }
        public override void OnUpdate()
        {
            if (state._isStop) return;
            //if (state.DEBUG_SpecialAttackGauge > 0)
            //{
            //    state._specialChargeNum = state.DEBUG_SpecialAttackGauge;
            //}

            // 移動ベクトルをリセット
            state._rigidbody.velocity = Vector3.zero;

            // 攻撃入力があれば近接攻撃状態に遷移
            if (state._isAttackInput)
            {
                state._isAttackInput = false;
                state.ChangeState(new MeleeAttackState(state));
                return;
            }

            // 移動入力があれば移動状態に遷移
            float magnitude = state._moveInput.magnitude;
            if (magnitude > state._playerData.moveInputLength)
            {
                state.ChangeState(new MoveState(state));
                return;
            }
        }
        public override void OnExitState()
        {
            // 待機アニメーションを停止
            state._animator.SetBool("Idle", false);
        }
    }

    // 移動状態
    public class MoveState : StateBase<PlayerState>
    {
        int _stateTime;
        public MoveState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // Stateの情報を設定
            state.SetStateKind(PlayerState.StateKind.MOVE);
            // 移動アニメーションを再生
            state._animator.SetBool("Move", true);
            // 移動サウンドを再生
            AudioManager.Instance.PlaySE(SoundID.PlayerMove);

        }
        public override void OnUpdate()
        {
            // 移動をリセットする
            state._rigidbody.velocity = Vector3.zero;

            if (state._isStop) return;

            _stateTime++;

            // 移動時のサウンド再生
            if (_stateTime % state._playerData.moveSoundInterval == 0)
            {
                AudioManager.Instance.PlaySE(SoundID.PlayerMove);
            }

            // 移動方向の計算
            Vector3 direction = new Vector3(state._moveInput.x, 0, state._moveInput.y).normalized;
            Vector3 moveDirection = state.CalculateMoveDirection(direction);

            // 移動ベクトル
            Vector3 moveVelocity = moveDirection * state._playerStatus.moveSpeed;

            // パッシブスキルによる移動速度上昇率を加算
            moveVelocity *= (1.0f + state._passiveStatus.moveSpeedAddRate / 100.0f);

            // 向きの更新
            if (moveDirection != Vector3.zero)
            {
                // 向きを徐々に変える
                state.transform.forward = Vector3.Slerp(state.transform.forward, moveDirection, state._playerData.rotateAngle * Time.fixedDeltaTime);
                // 現在の向きを保存
                state._currentDirection = state.transform.forward;
            }

            // リジッドボディの速度を設定
            state._rigidbody.velocity = moveVelocity;

            // 攻撃入力があれば近接攻撃状態に遷移
            if (state._isAttackInput)
            {
                state._isAttackInput = false;
                state.ChangeState(new MeleeAttackState(state));
                return;
            }
            // 移動入力がなければ待機状態に遷移
            float magnitude = state._moveInput.magnitude;
            if (magnitude <= state._playerData.moveInputLength)
            {
                state.ChangeState(new IdleState(state));
            }
        }
        public override void OnExitState()
        {
            // 移動アニメーションを停止
            state._animator.SetBool("Move", false);

            // 移動ベクトルをリセット
            state._rigidbody.velocity = Vector3.zero;
        }
    }

    // 回避状態
    public class DodgeState : StateBase<PlayerState>
    {
        // 回避時間のカウント
        private int _dodgeTime;

        // 回避方向
        private Vector3 _dodgeDirection;

        // 追随するエフェクトオブジェクト
        private GameObject _dashEffect;

        // 開始時のエフェクト
        private GameObject _startDashEffect;

        // 回避速度
        private float _dodgeSpeed;

        public DodgeState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // Stateの情報を設定
            state.SetStateKind(PlayerState.StateKind.DODGE);
            // 回避アニメーションを再生
            state._animator.SetTrigger("Dodge");
            // 回避時間を設定
            _dodgeTime = 0;

            // 移動方向がない場合は現在の向きを使用
            if (state._moveInput.magnitude < state._playerData.moveInputLength)
            {
                _dodgeDirection = state._currentDirection;
            }
            else
            {
                _dodgeDirection = new Vector3(state._moveInput.x, 0, state._moveInput.y).normalized;
                _dodgeDirection = state.CalculateMoveDirection(_dodgeDirection);
            }

            if (state._playerEffectData.dashEffectPrefab != null)
            {
                _dashEffect = GameObject.Instantiate(state._playerEffectData.dashEffectPrefab, state.transform.position, Quaternion.identity);
                // 向いている方向に回転させる
                _dashEffect.transform.forward = _dodgeDirection;
                _dashEffect.transform.SetParent(state.transform);
            }

            // 回避開始時のエフェクトを再生する
            if (state._playerEffectData.startDashEffectPrefab != null)
            {
                _startDashEffect = GameObject.Instantiate(state._playerEffectData.startDashEffectPrefab, state.transform.position, Quaternion.identity);
            }

            _dodgeSpeed = state._playerStatus.dodgeSpeed;
            _dodgeSpeed *= (1.0f + state._passiveStatus.moveSpeedAddRate / 100.0f);

            // 効果音を再生する
            AudioManager.Instance.PlaySE(SoundID.Dash);
        }
        public override void OnUpdate()
        {
            // 移動をリセットする
            state._rigidbody.velocity = Vector3.zero;

            if (state._isStop) return;
            // 回避時間をカウント
            _dodgeTime++;

            // 部位が拡大していたら少しずつ縮小する
            for (int i = 0; i < state._scallingAttackParts.Count; i++)
            {
                ScallingAttackPart part = state._scallingAttackParts[i];

                if (part.scale > 1.0f)
                {
                    float scale = part.scale;

                    scale -= state._playerData.chargeAttackPartScaleDownRatePerFrame;

                    scale = Mathf.Max(scale, 1.0f);

                    // 保存しているスケールを更新
                    state._scallingAttackParts[i].scale = scale;

                    // 攻撃する部位を縮小する
                    part.attackObj.transform.localScale = new Vector3(scale, scale, scale);

                    // 拡大している部位を元の座標に戻す
                    part.attackObj.transform.localPosition = part.defaultPos;
                }
            }

            // 回避方向に向ける
            if (_dodgeDirection != Vector3.zero)
            {
                state.transform.forward = _dodgeDirection;
            }

            // 最初の数フレームは移動しない
            if (_dodgeTime < state._playerData.dodgeStartTime)
            {
                state._rigidbody.velocity = Vector3.zero;
                return;
            }

            // 最初の数フレームは回避入力を無効化
            if (_dodgeTime == state._playerData.cancelDodgeCooldown)
            {
                state._isDodgeInput = false;
            }

            // エフェクトの位置と方向を更新
            if (_dashEffect != null)
            {
                _dashEffect.transform.position = state.transform.position;

                _dashEffect.transform.forward = _dodgeDirection;
            }

            // 一定時間経過したら速度を減速させる
            if (_dodgeTime >= state._playerData.dodgeTime - state._playerData.dodgeStopTime)
            {
                int leftTime = state._playerData.dodgeTime - _dodgeTime;
                float speed = state._playerStatus.dodgeSpeed * ((float)leftTime / (float)state._playerData.dodgeStopTime);

                _dodgeSpeed = speed;

                // クランプ
                _dodgeSpeed = Mathf.Max(_dodgeSpeed, state._playerStatus.moveSpeed);

                // 回避入力があればこの時点でもう一度回避状態に遷移
                if (state._isDodgeInput && state._dodgeCount < state._passiveStatus.dashCountAddNum)
                {
                    state._dodgeCount++;
                    state._isDodgeInput = false;
                    state.ChangeState(new DodgeState(state));
                    return;
                }
            }

            // 移動処理
            Vector3 dodgeVelocity = _dodgeDirection * _dodgeSpeed;

            state._rigidbody.velocity = dodgeVelocity;

            // 一定時間経過したら待機状態に遷移
            if (_dodgeTime >= state._playerData.dodgeTime)
            {
                // 回避入力があればもう一度回避状態に遷移
                if (state._isDodgeInput && state._dodgeCount < state._passiveStatus.dashCountAddNum)
                {
                    state._dodgeCount++;
                    state._isDodgeInput = false;
                    state.ChangeState(new DodgeState(state));
                    return;
                }

                // 回避のクールタイムを設定
                state._dodgeCoolTime = state._playerData.dodgeCoolTime;

                // 移動入力があれば移動状態に遷移、なければ待機状態に遷移
                float magnitude = state._moveInput.magnitude;

                // 回避の回数をリセット
                state._dodgeCount = 0;

                if (magnitude > state._playerData.moveInputLength)
                {
                    state.ChangeState(new MoveState(state));
                    return;
                }
                else
                {
                    state.ChangeState(new IdleState(state));
                    return;
                }
            }
        }
        public override void OnExitState()
        {
            // 回避アニメーションを停止
            state._animator.ResetTrigger("Dodge");
            // 移動ベクトルをリセット
            state._rigidbody.velocity = Vector3.zero;

            // エフェクトを削除
            if (_dashEffect != null)
            {
                GameObject.Destroy(_dashEffect);
            }
            if (_startDashEffect != null)
            {
                GameObject.Destroy(_startDashEffect);
            }


            // 拡大している攻撃部位があれば元に戻す
            for (int i = 0; i < state._scallingAttackParts.Count; i++)
            {
                ScallingAttackPart part = state._scallingAttackParts[i];
                if (part.attackObj)
                {
                    part.attackObj.transform.localScale = Vector3.one;
                    part.scale = 1.0f;
                }
            }
        }
    }

    // 近接攻撃状態
    public class MeleeAttackState : StateBase<PlayerState>
    {
        // 現在の攻撃名
        private string _currentAttackName;

        // 現在の攻撃データ
        private Attack _currentAttackData;

        // 現在のフレーム数
        private int _currentFrame;

        // 攻撃エフェクトオブジェクト
        private GameObject _effectObject;

        // 攻撃を出したかどうか
        bool _isAttack;
        public MeleeAttackState(PlayerState next) : base(next)
        {

        }
        public override void OnEnterState()
        {
            // Stateの情報を設定
            state.SetStateKind(PlayerState.StateKind.MELEEATTACK);

            // 攻撃する方向をジョイスティックの方向に設定
            if (state._moveInput.magnitude > state._playerData.moveInputLength)
            {
                Vector3 attackDirection = new Vector3(state._moveInput.x, 0, state._moveInput.y).normalized;
                attackDirection = state.CalculateMoveDirection(attackDirection);
                state.transform.forward = attackDirection;
                state._currentDirection = attackDirection;
            }

            // 最初の攻撃はLow1に設定
            _currentAttackName = "Low1";

            _isAttack = false;

            // アニメーションを再生
            state._animator.SetTrigger(_currentAttackName);

            // 攻撃の情報を設定
            _currentAttackData = state.SearchAttackData(_currentAttackName);

            // 攻撃する部位にエフェクトを出す
            Vector3 effectPos = state.GetAttackPosition(_currentAttackData.attackPartKind);

            // 攻撃する部位の情報を設定
            state.SetScallingAttackPart(_currentAttackData.scaleAttackParts);

            // 座標を少しプレイヤーから離す
            Vector3 shiftVec = (effectPos - state.transform.position).normalized;
            effectPos += shiftVec * 0.5f;

            _effectObject = Instantiate(_currentAttackData.attackEffect, effectPos, Quaternion.identity);

            GameObject target = state.SearchTargetObject();

            // ターゲットがいる場合はターゲットの方向に向ける
            if (target != null)
            {
                Vector3 targetDir = (target.transform.position - state.transform.position).normalized;
                targetDir.y = 0;
                state.transform.forward = targetDir;
                state._currentDirection = targetDir;
            }

            _currentFrame = 0;
        }
        public override void OnUpdate()
        {
            // 移動をリセットする
            state._rigidbody.velocity = Vector3.zero;

            if (state._isStop) return;

            // フレーム数をカウント
            _currentFrame++;

            // エフェクトの位置を更新
            if (_effectObject)
            {
                Vector3 effectPos = state.GetAttackPosition(_currentAttackData.attackPartKind);
                // 座標を少しプレイヤーから離す
                Vector3 shiftVec = (effectPos - state.transform.position).normalized;
                // Y座標を計算に入れない
                shiftVec.y = 0;

                effectPos += shiftVec * _currentAttackData.effectShiftScale;
                _effectObject.transform.position = effectPos;
            }

            // 攻撃するフレームに達したら攻撃オブジェクトを生成
            if (_currentFrame >= _currentAttackData.startFrame && !_isAttack)
            {
                state.CreateAttack(_currentAttackData);
                _isAttack = true;
            }

            // 攻撃を出した後
            if (_currentFrame >= _currentAttackData.startFrame)
            {
                // 攻撃オブジェクトが存在するなら攻撃オブジェクトの座標を更新
                if (state._currentAttack)
                {
                    Vector3 position = state.GetAttackPosition(_currentAttackData.attackPartKind);

                    // ずらす分を加算
                    Vector3 shiftVec = state.transform.forward * _currentAttackData.effectShiftScale;
                    // Y座標を計算に入れない
                    shiftVec.y = 0;
                    position += shiftVec;

                    state._currentAttack.transform.position = position;
                }

                // 移動ベクトルをリセット
                state._rigidbody.velocity = Vector3.zero;

                // 攻撃の入力を受け付ける
                state._isAbleToAttack = true;

                /// 攻撃部位の縮小処理 ///

                for (int i = 0; i < _currentAttackData.scaleAttackParts.Count; i++)
                {
                    // このループで変更する拡大部位
                    PlayerState.ScallingAttackPart part = state._errorDeleterPart;

                    // 拡大している攻撃部位を検索
                    for (int j = 0; j < state._scallingAttackParts.Count; j++)
                    {
                        // 見つかったら保存してループを抜ける
                        if (state._scallingAttackParts[j].attackPartKind == _currentAttackData.scaleAttackParts[i].attackPartKind)
                        {
                            part = state._scallingAttackParts[j];
                            break;
                        }
                    }

                    // 大きさの計算
                    float scale = Mathf.Lerp(_currentAttackData.scaleAttackParts[i].scale, 1.0f, Mathf.Clamp((float)(_currentFrame - _currentAttackData.startFrame) / (float)(_currentAttackData.cancelFrame - _currentAttackData.startFrame), 0.0f, 1.0f));
                    part.scale = scale;

                    // 攻撃する部位を小さくする
                    part.attackObj.transform.localScale = new Vector3(scale, scale, scale);

                    // 少しずつずらす
                    float shiftScale = Mathf.Lerp(_currentAttackData.scaleAttackParts[i].range, 1.0f, Mathf.Clamp((float)(_currentFrame - _currentAttackData.startFrame) / (float)(_currentAttackData.cancelFrame - _currentAttackData.startFrame), 0.0f, 1.0f));

                    // 攻撃座標の位置をずらす
                    part.attackObj.transform.localPosition = part.defaultPos * shiftScale;
                }

                // 硬直フレームの間は回避不可
                if (_currentFrame <= _currentAttackData.stunFrame)
                {
                    //   state._isAbleToDodge = false;
                }
                else
                {
                    state._isAbleToDodge = true;
                }
            }
            // 攻撃を出す前
            else
            {
                // 攻撃の入力を無効化
                state._isAbleToAttack = false;

                // 向いている方向に進む
                Vector3 attackVelocity = state._currentDirection * _currentAttackData.moveSpeed;
                state._rigidbody.velocity = attackVelocity;


                /// 攻撃部位の拡大処理 ///

                for (int i = 0; i < _currentAttackData.scaleAttackParts.Count; i++)
                {
                    // このループで変更する拡大部位
                    PlayerState.ScallingAttackPart part = state._errorDeleterPart;

                    // 拡大している攻撃部位を検索
                    for (int j = 0; j < state._scallingAttackParts.Count; j++)
                    {
                        // 見つかったら保存してループを抜ける
                        if (state._scallingAttackParts[j].attackPartKind == _currentAttackData.scaleAttackParts[i].attackPartKind)
                        {
                            part = state._scallingAttackParts[j];
                            break;
                        }
                    }

                    // 大きさの計算
                    float scale = Mathf.Lerp(1.0f, _currentAttackData.scaleAttackParts[i].scale, Mathf.Clamp((float)_currentFrame / (float)_currentAttackData.startFrame, 0.0f, 1.0f));
                    part.scale = scale;

                    // 攻撃する部位を大きくする
                    part.attackObj.transform.localScale = new Vector3(scale, scale, scale);

                    // 少しずつずらす
                    float shiftScale = Mathf.Lerp(1.0f, _currentAttackData.scaleAttackParts[i].range, Mathf.Clamp((float)_currentFrame / (float)_currentAttackData.startFrame, 0.0f, 1.0f));

                    // 攻撃座標の位置をずらす
                    part.attackObj.transform.localPosition = part.defaultPos * shiftScale;
                }
            }

            // 攻撃キャンセルフレームに達した時に攻撃入力があれば次の攻撃に遷移
            if (_currentFrame >= _currentAttackData.cancelFrame)
            {
                // 攻撃入力があれば次の攻撃に遷移
                if (state._isAttackInput)
                {
                    // 攻撃入力をリセット
                    state._isAttackInput = false;
                    // 次の攻撃データを取得
                    string nextAttackName = _currentAttackData.nextAttackName;
                    Attack nextAttackData = state.SearchAttackData(nextAttackName);

                    // 現在の攻撃オブジェクトを削除
                    if (state._currentAttack)
                    {
                        state._battleManager.RemovePlayerAttack(state._currentAttack);

                        Destroy(state._currentAttack);
                        state._currentAttack = null;
                    }

                    GameObject target = state.SearchTargetObject();

                    // ターゲットがいる場合はターゲットの方向に向ける
                    if (target != null)
                    {
                        Vector3 targetDir = (target.transform.position - state.transform.position).normalized;
                        targetDir.y = 0;
                        state.transform.forward = targetDir;
                        state._currentDirection = targetDir;
                    }

                    // 次の攻撃データが存在する場合、次の攻撃に遷移
                    if (nextAttackData != null)
                    {
                        _currentAttackName = nextAttackName;
                        _currentAttackData = nextAttackData;
                        _currentFrame = 0;

                        // 既存のエフェクトを削除
                        if (_effectObject)
                        {
                            Destroy(_effectObject);
                            _effectObject = null;
                        }

                        // 攻撃する部位にエフェクトを出す
                        Vector3 effectPos = state.GetAttackPosition(_currentAttackData.attackPartKind);

                        for (int i = 0; i < state._scallingAttackParts.Count; i++)
                        {
                            PlayerState.ScallingAttackPart part = state._scallingAttackParts[i];

                            if (part.attackObj)
                            {
                                // 攻撃する部位の大きさを元に戻す
                                part.attackObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                                // 位置を元に戻す
                                part.attackObj.transform.localPosition = part.defaultPos;

                            }
                        }
                        // 攻撃する部位の情報を設定
                        state.SetScallingAttackPart(_currentAttackData.scaleAttackParts);
                        _effectObject = Instantiate(_currentAttackData.attackEffect, effectPos, Quaternion.identity);

                        // 次の攻撃アニメーションを再生
                        state._animator.SetTrigger(_currentAttackName);

                        // 攻撃オブジェクトを生成するフラグをリセット
                        _isAttack = false;

                    }
                }
            }


            // 攻撃のトータルフレームに達したらアイドル状態に遷移
            if (_currentFrame >= _currentAttackData.totalFrame)
            {
                // 既存のエフェクトを削除
                if (_effectObject)
                {
                    Destroy(_effectObject);
                    _effectObject = null;
                }



                for (int i = 0; i < state._scallingAttackParts.Count; i++)
                {
                    PlayerState.ScallingAttackPart part = state._scallingAttackParts[i];

                    if (part.attackObj)
                    {
                        // 攻撃する部位の大きさを元に戻す
                        part.attackObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                        // 位置を元に戻す
                        part.attackObj.transform.localPosition = part.defaultPos;

                    }
                }

                state.ChangeState(new IdleState(state));
            }
        }
        public override void OnExitState()
        {
            // 攻撃アニメーションを停止
            state._animator.ResetTrigger(_currentAttackData.attackName);

            // チャージ時間をリセット
            state._normalChargeTime = 0;

            // 既存のエフェクトを削除
            if (_effectObject)
            {
                Destroy(_effectObject);
                _effectObject = null;
            }
        }
    }

    // 遠距離攻撃状態
    public class RangedAttackState : StateBase<PlayerState>
    {

        int _currentFrame;

        GameObject _target;

        Attack _currentAttackData;

        public RangedAttackState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // Stateの情報を設定
            state.SetStateKind(PlayerState.StateKind.RANGEDATTACK);
            // 攻撃する方向をジョイスティックの方向に設定
            if (state._moveInput.magnitude > state._playerData.moveInputLength)
            {
                Vector3 attackDirection = new Vector3(state._moveInput.x, 0, state._moveInput.y).normalized;
                attackDirection = state.CalculateMoveDirection(attackDirection);
                state.transform.forward = attackDirection;
                state._currentDirection = attackDirection;
            }
            else
            {
                // 移動入力がない場合は現在の向きを使用
                state.transform.forward = state._currentDirection;
            }

            // 前方向にいる敵を探す
            _target = state.SearchTargetObject();

            // 遠距離攻撃アニメーションを再生
            state._animator.SetTrigger("RangedAttack");
            // 攻撃の情報を設定
            _currentAttackData = state.SearchAttackData("RangedAttack");
            // フレームカウントをリセット
            _currentFrame = 0;
        }

        public override void OnUpdate()
        {
            // 移動をリセットする
            state._rigidbody.velocity = Vector3.zero;

            if (state._isStop) return;

            _currentFrame++;

            // 移動ベクトルをリセット
            state._rigidbody.velocity = Vector3.zero;


            // 攻撃するフレームに達したら攻撃オブジェクトを生成
            if (_currentFrame == _currentAttackData.startFrame)
            {
                GameObject bullet = state.CreateRangedAttack(_currentAttackData);

                state._nowBulletNum--;

                // 弾のスクリプトにターゲットを設定
                PlayerRangedAttack rangedAttack = bullet.GetComponent<PlayerRangedAttack>();

                // ターゲットがいる場合
                if (_target)
                {
                    rangedAttack.SetTarget(_target);

                    Debug.Log("ターゲットとの距離" + (_target.transform.position - state.transform.position).magnitude);
                }

                // 弾の向きと攻撃データを設定
                rangedAttack.SetCurrentDir(state._currentDirection);
            }


            // 攻撃のトータルフレームに達したらアイドル状態に遷移
            if (_currentFrame >= _currentAttackData.totalFrame)
            {
                // 移動入力があれば移動状態に遷移、なければ待機状態に遷移
                float magnitude = state._moveInput.magnitude;
                if (magnitude > state._playerData.moveInputLength)
                {
                    state.ChangeState(new MoveState(state));
                    return;
                }
                else
                {
                    state.ChangeState(new IdleState(state));
                    return;
                }
            }
        }

        public override void OnExitState()
        {
            state._animator.ResetTrigger("RangedAttack");
        }
    }

    // チャージ状態
    public class ChargeState : StateBase<PlayerState>
    {
        int _stateTime;

        bool _isShowAttackRange = false;

        bool _isHighChargeAttack = false;

       // GameObject _attackArea;

        float _attackScale;

        float _lowChargeAttackScale;

        GameObject _chargeEffect;

        Attack _currentAttackData;

        public ChargeState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // Stateの情報を設定
            state.SetStateKind(PlayerState.StateKind.CHARGE);

            // 通常チャージアニメーションを再生
            state._animator.SetTrigger("NormalCharge");

            // 攻撃情報を設定
            _currentAttackData = state.SearchAttackData("LowChargeAttack");

            // 攻撃範囲のスケールを弱チャージ攻撃のものに設定
            _attackScale = _currentAttackData.scale;

            // 攻撃を行う部位を設定
            state.SetScallingAttackPart(_currentAttackData.scaleAttackParts);

            _stateTime = 0;

            // 効果音を再生する
            AudioManager.Instance.PlaySE(SoundID.Charge);

            // チャージエフェクトをだす座標を計算
            Vector3 effectPos = state.transform.position;
            Vector3 toCameraVec = (state._camera.transform.position - state.transform.position).normalized;
            effectPos += toCameraVec * state._playerData.chargeEffectShiftScale;

            // チャージエフェクトを出す
            _chargeEffect = Instantiate(state._playerEffectData.chargeEffectPrefab, effectPos, Quaternion.identity);
        }
        public override void OnUpdate()
        {
            // 移動をリセットする
            state._rigidbody.velocity = Vector3.zero;

            if (state._isStop) return;

            _stateTime++;

            state._normalChargeTime++;

            // 攻撃を出すことができる時に
            if (state._normalChargeTime > state._playerData.chargeAttackTime)
            {
                // 攻撃範囲を表示していない場合、攻撃範囲を表示
                if (!_isShowAttackRange)
                {
                    Vector3 AreaPos = state.transform.position;

                    //　ずらす分のベクトル
                    Vector3 shift = state._playerData.chargeAttackAreaShiftVector;
                    // プレイヤーの向きに合わせてずらす分を回転させる
                    shift = Quaternion.Euler(0, state.transform.eulerAngles.y, 0) * shift;

                    AreaPos += shift;

                    //_attackArea = Instantiate(state._attackData.chargeAttackAreaGameObject, AreaPos, Quaternion.identity);

                    //_attackArea.transform.localScale = new Vector3(_attackScale, _attackArea.transform.localScale.y, _attackScale);

                    _isShowAttackRange = true;
                }

                // チャージ時間が50％以上なら強チャージ攻撃の攻撃範囲に変更
                if (state._normalChargeTime >= state._playerData.maxChargeAttackTime / 2)
                {
                    // 強チャージ攻撃の攻撃範囲に変更
                    if (!_isHighChargeAttack)
                    {
                        _attackScale = state.SearchAttackData("ChargeAttack").scale;

                        //_attackArea.transform.localScale = new Vector3(_attackScale, _attackArea.transform.localScale.y, _attackScale);

                        _isHighChargeAttack = true;

                        _currentAttackData = state.SearchAttackData("ChargeAttack");

                        // 攻撃する部位を変更
                        state.SetScallingAttackPart(_currentAttackData.scaleAttackParts);
                    }

                    /// 攻撃部位の拡大処理 ///

                    // 強チャージ攻撃に入ってから何フレームたったか
                    float frame = (float)(state._normalChargeTime - state._playerData.maxChargeAttackTime / 2);

                    for (int i = 0; i < _currentAttackData.scaleAttackParts.Count; i++)
                    {
                        PlayerState.ScallingAttackPart part = state._errorDeleterPart;

                        // 拡大している攻撃部位を検索
                        for (int j = 0; j < state._scallingAttackParts.Count; j++)
                        {
                            // 見つかったら保存してループを抜ける
                            if (state._scallingAttackParts[j].attackPartKind == _currentAttackData.scaleAttackParts[i].attackPartKind)
                            {
                                part = state._scallingAttackParts[j];
                                break;
                            }
                        }


                        // 大きさの計算

                        // 目標の大きさ
                        float targetScale = _currentAttackData.scaleAttackParts[i].scale - _lowChargeAttackScale;

                        float scale = Mathf.Lerp(1.0f, targetScale, Mathf.Clamp(frame / (float)state._playerData.chargeAttackPartScaleUpTime, 0.0f, 1.0f));

                        // 攻撃する部位を大きくする
                        part.scale = scale + _lowChargeAttackScale;
                        part.attackObj.transform.localScale = new Vector3(scale + _lowChargeAttackScale, scale + _lowChargeAttackScale, scale + _lowChargeAttackScale);

                        // 少しずつずらす
                        float shiftScale = Mathf.Lerp(1.0f, _currentAttackData.scaleAttackParts[i].range, Mathf.Clamp(frame / (float)state._playerData.chargeAttackPartScaleUpTime, 0.0f, 1.0f));

                        // 攻撃座標の位置をずらす
                        part.attackObj.transform.localPosition = part.defaultPos * shiftScale;
                    }
                }
                // 弱チャージ攻撃の攻撃範囲の時
                else
                {
                    /// 攻撃部位の拡大処理 ///

                    // 弱チャージ攻撃に入ってから何フレームたったか
                    float frame = (float)(state._normalChargeTime - state._playerData.chargeAttackTime);

                    for (int i = 0; i < _currentAttackData.scaleAttackParts.Count; i++)
                    {
                        // このループで変更する拡大部位
                        PlayerState.ScallingAttackPart part = state._errorDeleterPart;

                        // 拡大している攻撃部位を検索
                        for (int j = 0; j < state._scallingAttackParts.Count; j++)
                        {
                            // 見つかったら保存してループを抜ける
                            if (state._scallingAttackParts[j].attackPartKind == _currentAttackData.scaleAttackParts[i].attackPartKind)
                            {
                                part = state._scallingAttackParts[j];
                                break;
                            }
                        }

                        // 大きさの計算
                        float scale = Mathf.Lerp(1.0f, _currentAttackData.scaleAttackParts[i].scale, Mathf.Clamp(frame / (float)state._playerData.chargeAttackPartScaleUpTime, 0.0f, 1.0f));

                        part.scale = scale;

                        _lowChargeAttackScale = scale;

                        // 攻撃する部位を大きくする
                        part.attackObj.transform.localScale = new Vector3(scale, scale, scale);

                        // 少しずつずらす
                        float shiftScale = Mathf.Lerp(1.0f, _currentAttackData.scaleAttackParts[i].range, Mathf.Clamp(frame / (float)state._playerData.chargeAttackPartScaleUpTime, 0.0f, 1.0f));

                        // 攻撃座標の位置をずらす
                        part.attackObj.transform.localPosition = part.defaultPos * shiftScale;
                    }
                }

                // 最大チャージ時間を超えたらチャージ攻撃に遷移
                if (state._normalChargeTime >= state._playerData.maxChargeAttackTime)
                {
                    state._normalChargeTime = state._playerData.maxChargeAttackTime;
                    state.ChangeState(new ChargeAttackState(state));
                    return;
                }

            }
        }
        public override void OnExitState()
        {
            state._animator.ResetTrigger("NormalCharge");

            // 攻撃範囲オブジェクトを削除
            //if (_attackArea)
            //{
            //    Destroy(_attackArea);
            //    _attackArea = null;
            //}

            // チャージエフェクトを削除
            if (_chargeEffect)
            {
                Destroy(_chargeEffect);
                _chargeEffect = null;
            }

            // 効果音を停止する
            AudioManager.Instance.StopSE(SoundID.Charge);
        }
    }

    // チャージ攻撃状態
    public class ChargeAttackState : StateBase<PlayerState>
    {
        private string _currentAttackName;
        private Attack _currentAttackData;
        private int _currentFrame;
        public ChargeAttackState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // Stateの情報を設定
            state.SetStateKind(PlayerState.StateKind.CHARGEATTACK);

            // チャージ時間が50％以上なら強チャージ攻撃、そうでなければ弱チャージ攻撃に設定
            if (state._normalChargeTime >= state._playerData.maxChargeAttackTime / 2)
            {
                _currentAttackName = "ChargeAttack";
            }
            else
            {
                _currentAttackName = "LowChargeAttack";
            }

            // アニメーションを再生
            state._animator.SetTrigger(_currentAttackName);
            // 攻撃の情報を設定
            _currentAttackData = state.SearchAttackData(_currentAttackName);

            _currentFrame = 0;
        }
        public override void OnUpdate()
        {
            // 移動をリセットする
            state._rigidbody.velocity = Vector3.zero;

            if (state._isStop) return;

            _currentFrame++;


            // 攻撃オブジェクトを生成するフレームに達したら攻撃オブジェクトを生成
            if (_currentFrame == _currentAttackData.startFrame)
            {
                state.CreateAttack(_currentAttackData);
                // チャージ時間をリセット
                state._normalChargeTime = 0;
            }
            // 攻撃を出している間
            else if (_currentFrame > _currentAttackData.startFrame &&
                _currentFrame <= _currentAttackData.stunFrame)
            {
                // 前に進む
                Vector3 attackVelocity = state._currentDirection * _currentAttackData.moveSpeed;
                state._rigidbody.velocity = attackVelocity;

                // 攻撃の座標を更新
                Vector3 attackPos = state.GetAttackPosition(_currentAttackData.attackPartKind);

                // ずらす分を加算
                Vector3 shiftVec = state.transform.forward * _currentAttackData.shiftPosZ;

                // Y座標を計算に入れない
                shiftVec.y = 0;
                attackPos += shiftVec;

                if (state._currentAttack)
                {
                    state._currentAttack.transform.position = attackPos;
                }
            }
            // 攻撃の硬直のあと
            else if (_currentFrame > _currentAttackData.stunFrame)
            {
                // 移動ベクトルをlerpで徐々に減速させる
                float speed = Mathf.Lerp(_currentAttackData.moveSpeed, 0.0f, ((float)_currentFrame - (float)_currentAttackData.stunFrame) / ((float)_currentAttackData.totalFrame - (float)_currentAttackData.stunFrame));

                state._rigidbody.velocity = state._currentDirection * speed;

                /// 攻撃部位の縮小処理 ///

                for (int i = 0; i < _currentAttackData.scaleAttackParts.Count; i++)
                {
                    // このループで変更する拡大部位
                    PlayerState.ScallingAttackPart part = state._errorDeleterPart;

                    // 拡大している攻撃部位を検索
                    for (int j = 0; j < state._scallingAttackParts.Count; j++)
                    {
                        // 見つかったら保存してループを抜ける
                        if (state._scallingAttackParts[j].attackPartKind == _currentAttackData.scaleAttackParts[i].attackPartKind)
                        {
                            part = state._scallingAttackParts[j];
                            break;
                        }
                    }

                    // 大きさの計算
                    float scale = Mathf.Lerp(_currentAttackData.scaleAttackParts[i].scale, 1.0f, Mathf.Clamp((float)(_currentFrame - _currentAttackData.stunFrame) / (float)(_currentAttackData.totalFrame - _currentAttackData.stunFrame), 0.0f, 1.0f));
                    part.scale = scale;

                    // 攻撃する部位を小さくする
                    part.attackObj.transform.localScale = new Vector3(scale, scale, scale);

                    // 少しずつずらす
                    float shiftScale = Mathf.Lerp(_currentAttackData.scaleAttackParts[i].range, 1.0f, Mathf.Clamp((float)(_currentFrame - _currentAttackData.stunFrame) / (float)(_currentAttackData.totalFrame - _currentAttackData.stunFrame), 0.0f, 1.0f));

                    // 攻撃座標の位置をずらす
                    part.attackObj.transform.localPosition = part.defaultPos * shiftScale;
                }
            }

            // 攻撃のキャンセルフレームに達したときに回避でキャンセルできるようにする
            if (_currentFrame >= _currentAttackData.cancelFrame)
            {
                // 回避可能にする
                state._isAbleToDodge = true;
            }

            // 攻撃のトータルフレームに達したら
            if (_currentFrame >= _currentAttackData.totalFrame)
            {
                // 移動入力があれば移動状態に遷移、なければ待機状態に遷移
                float magnitude = state._moveInput.magnitude;
                if (magnitude > state._playerData.moveInputLength)
                {
                    state.ChangeState(new MoveState(state));
                    return;
                }
                else
                {
                    state.ChangeState(new IdleState(state));
                    return;
                }
            }
        }
        public override void OnExitState()
        {
            // 攻撃アニメーションを停止
            state._animator.ResetTrigger(_currentAttackData.attackName);
            // チャージ時間をリセット
            state._normalChargeTime = 0;
            // 攻撃する部位の大きさを元に戻す

            for (int i = 0; i < state._scallingAttackParts.Count; i++)
            {
                if (state._scallingAttackParts[i].attackObj)
                {
                    state._scallingAttackParts[i].attackObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    // 位置を元に戻す
                    state._scallingAttackParts[i].attackObj.transform.localPosition = state._scallingAttackParts[i].defaultPos;
                }
            }
        }
    }

    // 特殊攻撃状態
    public class SpecialAttackState : StateBase<PlayerState>
    {
        private float _stateTime;

        private string _currentAttackName;

        private Attack _currentAttackData;

        private CameraMove _cameraMove;

        private GameObject _attackEffect;

        public SpecialAttackState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // Stateの情報を設定
            state.SetStateKind(PlayerState.StateKind.SPECIALATTACK);

            _stateTime = 0;

            int chargeLevel = state.GetSpecialChargeLevel();

            // チャージ時間に応じて攻撃名を設定
            _currentAttackName = "SpecialAttack" + chargeLevel.ToString();

            // 特殊攻撃アニメーションを再生
            state._animator.SetTrigger("SpecialAttack");

            // カメラの特殊攻撃中フラグを設定
            _cameraMove = state._camera.GetComponent<CameraMove>();
            _cameraMove.StartSpecialAttack(chargeLevel, state.GetAttackPart(AttackPart.AttackPartKind.SpecialAttackCameraPos));

            // 攻撃の情報を設定
            _currentAttackData = state.SearchAttackData(_currentAttackName);

            // 拡大する部位を設定
            state.SetScallingAttackPart(_currentAttackData.scaleAttackParts);
        }
        public override void OnUpdate()
        {
            // 移動をリセットする
            state._rigidbody.velocity = Vector3.zero;

            if (state._isStop) return;

            _stateTime++;

            // 移動ベクトルをリセット
            state._rigidbody.velocity = Vector3.zero;

            /// 攻撃部位の拡大処理 ///

            // 攻撃を出す前
            if (_stateTime <= _currentAttackData.startFrame)
            {
                for (int i = 0; i < _currentAttackData.scaleAttackParts.Count; i++)
                {
                    // このループで変更する拡大部位
                    PlayerState.ScallingAttackPart part = state._errorDeleterPart;

                    // 拡大している攻撃部位を検索
                    for (int j = 0; j < state._scallingAttackParts.Count; j++)
                    {
                        // 見つかったら保存してループを抜ける
                        if (state._scallingAttackParts[j].attackPartKind == _currentAttackData.scaleAttackParts[i].attackPartKind)
                        {
                            part = state._scallingAttackParts[j];
                            break;
                        }
                    }

                    // 大きさの計算
                    float scale = Mathf.Lerp(1.0f, _currentAttackData.scaleAttackParts[i].scale, Mathf.Clamp(_stateTime / (float)_currentAttackData.startFrame, 0.0f, 1.0f));

                    part.scale = scale;

                    // 攻撃する部位を大きくする
                    part.attackObj.transform.localScale = new Vector3(scale, scale, scale);

                    // 少しずつずらす
                    float shiftScale = Mathf.Lerp(1.0f, _currentAttackData.scaleAttackParts[i].range, Mathf.Clamp(_stateTime / (float)_currentAttackData.startFrame, 0.0f, 1.0f));

                    // 攻撃座標の位置をずらす
                    part.attackObj.transform.localPosition = part.defaultPos * shiftScale;
                }
            }
            // 攻撃を出した後
            else
            {
                for (int i = 0; i < _currentAttackData.scaleAttackParts.Count; i++)
                {
                    // このループで変更する拡大部位
                    PlayerState.ScallingAttackPart part = state._errorDeleterPart;
                    // 拡大している攻撃部位を検索
                    for (int j = 0; j < state._scallingAttackParts.Count; j++)
                    {
                        // 見つかったら保存してループを抜ける
                        if (state._scallingAttackParts[j].attackPartKind == _currentAttackData.scaleAttackParts[i].attackPartKind)
                        {
                            part = state._scallingAttackParts[j];
                            break;
                        }
                    }
                    // 大きさの計算
                    float scale = Mathf.Lerp(_currentAttackData.scaleAttackParts[i].scale, 1.0f, Mathf.Clamp((_stateTime - _currentAttackData.startFrame) / (float)(_currentAttackData.totalFrame - _currentAttackData.startFrame), 0.0f, 1.0f));
                    part.scale = scale;
                    // 攻撃する部位を小さくする
                    part.attackObj.transform.localScale = new Vector3(scale, scale, scale);
                    // 少しずつずらす
                    float shiftScale = Mathf.Lerp(_currentAttackData.scaleAttackParts[i].range, 1.0f, Mathf.Clamp((_stateTime - _currentAttackData.startFrame) / (float)(_currentAttackData.totalFrame - _currentAttackData.startFrame), 0.0f, 1.0f));
                    // 攻撃座標の位置をずらす
                    part.attackObj.transform.localPosition = part.defaultPos * shiftScale;
                }

            }

            // 攻撃オブジェクトを生成するフレームに達したら攻撃オブジェクトを生成
            if (_stateTime == _currentAttackData.startFrame)
            {
                state.CreateAttack(_currentAttackData);

                // 画面を揺らす
                _cameraMove.SetShakeData(_currentAttackData.cameraShakeKind);

                _attackEffect = Instantiate(state._playerEffectData.specialAttackEffectPrefab, state.transform.position, Quaternion.identity);

                // エフェクトの向きをプレイヤーの向きに合わせる
                _attackEffect.transform.forward = state.transform.forward;

                // エフェクトの位置を少し前にずらす
                Vector3 shiftVec = state.transform.forward * _currentAttackData.shiftPosZ;
                _attackEffect.transform.position += shiftVec;

                // エフェクトのサイズを攻撃データに合わせる
                float effectScale = _currentAttackData.scale;
                _attackEffect.transform.localScale = new Vector3(effectScale, effectScale, effectScale);

                // TODO : 弾を生成する(特殊攻撃を飛ばす仕様ならば)
            }
            // 攻撃のトータルフレームに達した時
            if (_stateTime >= _currentAttackData.totalFrame)
            {
                // 移動入力があれば移動状態に遷移、なければ待機状態に遷移
                float magnitude = state._moveInput.magnitude;
                if (magnitude > state._playerData.moveInputLength)
                {
                    state.ChangeState(new MoveState(state));
                    return;
                }
                else
                {
                    state.ChangeState(new IdleState(state));
                    return;
                }
            }
        }
        public override void OnExitState()
        {
            // 攻撃アニメーションを停止
            state._animator.ResetTrigger(_currentAttackName);
            // チャージ時間をリセット
            state._specialChargeNum = 0;

            if (_cameraMove != null)
            {
                // カメラの特殊攻撃中フラグを解除
                _cameraMove.EndSpecialAttack();
            }

            // エフェクトを削除
            if (_attackEffect != null)
            {
                Destroy(_attackEffect);
                _attackEffect = null;
            }
        }
    }

    // ダメージ状態
    public class DamageState : StateBase<PlayerState>
    {
        private int _stateTime;
        private int _stunDuration;
        private int _knockbackTime;
        private float _knockBackScale;
        private string _damageAnim;
        private bool _changeMaterial = false;

        public DamageState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // Stateの情報を設定
            state.SetStateKind(PlayerState.StateKind.DAMAGE);

            // ダメージの種類でスタン時間とアニメーションを変更

            // 小ダメージ
            if (state._damageKind == DamageKind.LOW)
            {
                _stunDuration = state._damageData.lowStanTime;
                _knockbackTime = 0;
                _knockBackScale = 0;
                state._lowDamageInvincibleTime = state._damageData.lowInvincibleTime;

                // 軽ダメージアニメーションを再生
                _damageAnim = "LowHit";
            }
            // 中ダメージ
            else if (state._damageKind == DamageKind.MIDDLE)
            {
                _stunDuration = state._damageData.middleStanTime;
                _knockbackTime = state._damageData.middleKnockBackTime;
                _knockBackScale = state._damageData.middleKnockBackScale;
                state._normalDamageInvincibleTime = state._damageData.middleInvincibleTime;

                // ダメージを前から受けたかどうかでアニメーションを変える
                if (state._isFrontDamage)
                {
                    _damageAnim = "FrontMiddleHit";
                }
                else
                {
                    _damageAnim = "BackMiddleHit";
                }
            }
            // 大ダメージ
            else if (state._damageKind == DamageKind.HIGH)
            {
                _stunDuration = state._damageData.highStanTime;
                _knockbackTime = state._damageData.highKnockBackTime;
                _knockBackScale = state._damageData.highKnockBackScale;
                state._normalDamageInvincibleTime = state._damageData.highInvincibleTime;

                // ダメージを前から受けたかどうかでアニメーションを変える
                if (state._isFrontDamage)
                {
                    _damageAnim = "FrontHeavyHit";
                }
                else
                {
                    _damageAnim = "BackHeavyHit";
                }
            }
            // 時間のカウントをリセット
            _stateTime = 0;

            // ダメージアニメーションを再生
            state._animator.SetTrigger(_damageAnim);

            // ヒットストップを行う
            int stopTime = 0;

            switch (state._damageKind)
            {
                case DamageKind.LOW:
                    stopTime = state._damageData.lowHitStop;
                    break;
                case DamageKind.MIDDLE:
                    stopTime = state._damageData.middleHitStop;
                    break;
                case DamageKind.HIGH:
                    stopTime = state._damageData.highHitStop;
                    break;
            }
            state._battleManager.GetComponent<BattleManager>().SetHitStop(stopTime);
        }
        public override void OnUpdate()
        {
            // 移動をリセットする
            state._rigidbody.velocity = Vector3.zero;

            if (state._isStop) return;

            // ヒットストップが終わってマテリアルが変更されたままだったら
            if (!_changeMaterial)
            {
                // マテリアルを元に戻す
                state._playerMeshRenderer.material.color = Color.white;

                _changeMaterial = true;
            }

            // 部位が拡大していたら少しずつ縮小する

            for (int i = 0; i < state._scallingAttackParts.Count; i++)
            {
                PlayerState.ScallingAttackPart part = state._scallingAttackParts[i];

                if (part.scale > 1.0f)
                {
                    float scale = part.scale;

                    scale -= state._playerData.chargeAttackPartScaleDownRatePerFrame;

                    scale = Mathf.Max(scale, 1.0f);

                    part.scale = scale;

                    // 攻撃する部位を縮小する
                    part.attackObj.transform.localScale = new Vector3(scale, scale, scale);

                    // 位置を元の座標に戻す
                    part.attackObj.transform.localPosition = part.defaultPos;
                }
            }
            // スタン時間をカウントダウン
            _stateTime++;

            // ノックバック時間内ならノックバックさせる
            if (_stateTime <= _knockbackTime)
            {
                // 前方向からの攻撃なら後ろにノックバック
                if (state._isFrontDamage)
                {
                    Vector3 knockbackVelocity = -state.transform.forward * _knockBackScale;
                    state._rigidbody.velocity = knockbackVelocity;
                }
                // 後ろからの攻撃なら前にノックバック
                else
                {
                    Vector3 knockbackVelocity = state.transform.forward * _knockBackScale;
                    state._rigidbody.velocity = knockbackVelocity;
                }
            }
            else
            {
                // ノックバック時間が終了したら移動ベクトルをリセット
                state._rigidbody.velocity = Vector3.zero;
            }


            // スタン時間が終了したら待機状態に遷移
            if (_stateTime >= _stunDuration)
            {
                // 移動入力があれば移動状態に遷移、なければ待機状態に遷移
                float magnitude = state._moveInput.magnitude;
                if (magnitude > state._playerData.moveInputLength)
                {
                    state.ChangeState(new MoveState(state));
                    return;
                }
                else
                {
                    state.ChangeState(new IdleState(state));
                    return;
                }
            }
        }
        public override void OnExitState()
        {
            // ダメージアニメーションを停止
            state._animator.ResetTrigger(_damageAnim);

            // 拡大している攻撃部位があれば元に戻す
            for (int i = 0; i < state._scallingAttackParts.Count; i++)
            {
                PlayerState.ScallingAttackPart part = state._scallingAttackParts[i];

                if (part.scale > 1.0f)
                {
                    part.attackObj.transform.localScale = Vector3.one;
                    part.scale = 1.0f;
                }
            }
        }
    }

    // 死亡状態
    public class DeadState : StateBase<PlayerState>
    {
        bool _changeMaterial = false;

        int _deadTime = 0;

        public DeadState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // Stateの情報を設定
            state.SetStateKind(PlayerState.StateKind.DEAD);
            // 死亡アニメーションを再生
            state._animator.SetTrigger("Dead");
            // 移動ベクトルをリセット
            state._rigidbody.velocity = Vector3.zero;

            // マテリアルを赤色に変更する
            state._playerMeshRenderer.material.color = Color.red;

            // 重めのヒットストップを行う
            state._battleManager.GetComponent<BattleManager>().SlowTime(state._playerData.deathSlowTime, state._playerData.deathTimeScale);

        }
        public override void OnUpdate()
        {
            // 移動をリセットする
            state._rigidbody.velocity = Vector3.zero;

            if (state._isStop) return;

            // スロー演出が終わってマテリアルが変更されたままだったら
            if (!_changeMaterial)
            {
                if (Time.timeScale == 1.0f)
                {
                    // マテリアルを元に戻す
                    state._playerMeshRenderer.material.color = Color.white;

                    _changeMaterial = true;
                }
            }
            _deadTime++;

            if (_deadTime == state._playerData.deathEffectTime)
            {
                // 死亡エフェクトを再生
                Instantiate(state._playerEffectData.deathEffectPrefab, state.transform.position, Quaternion.identity);

                //state._battleManager.StartFadeOut();
            }

            // 移動ベクトルをリセットし続ける
            state._rigidbody.velocity = Vector3.zero;
            AudioManager.Instance.StopBGM();
        }
        public override void OnExitState()
        {

        }
    }


    private void Move(InputAction.CallbackContext constext)
    {
        _moveInput = constext.ReadValue<Vector2>();
    }

    private void Dodge(InputAction.CallbackContext constext)
    {
        if (_stateKind == StateKind.DODGE)
        {
            _isDodgeInput = true;
        }

        if (_isInItemRange)
        {
            _isInItemRange = false;
            return;
        }

        if (_dodgeCoolTime > 0)
        {
            return;
        }

        if (_isAbleToDodge)
        {
            // チャージ時間をリセット
            _normalChargeTime = 0;

            // 回避状態に遷移
            ChangeState(new DodgeState(this));
        }
    }

    private void LowAttack(InputAction.CallbackContext constext)
    {
        if (_isAbleToAttack)
        {
            _isAttackInput = true;
        }
    }

    private void NormalCharge(InputAction.CallbackContext context)
    {
        if (_isAbleToAttack && _stateKind != StateKind.MELEEATTACK)
        {
            ChangeState(new ChargeState(this));
        }
    }

    private void ChargeAttack(InputAction.CallbackContext context)
    {

        // チャージ状態以外では何もしない
        if (_stateKind != StateKind.CHARGE) return;

        // 一定時間以上チャージを行っていたらチャージ攻撃に移行
        if (_normalChargeTime > _playerData.chargeAttackTime)
        {
            ChangeState(new ChargeAttackState(this));
        }
        // そうでなければ待機状態に戻る
        else
        {

            //チャージ時間をリセット
            _normalChargeTime = 0;

            // 移動入力があれば移動状態に遷移、なければ待機状態に遷移
            float magnitude = _moveInput.magnitude;
            if (magnitude > _playerData.moveInputLength)
            {
                ChangeState(new MoveState(this));
                return;
            }
            else
            {
                ChangeState(new IdleState(this));
                return;
            }
        }
    }

    private void RangedAttack(InputAction.CallbackContext context)
    {
        if (_isAbleToAttack && _nowBulletNum > 0)
        {
            ChangeState(new RangedAttackState(this));
        }
    }

    private void SpecialAttack(InputAction.CallbackContext context)
    {
        int chargeLevel = GetSpecialChargeLevel();

        if (_isAbleToSpecialAttack && chargeLevel > 0)
        {
            ChangeState(new SpecialAttackState(this));
        }
    }

    private Attack SearchAttackData(string attackName)
    {
        Attack result = null;

        if (attackName == "none") return result;

        for (int i = 0; i < _attackData.attackDataList.Count; i++)
        {
            if (_attackData.attackDataList[i].attackName == attackName)
            {
                result = _attackData.attackDataList[i];
                break;
            }
        }
        return result;
    }

    private GameObject SearchTargetObject()
    {
        GameObject result = null;
        // シーン内のEnemyを取得
        List<GameObject> enemies = _battleManager.enemies;

        // 探知範囲内にいる敵を取得
        List<GameObject> forwardEnemies = new List<GameObject>();

        // プレイヤーから前方の設定した角度内にいる敵を探す
        foreach (GameObject enemy in enemies)
        {
            Vector3 toEnemy = enemy.transform.position - transform.position;
            toEnemy.y = 0; // y成分を無視して水平面での方向を考える
            toEnemy.Normalize();
            float dot = Vector3.Dot(transform.forward, toEnemy);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg; // ラジアンを度に変換
            if (angle <= _playerData.forwardAngle)
            {
                forwardEnemies.Add(enemy);
            }
        }

        // 探知範囲内にいる敵の中で一番近い敵を取得
        float minDistance = Mathf.Infinity;
        foreach (GameObject enemy in forwardEnemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance && distance <= _playerData.forwardDistance)
            {
                minDistance = distance;
                result = enemy;
            }
        }
        return result;
    }
    private void SetScallingAttackPart(List<Attack.ScaleAttackPart> parts)
    {
        // 拡大している部位をすべて元に戻す
        for (int i = 0; i < _scallingAttackParts.Count; i++)
        {
            if (_scallingAttackParts[i].attackObj)
            {
                _scallingAttackParts[i].attackObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                // 位置を元に戻す
                _scallingAttackParts[i].attackObj.transform.localPosition = _scallingAttackParts[i].defaultPos;
            }
        }

        // 拡大する攻撃部位のリストをクリア
        _scallingAttackParts.Clear();

        // 拡大する攻撃部位を追加
        for (int i = 0; i < parts.Count; i++)
        {
            // 攻撃部位のゲームオブジェクトを取得
            PlayerState.ScallingAttackPart scallingPart = _errorDeleterPart;

            GameObject attackPart = GetAttackPart(parts[i].attackPartKind);
            if (attackPart != null)
            {
                scallingPart.attackObj = attackPart;
                scallingPart.attackPartKind = parts[i].attackPartKind;
                scallingPart.defaultPos = attackPart.transform.localPosition;
                scallingPart.scale = parts[i].scale;
                _scallingAttackParts.Add(scallingPart);
            }
        }
    }

    private GameObject GetAttackPart(AttackPart.AttackPartKind part)
    {
        GameObject result = null;

        // 攻撃部位のリグの名前を取得
        string rigName = _attackData.attackPartList.attackPartDataList[(int)part].objectRigName;

        // 早期リターン
        if (rigName == null) return result;

        // リグのTransformを取得
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform t in allChildren)
        {
            if (t.name == rigName) // モデルのボーン名
            {
                result = t.gameObject;
            }
        }

        return result;
    }

    private Vector3 GetAttackPosition(AttackPart.AttackPartKind part)
    {
        Vector3 result = Vector3.zero;

        // 攻撃部位のリグの名前を取得
        string rigName = _attackData.attackPartList.attackPartDataList[(int)part].objectRigName;

        Vector3 rigPos = Vector3.zero;

        // リグのTransformを取得
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform t in allChildren)
        {
            if (t.name == rigName) // モデルのボーン名
            {
                rigPos = t.position;
            }
        }

        result = rigPos;
        return result;
    }

    private GameObject CreateSkillAttack(GameObject skillAttack)
    {
        skillAttack.GetComponent<AttackEffect>().SetPos(transform.position);

        // ゲームオブジェクトを生成
        GameObject attack = Instantiate(skillAttack);

        // 座標を自身の位置に設定
        attack.transform.position = transform.position;

        PlayerAttack playerAttack = attack.GetComponent<PlayerAttack>();

        // カメラを設定
        playerAttack.SetCamera(_camera);

        // バトルマネージャーを設定
        playerAttack.SetBattleManager(_battleManager);

        // バトルマネージャーに攻撃オブジェクトを登録
        _battleManager.AddPlayerAttack(attack);

        return attack;
    }

    private void CreateAttack(Attack data)
    {
        // ゲームオブジェクトを生成
        GameObject attackObject = Instantiate(_attackData.meleeAttackGameObject);

        // 当たり判定のサイズ
        float scale = data.scale;

        // 攻撃のデータ
        PlayerAttack.PlayerAttackData attackData = new PlayerAttack.PlayerAttackData();

        // ダメージの値を設定
        attackData.damage = data.damage * _playerStatus.attackPower * (1 + _passiveStatus.attackPowerAddRate / 100.0f);

        // 攻撃の大きさを設定
        attackObject.transform.localScale = new Vector3(scale, scale, scale);

        // 攻撃の生存時間を設定
        attackData.attackLifeTime = data.attackLifeTime;

        // カメラを揺らす種類を設定
        attackData.shakeKind = data.cameraShakeKind;

        // ヒットエフェクトを設定
        attackData.hitEffect = data.hitEffect;

        // ヒットストップ時間を設定
        attackData.hitStopFrame = data.hitStopFrame;

        // 攻撃がぶつかっても消えないかどうかを設定
        attackData.isHitDelete = false;

        // 効果音を設定
        attackData.hitSoundID = data.hitSoundID;
        attackData.missSoundID = data.missSoundID;

        // もし弱攻撃ならば
        if (data.attackKind == Attack.AttackType.WeakAttack)
        {
            // 弱攻撃フラグを立てる
            attackData.isWeakAttack = true;
        }

        PlayerAttack playerAttack = attackObject.GetComponent<PlayerAttack>();

        // カメラを設定
        playerAttack.SetCamera(_camera);

        // 攻撃の情報を設定
        playerAttack.SetPlayerAttackData(attackData);

        // 攻撃の位置を設定
        Vector3 position = GetAttackPosition(data.attackPartKind);

        // 攻撃の座標を設定
        playerAttack.SetPlayerPos(position);

        // バトルマネージャーを設定
        playerAttack.SetBattleManager(_battleManager);

        // ずらす分を加算
        Vector3 shiftVec = transform.forward * data.shiftPosZ;
        attackObject.transform.position = position + shiftVec;

        // 攻撃の向きを設定
        attackObject.transform.forward = transform.forward;

        // 攻撃を出すときの効果音があれば再生
        if (data.attackSoundID != SoundID.None)
        {
            AudioManager.Instance.PlaySE(data.attackSoundID);
        }

        // 攻撃オブジェクトをバトルマネージャーに登録
        _battleManager.AddPlayerAttack(attackObject);

        // 攻撃オブジェクトを保存
        _currentAttack = attackObject;
    }

    /// <summary>
    /// 遠距離攻撃オブジェクトを生成するときに使用
    /// </summary>
    /// <param name="attack">攻撃オブジェクト</param>
    private GameObject CreateRangedAttack(Attack data)
    {
        GameObject attack = _attackData.rangedAttackGameObject;

        PlayerRangedAttack.RangedAttackData attackData = new PlayerRangedAttack.RangedAttackData();

        // 攻撃の座標を設定
        Vector3 position = GetAttackPosition(data.attackPartKind);
        attack.transform.position = position;
        // 攻撃の向きを設定
        attack.transform.forward = transform.forward;

        // 攻撃の情報を設定

        // スキルがある場合
        if (_playerSkill.rangedAttackSkillData != null)
        {
            // ダメージにスキルのダメージ加算率を加算
            attackData.damage = data.damage + (data.damage * (_playerSkill.rangedAttackSkillData.damageAddRate / 100));
            // 弾速を変更
            attackData.speedRate = _playerSkill.rangedAttackSkillData.speedRate;
            // デバフを追加
            attackData.debuffType = _playerSkill.rangedAttackSkillData.debuffType;
            // 追撃を追加
            attackData.chaseAttack = _playerSkill.rangedAttackSkillData.chaseAttack;
        }
        // ない場合は通常の攻撃のデータをそのまま使用
        else
        {
            attackData.damage = data.damage;
            attackData.speedRate = 1.0f;
        }

        // ステータスの値を設定
        attackData.damage = (int)((float)attackData.damage * _playerStatus.attackPower);

        // 弾のヒットストップ時間を設定
        attackData.hitStopTime = data.hitStopFrame;

        // 攻撃の情報をオブジェクトに入れる
        GameObject bullet = Instantiate(attack);

        bullet.GetComponent<PlayerRangedAttack>().SetRangedAttackData(attackData);

        return bullet;

    }

    private Vector3 CalculateMoveDirection(Vector3 direction)
    {
        Vector3 moveDirection = direction;

        // 固定値分回転させる
        moveDirection = Quaternion.Euler(0, _playerData.moveDirAngle, 0) * moveDirection;

        return moveDirection;
    }
    public void AddSpecialGauge(float addNum)
    {
        _specialChargeNum += addNum;
    }

    public int GetMaxHp()
    {
        return _playerStatus.maxHp + _passiveStatus.maxHpAddNum;
    }
    public int GetNowHp()
    {
        return _nowHp;
    }
    public int GetMaxBulletNum()
    {
        return _playerStatus.maxBulletNum;
    }
    public int GetNowBulletNum()
    {
        return _nowBulletNum;
    }
    public float GetMaxSpecialChargeNum()
    {
        return _playerData.maxSpecialChargeNum;
    }
    public float GetNowSpecialChargeNum()
    {
        return _specialChargeNum;
    }
    public void SetLowAttackSkill(LowAttackSkillData skill)
    {
        _playerSkill.lowAttackSkillData = skill;
    }
    public void SetChargeAttackSkill(ChargeAttackSkillData skill)
    {
        _playerSkill.chargeAttackSkillData = skill;
    }
    public void SetRangedAttackSkill(RangedAttackSkillData skill)
    {
        _playerSkill.rangedAttackSkillData = skill;
    }
    public void SetSpecialChargeSkill(SpecialChargeSkillData skill)
    {
        _playerSkill.specialChargeSkill = skill;
    }
    public void SetDashSkill(DashSkillData skill)
    {
        _playerSkill.dashSkillData = skill;
    }

    public void SetStop(bool flag)
    {
        _isStop = flag;
    }

    private void SetStateKind(PlayerState.StateKind stateKind)
    {
        _stateKind = stateKind;

        // 状態ごとの情報を設定
        _isAbleToAttack = _playerStateDataList.StateDataList[(int)stateKind].ableToAttack;
        _isAbleToDodge = _playerStateDataList.StateDataList[(int)stateKind].ableToDodge;
        _isAbleToSpecialAttack = _playerStateDataList.StateDataList[(int)stateKind].ableToSpecialAttack;
    }


    public void SetStateUpdateFlag(bool flag)
    {
        Debug.Log("SetStateUpdateFlag: " + flag);
        _isStopStateUpdate = flag;
    }

    public void StopAnimation()
    {
        if (_animator.speed > 0)
        {
            _animationSpeed = _animator.speed;
        }
        _animator.speed = 0;
    }
    public void StartAnimation()
    {
        _animator.speed = _animationSpeed;
    }

    public void OnMoveStage()
    {
        // 球数を最大まで回復
        _nowBulletNum = GetMaxBulletNum();
    }

    public int GetSpecialChargeLevel()
    {
        float chargeNum = _specialChargeNum;

        float subNum = _playerData.maxSpecialChargeNum / _playerData.specialAttackMaxLevel;

        int count = 0;

        while (chargeNum >= subNum)
        {
            chargeNum -= subNum;
            count++;
        }

        return count;
    }

    public void SetPassiveSkills(List<PassiveSkillData> passiveSkillDatas)
    {
        _playerSkill.passiveSkillDataList = passiveSkillDatas;

        _passiveStatus = new PassiveStatus();
        _passiveStatus.passiveGameObjects = new List<PassiveGameObject>();

        // パッシブスキルの効果を適用
        foreach (PassiveSkillData skill in _playerSkill.passiveSkillDataList)
        {
            if (skill.upStatuses != null)
            {
                // ステータスに効果を加算
                foreach (PassiveSkillData.UpStatus status in skill.upStatuses)
                {
                    switch (status.statusKind)
                    {
                        // 最大体力
                        case PassiveSkillData.PassiveStatusKind.MaxHp:
                            _passiveStatus.maxHpAddNum += (int)status.addNum;
                            break;
                        // 攻撃力
                        case PassiveSkillData.PassiveStatusKind.AttackPower:
                            _passiveStatus.attackPowerAddRate += status.addNum;
                            break;
                        // 被ダメージカット率
                        case PassiveSkillData.PassiveStatusKind.DamageCutRate:
                            _passiveStatus.damageCutRateAddRate += status.addNum;
                            break;
                        // ダッシュ回数
                        case PassiveSkillData.PassiveStatusKind.DashCount:
                            _passiveStatus.dashCountAddNum += (int)status.addNum;
                            break;
                        // 移動速度
                        case PassiveSkillData.PassiveStatusKind.MoveSpeed:
                            _passiveStatus.moveSpeedAddRate += status.addNum;
                            break;
                        // 回避率
                        case PassiveSkillData.PassiveStatusKind.DodgeRate:
                            _passiveStatus.dodgeRateAddRate += (int)status.addNum;
                            break;
                    }
                }
            }

            if (skill.PassiveObjects != null)
            {
                // ゲームオブジェクトを出すパッシブスキルを適用
                foreach (PassiveSkillData.PassiveObject obj in skill.PassiveObjects)
                {
                    PassiveGameObject passiveObj = new PassiveGameObject();
                    passiveObj.popTiming = obj.popTiming;
                    passiveObj.gameObject = obj.gameObject;

                    _passiveStatus.passiveGameObjects.Add(passiveObj);
                }
            }
        }

        // 体力の最大値が増えていたらその分体力を回復する
        if (_passiveStatus.maxHpAddNum > _lastPassiveStatus.maxHpAddNum)
        {
            _nowHp = _nowHp + (_passiveStatus.maxHpAddNum - _lastPassiveStatus.maxHpAddNum);
        }

        _lastPassiveStatus = _passiveStatus;

    }
    void OnTriggerEnter(Collider other)
    {
        // 敵の攻撃に当たったらダメージ状態に遷移
        if (other.gameObject.CompareTag("EnemyAttack") ||
            other.gameObject.CompareTag("EnemyRangedAttack"))
        {
            // 無敵時間がある間は処理を行わない
            if (_normalDamageInvincibleTime > 0) return;
            // 死亡時はダメージを受けない
            if (_stateKind == StateKind.DEAD) return;
            // 回避中はダメージを受けない
            if (_stateKind == StateKind.DODGE) return;
            // 被弾中はダメージを受けない
            if (_stateKind == StateKind.DAMAGE) return;
            // 特殊攻撃発動中はダメージを受けない
            if (_stateKind == StateKind.SPECIALATTACK) return;

            // ダメージの種類が弱ならば何もしない
            if (other.gameObject.GetComponent<EnemyAttackCol>().GetDamageKind() == DamageKind.LOW) return;


            // 回避率を計算して回避できたらダメージを受けない
            int randNum = UnityEngine.Random.Range(0, 100);
            if (randNum < _passiveStatus.dodgeRateAddRate)
            {
                foreach (PassiveGameObject obj in _passiveStatus.passiveGameObjects)
                {
                    if (obj.popTiming == PassiveSkillData.GameObjectPopTiming.Dodge)
                    {
                        Instantiate(obj.gameObject, transform.position, Quaternion.identity);
                    }
                }
                return;
            }

            // 攻撃を前から受けたかどうか
            Vector3 toEnemy = other.transform.position - transform.position;
            toEnemy.y = 0; // y成分を無視して水平面での方向を考える
            toEnemy.Normalize();
            float dot = Vector3.Dot(transform.forward, toEnemy);

            // ダメージの種類を取得
            _damageKind = other.gameObject.GetComponent<EnemyAttackCol>().GetDamageKind();

            if (dot > 0)
            {
                _isFrontDamage = true;

                // もし攻撃がHeavyなら攻撃の方向を向く
                if (_damageKind == DamageKind.HIGH)
                {
                    transform.forward = toEnemy;
                    _currentDirection = toEnemy;
                }
            }
            else
            {
                _isFrontDamage = false;

                // もし攻撃がHeavyなら攻撃の方向と逆を向く
                if (_damageKind == DamageKind.HIGH)
                {
                    transform.forward = -toEnemy;
                    _currentDirection = -toEnemy;
                }
            }

            _battleManager.RemoveEnemyAttack(other.gameObject);

            // 攻撃オブジェクトを削除する
            Destroy(other.gameObject);

            int damage = (int)other.gameObject.GetComponent<EnemyAttackCol>().GetDamage();

            // 被ダメージカット率を計算してダメージを減らす
            damage = (int)(damage * (1.0f - (_passiveStatus.damageCutRateAddRate / 100.0f)));

            // HPを減らす
            _nowHp -= damage;

            // ダメージの一定割合を特殊ゲージに加算
            float specialGaugeAddNum = damage * _playerData.specialAttackChargeRate;

            AddSpecialGauge(specialGaugeAddNum);

            // ダメージマテリアルに変更
            _playerMeshRenderer.material.color = Color.red;

            // 当たり判定と攻撃の当たり判定が重なった位置にエフェクトを生成
            Vector3 hitPosition = other.ClosestPoint(transform.position);

            // ダメージの種類に応じたサウンドを鳴らす
            switch (_damageKind)
            {
                case DamageKind.HIGH:
                    AudioManager.Instance.PlaySE(SoundID.HighAttackHit);
                    break;
                case DamageKind.MIDDLE:
                    AudioManager.Instance.PlaySE(SoundID.MiddleAttackHit);
                    break;
            }

            // ダメージエフェクトを生成
            Instantiate(other.gameObject.GetComponent<EnemyAttackCol>().GetHitEffectPrefab(), hitPosition, Quaternion.identity);

            // 攻撃入力をリセット
            _isAttackInput = false;

            // 通常攻撃のチャージ時間をリセット
            _normalChargeTime = 0;

            // HPが0以下なら死亡状態に遷移
            if (_nowHp <= 0)
            {
                _nowHp = 0;
                ChangeState(new DeadState(this));
                return;
            }
            else
            {
                // ダメージ状態に遷移
                ChangeState(new DamageState(this));
            }

        }

        //// ドロップした弾にあたったら弾を補充
        //if (other.gameObject.CompareTag("DropBullet"))
        //{
        //    // 弾を補充
        //    _nowBulletNum++;

        //    // アイテムを親ごと消す
        //    Destroy(other.transform.parent.gameObject);
        //}

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("EnemyAttack") ||
            other.CompareTag("EnemyRangedAttack"))
        {
            // 無敵時間がある間は処理を行わない
            if (_normalDamageInvincibleTime > 0) return;
            // 弱攻撃の無敵時間がある場合も処理を行わない
            if (_lowDamageInvincibleTime > 0) return;

            // 弱攻撃の処理を行う
            EnemyAttackCol enemyAttackCol = other.GetComponent<EnemyAttackCol>();
            if (enemyAttackCol.GetDamageKind() == DamageKind.LOW)
            {
                // 死亡時はダメージを受けない
                if (_stateKind == StateKind.DEAD) return;
                // 回避中はダメージを受けない
                if (_stateKind == StateKind.DODGE) return;
                // 被弾中はダメージを受けない
                if (_stateKind == StateKind.DAMAGE) return;
                // 特殊攻撃発動中はダメージを受けない
                if (_stateKind == StateKind.SPECIALATTACK) return;

                // 回避率を計算して回避できたらダメージを受けない
                int randNum = UnityEngine.Random.Range(0, 100);
                if (randNum < _passiveStatus.dodgeRateAddRate)
                {
                    foreach (PassiveGameObject obj in _passiveStatus.passiveGameObjects)
                    {
                        if (obj.popTiming == PassiveSkillData.GameObjectPopTiming.Dodge)
                        {
                            Instantiate(obj.gameObject, transform.position, Quaternion.identity);
                        }
                    }

                    return;
                }

                int damage = (int)other.gameObject.GetComponent<EnemyAttackCol>().GetDamage();

                // 被ダメージカット率を計算してダメージを減らす
                damage = (int)(damage * (1.0f - (_passiveStatus.damageCutRateAddRate / 100.0f)));

                // HPを減らす
                _nowHp -= damage;

                // 無敵時間を設定
                _lowDamageInvincibleTime = _damageData.lowInvincibleTime;

                // ダメージの一定割合を特殊ゲージに加算
                float specialGaugeAddNum = damage * _playerData.specialAttackChargeRate;

                AddSpecialGauge(specialGaugeAddNum);

                // ダメージマテリアルに変更
                _playerMeshRenderer.material.color = Color.red;

                // ダメージの種類を取得
                _damageKind = other.gameObject.GetComponent<EnemyAttackCol>().GetDamageKind();

                // 当たり判定と攻撃の当たり判定が重なった位置にエフェクトを生成
                Vector3 hitPosition = other.ClosestPoint(transform.position);

                // ダメージサウンドを鳴らす
                AudioManager.Instance.PlaySE(SoundID.LowAttackHit);
                // ダメージエフェクトを生成
                Instantiate(other.gameObject.GetComponent<EnemyAttackCol>().GetHitEffectPrefab(), hitPosition, Quaternion.identity);

                // 攻撃入力をリセット
                _isAttackInput = false;

                // 通常攻撃のチャージ時間をリセット
                _normalChargeTime = 0;

                // HPが0以下なら死亡状態に遷移
                if (_nowHp <= 0)
                {
                    _nowHp = 0;
                    ChangeState(new DeadState(this));
                    return;
                }
            }
        }
    }
}

