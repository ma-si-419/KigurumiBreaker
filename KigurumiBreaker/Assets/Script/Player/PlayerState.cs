using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


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
        DEAD,           // 死亡
        CINEMATIC,      // 演出中
    }

    public enum DamageKind
    {
        LOW,
        MIDDLE,
        HEAVY
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

    // 攻撃データ
    [SerializeField] private AttackDataList _attackData;

    // 攻撃オブジェクトの基本データ
    [SerializeField] private GameObject _attackObjectPrefab;

    // プレイヤーが使用する定数データ
    [SerializeField] private PlayerData _playerData;

    // プレイヤーの攻撃部位
    [SerializeField] private AttackPartList _attackPartData;

    // プレイヤーの被弾時のデータ
    [SerializeField] private DamageData _damageData;

    // デフォルトの遠距離攻撃の弾
    [SerializeField] private GameObject _bulletPrefab;

    // バトルマネージャー
    [SerializeField] private BattleManager _battleManager;

    // 現在持っているスキル
    PlayerSkill _playerSkill;

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
    private bool isAbleToSpecialAttack;

    // 攻撃入力がされたかどうか
    private bool _isAttackInput;

    // 攻撃ボタンを長押ししている時間
    private int _normalChargeTime;

    // 特殊攻撃のチャージをしている場合trueにする
    bool _isSpecialCharge;

    // 特殊攻撃のチャージ時間
    private float _specialChargeTime;

    // アニメーター
    private Animator _animator;

    // 出している攻撃オブジェクト
    private GameObject _currentAttack;

    // 受けたダメージの種類
    private DamageKind _damageKind;

    // 攻撃を前から受けたかどうか
    private bool _isFrontDamage;


    // Start is called before the first frame update
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
        _input.Player.RangedAttack.started += RangedAttack;
        _input.Player.ChargeAttack.started += NormalCharge;
        _input.Player.ChargeAttack.canceled += ChargeAttack;
        _input.Player.SpecialAttack.started += SpecialCharge;
        _input.Player.SpecialAttack.started += SpecialAttack;

        // InputActionを有効化
        _input.Enable();

        // 初期状態は待機状態に設定
        ChangeState(new IdleState(this));

        // 体力を最大体力に設定
        _nowHp = _playerData.maxHp;

        // 弾の数を最大弾数に設定
        _nowBulletNum = _playerData.maxBulletNum;
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
            // 待機状態では回避可能
            state._isAbleToDodge = true;
            // 待機状態では攻撃可能
            state._isAbleToAttack = true;
            // 待機状態では特殊攻撃可能
            state.isAbleToSpecialAttack = true;

            // 状態を待機に設定
            state._stateKind = StateKind.IDLE;

            // 待機アニメーションを再生
            state._animator.SetBool("Idle", true);

            // 移動ベクトルをリセット
            state._rigidbody.velocity = Vector3.zero;

            // 現在の向きを保存
            state._currentDirection = state.transform.forward;


        }
        public override void OnUpdate()
        {
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
        public MoveState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // 移動状態では回避可能
            state._isAbleToDodge = true;
            // 移動状態では攻撃可能
            state._isAbleToAttack = true;
            // 移動状態では特殊攻撃可能
            state.isAbleToSpecialAttack = true;
            // 状態を移動に設定
            state._stateKind = StateKind.MOVE;
            // 移動アニメーションを再生
            state._animator.SetBool("Move", true);
        }
        public override void OnUpdate()
        {
            // 移動方向の計算
            Vector3 direction = new Vector3(state._moveInput.x, 0, state._moveInput.y).normalized;
            Vector3 moveDirection = state.CalculateMoveDirection(direction);

            // 移動ベクトル
            Vector3 moveVelocity = moveDirection * state._playerData.moveSpeed;

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
        int dodgeTime;

        // 回避方向
        Vector3 dodgeDirection;

        public DodgeState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // 回避中は回避不可
            state._isAbleToDodge = false;
            // 回避中は攻撃不可
            state._isAbleToAttack = false;
            // 回避中は特殊攻撃不可
            state.isAbleToSpecialAttack = false;
            // 状態を回避に設定
            state._stateKind = StateKind.DODGE;
            // 回避アニメーションを再生
            state._animator.SetTrigger("Dodge");
            // 回避時間を設定
            dodgeTime = 0;

            // 移動方向の計算

            // 移動方向がない場合は現在の向きを使用
            if (state._moveInput.magnitude < state._playerData.moveInputLength)
            {
                dodgeDirection = state._currentDirection;
            }
            else
            {
                dodgeDirection = new Vector3(state._moveInput.x, 0, state._moveInput.y).normalized;
                dodgeDirection = state.CalculateMoveDirection(dodgeDirection);
            }

            if (state._playerSkill.dashSkillData.startAttack != null)
            {
                // ダッシュ開始時に出すスキルを出す
                Instantiate(state._playerSkill.dashSkillData.startAttack, state.transform.position, Quaternion.identity);
            }

        }
        public override void OnUpdate()
        {
            // 回避時間をカウント
            dodgeTime++;

            // 回避方向に向ける
            if (dodgeDirection != Vector3.zero)
            {
                state.transform.forward = dodgeDirection;
            }

            // 最初の数フレームは移動しない
            if (dodgeTime < state._playerData.dodgeStartTime)
            {
                state._rigidbody.velocity = Vector3.zero;
                return;
            }

            // 移動処理
            Vector3 dodgeVelocity = dodgeDirection * state._playerData.dodgeSpeed;
            state._rigidbody.velocity = dodgeVelocity;

            // スキル処理
            if (state._playerSkill.dashSkillData != null)
            {
                // 回避中に出すスキルを出す
                if (state._playerSkill.dashSkillData.onDashAttack != null)
                {
                    Instantiate(state._playerSkill.dashSkillData.onDashAttack, state.transform.position, Quaternion.identity);
                }
            }


            // 一定時間経過したら待機状態に遷移
            if (dodgeTime >= state._playerData.dodgeTime)
            {
                // 回避終了時に出すスキルを出す
                if (state._playerSkill.dashSkillData.endAttack != null)
                {
                    Instantiate(state._playerSkill.dashSkillData.endAttack, state.transform.position, Quaternion.identity);
                }

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
            // 回避アニメーションを停止
            state._animator.ResetTrigger("Dodge");
            // 移動ベクトルをリセット
            state._rigidbody.velocity = Vector3.zero;
        }
    }

    // 近接攻撃状態
    public class MeleeAttackState : StateBase<PlayerState>
    {
        private string _currentAttackName;
        private AttackData _currentAttackData;
        private int _currentFrame;
        private GameObject _effectObject;
        // 攻撃を出したかどうか
        bool _isAttack;
        public MeleeAttackState(PlayerState next) : base(next)
        {

        }
        public override void OnEnterState()
        {
            // 回避でキャンセル可能にする
            state._isAbleToDodge = true;
            // 攻撃の入力を一時的に無効化
            state._isAbleToAttack = false;
            // 特殊攻撃を不可にする
            state.isAbleToSpecialAttack = false;
            // 現在のStateKindを近接攻撃に設定
            state._stateKind = StateKind.MELEEATTACK;

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
            Vector3 effectPos = state.GetAttackPosition(_currentAttackData.attackPart);
            // 座標を少しプレイヤーから離す
            Vector3 shiftVec = (effectPos - state.transform.position).normalized;
            effectPos += shiftVec * 0.5f;

            _effectObject = Instantiate(_currentAttackData.attackEffect, effectPos, Quaternion.identity);

            _currentFrame = 0;
        }
        public override void OnUpdate()
        {
            _currentFrame++;

            // エフェクトの位置を更新
            if (_effectObject)
            {
                Vector3 effectPos = state.GetAttackPosition(_currentAttackData.attackPart);
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
            if (_currentFrame > _currentAttackData.startFrame)
            {
                // 攻撃オブジェクトが存在するなら攻撃オブジェクトの座標を更新
                if (state._currentAttack)
                {
                    Vector3 position = state.GetAttackPosition(_currentAttackData.attackPart);

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

                // 硬直フレームの間は回避不可
                if (_currentFrame <= _currentAttackData.stunFrame)
                {
                    state._isAbleToDodge = false;
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
                    AttackData nextAttackData = state.SearchAttackData(nextAttackName);

                    // 現在の攻撃オブジェクトを削除
                    if (state._currentAttack)
                    {
                        state._currentAttack = null;
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
                        Vector3 effectPos = state.GetAttackPosition(_currentAttackData.attackPart);
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

        AttackData _currentAttackData;

        public RangedAttackState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // 回避でキャンセル可能にする
            state._isAbleToDodge = true;
            // 攻撃の入力を一時的に無効化
            state._isAbleToAttack = false;
            // 特殊攻撃を不可にする
            state.isAbleToSpecialAttack = false;
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

            // 現在のStateKindを遠距離攻撃に設定
            state._stateKind = StateKind.RANGEDATTACK;
            // 遠距離攻撃アニメーションを再生
            state._animator.SetTrigger("RangedAttack");
            // 攻撃の情報を設定
            _currentAttackData = state.SearchAttackData("RangedAttack");
            // フレームカウントをリセット
            _currentFrame = 0;
        }

        public override void OnUpdate()
        {
            _currentFrame++;


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

        public ChargeState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // チャージ中は回避可能
            state._isAbleToDodge = true;
            // チャージ中は攻撃不可
            state._isAbleToAttack = false;
            // チャージ中は特殊攻撃不可
            state.isAbleToSpecialAttack = false;
            // 状態をチャージに設定
            state._stateKind = StateKind.CHARGE;
            // 通常チャージと特殊チャージをここで分ける
            if (state._isSpecialCharge)
            {
                // 特殊チャージアニメーションを再生
                state._animator.SetTrigger("SpecialCharge");
            }
            else
            {
                // 通常チャージアニメーションを再生
                state._animator.SetTrigger("NormalCharge");
            }
        }
        public override void OnUpdate()
        {
            //特殊チャージの場合
            if (state._isSpecialCharge)
            {
                // 特殊攻撃可能
                state.isAbleToSpecialAttack = true;


                if (state._playerSkill.specialChargeSkill != null)
                {
                    // 溜め速度が早くなっている場合チャージを早める
                    state._specialChargeTime += state._playerSkill.specialChargeSkill.chargeSpeedRate;

                    // チャージ中に出すスキルを出す
                    if (state._playerSkill.specialChargeSkill.chargingAttackObject != null)
                    {
                        Instantiate(state._playerSkill.specialChargeSkill.chargingAttackObject, state.transform.position, Quaternion.identity);
                    }
                }
                else
                {
                    state._specialChargeTime++;
                }

                // 最大チャージ時間を超えたらアイドルに遷移
                if (state._specialChargeTime >= state._playerData.maxSpecialChargeTime)
                {
                    state._specialChargeTime = state._playerData.maxSpecialChargeTime;

                    state.ChangeState(new IdleState(state));
                    return;
                }
            }
            // 通常チャージの場合
            else
            {
                state._normalChargeTime++;

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
            if (state._isSpecialCharge)
            {
                state._animator.ResetTrigger("SpecialCharge");
                state._isSpecialCharge = false;
            }
            else
            {
                state._animator.ResetTrigger("NormalCharge");
            }
        }
    }

    // チャージ攻撃状態
    public class ChargeAttackState : StateBase<PlayerState>
    {
        private string _currentAttackName;
        private AttackData _currentAttackData;
        private int _currentFrame;
        public ChargeAttackState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // 回避不可能にする
            state._isAbleToDodge = false;
            // 攻撃の入力を無効化
            state._isAbleToAttack = false;
            // 特殊攻撃を不可にする
            state.isAbleToSpecialAttack = false;
            // 現在のStateKindを近接攻撃に設定
            state._stateKind = StateKind.CHARGEATTACK;

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
            _currentFrame++;

            // 攻撃オブジェクトを生成するフレームに達したら攻撃オブジェクトを生成
            if (_currentFrame == _currentAttackData.startFrame)
            {
                state.CreateAttack(_currentAttackData);
                // チャージ時間をリセット
                state._normalChargeTime = 0;
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
        }
    }

    // 特殊攻撃状態
    public class SpecialAttackState : StateBase<PlayerState>
    {
        private int _stateTime;

        private string _currentAttackName;

        private AttackData _currentAttackData;

        public SpecialAttackState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // 回避不可能にする
            state._isAbleToDodge = false;
            // 攻撃の入力を無効化
            state._isAbleToAttack = false;
            // 特殊攻撃を不可にする
            state.isAbleToSpecialAttack = false;
            // 現在のStateKindを近接攻撃に設定
            state._stateKind = StateKind.SPECIALATTACK;

            _stateTime = 0;

            // チャージ時間が最大なら強特殊攻撃、そうでなければ弱特殊攻撃に設定
            if (state._specialChargeTime == state._playerData.maxSpecialChargeTime)
            {
                _currentAttackName = "SpecialAttack";
            }
            else
            {
                _currentAttackName = "LowSpecialAttack";
            }

            Debug.Log(_currentAttackName);

            // 特殊攻撃アニメーションを再生
            state._animator.SetTrigger(_currentAttackName);
            // 攻撃の情報を設定
            _currentAttackData = state.SearchAttackData(_currentAttackName);
        }
        public override void OnUpdate()
        {
            _stateTime++;

            // 攻撃オブジェクトを生成するフレームに達したら攻撃オブジェクトを生成
            if (_stateTime == _currentAttackData.startFrame)
            {
                state.CreateAttack(_currentAttackData);

                // TODO : 弾を生成する

                // チャージ時間をリセット
                state._specialChargeTime = 0;

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
            state._specialChargeTime = 0;
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

        public DamageState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // ダメージ中は回避不可
            state._isAbleToDodge = false;
            // ダメージ中は攻撃不可
            state._isAbleToAttack = false;
            // 状態をダメージに設定
            state._stateKind = StateKind.DAMAGE;

            // ダメージの種類でスタン時間とアニメーションを変更

            // 小ダメージ
            if (state._damageKind == DamageKind.LOW)
            {
                _stunDuration = state._damageData.lowStanTime;
                _knockbackTime = 0;
                _knockBackScale = 0;

                // 軽ダメージアニメーションを再生
                _damageAnim = "LowHit";
            }
            // 中ダメージ
            else if (state._damageKind == DamageKind.MIDDLE)
            {
                _stunDuration = state._damageData.middleStanTime;
                _knockbackTime = state._damageData.middleKnockBackTime;
                _knockBackScale = state._damageData.middleKnockBackScale;

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
            else if (state._damageKind == DamageKind.HEAVY)
            {
                _stunDuration = state._damageData.highStanTime;
                _knockbackTime = state._damageData.highKnockBackTime;
                _knockBackScale = state._damageData.highKnockBackScale;

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
        }
        public override void OnUpdate()
        {
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
        }
    }

    // 死亡状態
    public class DeadState : StateBase<PlayerState>
    {
        public DeadState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // 状態を死亡に設定
            state._stateKind = StateKind.DEAD;
            // 死亡アニメーションを再生
            state._animator.SetTrigger("Dead");
            // 移動ベクトルをリセット
            state._rigidbody.velocity = Vector3.zero;
            // 回避不可にする
            state._isAbleToDodge = false;
            // 攻撃不可にする
            state._isAbleToAttack = false;
            // 特殊攻撃不可にする
            state.isAbleToSpecialAttack = false;
        }
        public override void OnUpdate()
        {
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
            _isSpecialCharge = false;
            ChangeState(new ChargeState(this));
        }
    }

    private void ChargeAttack(InputAction.CallbackContext context)
    {
        // 一定時間以上チャージを行っていたらチャージ攻撃に移行
        if (_normalChargeTime > _playerData.chargeAttackTime)
        {
            ChangeState(new ChargeAttackState(this));
        }
        // そうでなければ待機状態に戻る
        else
        {

            // 回避中にキャンセルしちゃってるからしゃあなくif追加
            if (_stateKind == StateKind.DODGE)
            {
                return;
            }

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

    private void SpecialCharge(InputAction.CallbackContext context)
    {
        if (_isAbleToAttack && _stateKind != StateKind.MELEEATTACK)
        {
            _isSpecialCharge = true;
            ChangeState(new ChargeState(this));
        }
    }

    private void SpecialAttack(InputAction.CallbackContext context)
    {
        // 特殊攻撃チャージ中なら特殊攻撃状態に遷移
        if (_stateKind == StateKind.CHARGE && _isSpecialCharge)
        {
            // アニメーションが特殊攻撃開始アニメであれば
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("SpecialChargeStart"))
            {
                // 何もしない
                return;
            }

            ChangeState(new SpecialAttackState(this));
        }
        // 特殊ゲージが最大なら特殊攻撃状態に遷移
        else if (isAbleToSpecialAttack && _specialChargeTime == _playerData.maxSpecialChargeTime)
        {
            ChangeState(new SpecialAttackState(this));
        }
    }

    private AttackData SearchAttackData(string attackName)
    {
        AttackData result = null;

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

    private Vector3 GetAttackPosition(string partName)
    {
        Vector3 result = Vector3.zero;

        // 早期リターン
        if (partName == "None") return result;

        // 攻撃部位のリグの名前を取得
        string rigName = null;

        for (int i = 0; i < _attackPartData.attackDataList.Count; i++)
        {
            if (_attackPartData.attackDataList[i].attackPartName == partName)
            {
                rigName = _attackPartData.attackDataList[i].objectRigName;
                break;
            }
        }

        // 早期リターン
        if (rigName == null) return result;

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

    private void CreateAttack(AttackData data)
    {
        // ゲームオブジェクトを生成
        GameObject attackObject = Instantiate(_attackObjectPrefab);

        // 当たり判定のサイズ
        float scale = data.scale;

        // 攻撃のデータ
        PlayerAttack.PlayerAttackData attackData = new PlayerAttack.PlayerAttackData();

        // 攻撃にスキルの効果を乗せる
        switch (data.attackKind)
        {
            case AttackData.AttackType.LowAttack:
                if (_playerSkill.lowAttackSkillData != null)
                {
                    // ダメージにスキルのダメージ加算率を加算
                    attackData.damage = data.damage + (int)(data.damage * (_playerSkill.lowAttackSkillData.damageAddRate / 100));


                    // ノックバックを追加
                    attackData.knockBackPower = _playerSkill.lowAttackSkillData.addKnockBackPower;

                    // 当たり判定のサイズを変更
                    scale = data.scale + data.scale * (_playerSkill.lowAttackSkillData.attackRangeAddRate / 100);

                    // デバフを追加
                    attackData.debuffType = _playerSkill.lowAttackSkillData.debuffType;

                    // 追撃を追加
                    attackData.chaseAttack = _playerSkill.lowAttackSkillData.chaseAttack;
                }
                // ない場合は通常攻撃のデータをそのまま使用
                else
                {
                    attackData.damage = data.damage;
                }
                break;

            case AttackData.AttackType.ChargeAttack:
                if (_playerSkill.chargeAttackSkillData != null)
                {
                    // ダメージにスキルのダメージ加算率を加算
                    attackData.damage = data.damage + (data.damage * (_playerSkill.chargeAttackSkillData.damageAddRate / 100));

                    // ノックバックを追加
                    attackData.knockBackPower = _playerSkill.chargeAttackSkillData.addKnockBackPower;

                    // 当たり判定のサイズを変更
                    scale = data.scale + data.scale * (_playerSkill.chargeAttackSkillData.attackRangeAddRate / 100);

                    // デバフを追加
                    attackData.debuffType = _playerSkill.chargeAttackSkillData.debuffType;

                    // 追撃を追加
                    attackData.chaseAttack = _playerSkill.chargeAttackSkillData.chaseAttack;

                    // 跳ね返すかどうかを追加
                    attackData.isReflect = _playerSkill.chargeAttackSkillData.isReflect;
                }
                // ない場合は通常の攻撃のデータをそのまま使用
                else
                {
                    attackData.damage = data.damage;
                }
                break;
        }
        // 当たり判定のサイズを設定
        attackObject.GetComponent<SphereCollider>().radius = scale;

        // 攻撃の情報を設定
        attackObject.GetComponent<PlayerAttack>().SetPlayerAttackData(attackData);

        // 攻撃の位置を設定
        Vector3 position = GetAttackPosition(data.attackPart);

        // 攻撃の座標を設定
        attackObject.GetComponent<PlayerAttack>().SetPlayerPos(position);

        // ずらす分を加算
        Vector3 shiftVec = transform.forward * data.shiftPosZ;

        attackObject.transform.position = position + shiftVec;

        // 攻撃の向きを設定
        attackObject.transform.forward = transform.forward;

        // 攻撃オブジェクトを保存
        _currentAttack = attackObject;
    }

    /// <summary>
    /// 遠距離攻撃オブジェクトを生成するときに使用
    /// </summary>
    /// <param name="attack">攻撃オブジェクト</param>
    private GameObject CreateRangedAttack(AttackData data)
    {
        GameObject attack = _bulletPrefab;

        PlayerRangedAttack.RangedAttackData attackData = new PlayerRangedAttack.RangedAttackData();

        // 攻撃の座標を設定
        Vector3 position = GetAttackPosition(data.attackPart);
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
    public int GetMaxHp()
    {
        return _playerData.maxHp;
    }
    public int GetNowHp()
    {
        return _nowHp;
    }
    public int GetMaxBulletNum()
    {
        return _playerData.maxBulletNum;
    }
    public int GetNowBulletNum()
    {
        return _nowBulletNum;
    }
    public float GetMaxSpecialChargeTime()
    {
        return _playerData.maxSpecialChargeTime;
    }
    public float GetNowSpecialChargeTime()
    {
        return _specialChargeTime;
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
    public void AddPassiveSkill(PassiveSkillData skill)
    {
        _playerSkill.passiveSkillDataList.Add(skill);
    }
    void OnTriggerEnter(Collider other)
    {
        // 敵の攻撃に当たったらダメージ状態に遷移
        if (other.gameObject.CompareTag("EnemyAttack")||
            other.gameObject.CompareTag("EnemyRangedAttack"))
        {
            // 死亡時はダメージを受けない
            if (_stateKind == StateKind.DEAD) return;
            // 回避中はダメージを受けない
            if (_stateKind == StateKind.DODGE) return;
            // 被弾中はダメージを受けない
            if (_stateKind == StateKind.DAMAGE) return;

            // 攻撃を前から受けたかどうか
            Vector3 toEnemy = other.transform.position - transform.position;
            toEnemy.y = 0; // y成分を無視して水平面での方向を考える
            toEnemy.Normalize();
            float dot = Vector3.Dot(transform.forward, toEnemy);
            if (dot > 0)
            {
                _isFrontDamage = true;

                // もし攻撃がHeavyなら攻撃の方向を向く
                if (_damageKind == DamageKind.HEAVY)
                {
                    transform.forward = toEnemy;
                    _currentDirection = toEnemy;
                }
            }
            else
            {
                _isFrontDamage = false;

                // もし攻撃がHeavyなら攻撃の方向と逆を向く
                if (_damageKind == DamageKind.HEAVY)
                {
                    transform.forward = -toEnemy;
                    _currentDirection = -toEnemy;
                }
            }

            // 最大溜めの特殊攻撃を行っているときはダメージ状態に遷移しない
            if (_stateKind == StateKind.SPECIALATTACK && _specialChargeTime == _playerData.maxSpecialChargeTime)
            {
                // ダメージをカットする
                int damage = (int)((float)other.gameObject.GetComponent<ZangiAttack>().GetDamage() * _playerData.maxSpecialAttackDamegeCutRate);

                _nowHp -= damage;

                // HPを1以下にしない
                if (_nowHp <= 0)
                {
                    _nowHp = 1;
                }

            }
            else
            {
                // HPを減らす
                _nowHp -= (int)other.gameObject.GetComponent<ZangiAttack>().GetDamage();

                // ダメージの種類を取得
                _damageKind = other.gameObject.GetComponent<ZangiAttack>().GetDamageKind();

                // 当たり判定と攻撃の当たり判定が重なった位置にエフェクトを生成
                Vector3 hitPosition = other.ClosestPoint(transform.position);

                // ダメージエフェクトを生成
                Instantiate(other.gameObject.GetComponent<ZangiAttack>().GetHitEffectPrefab(), hitPosition, Quaternion.identity);

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
        }

        // ドロップした弾にあたったら弾を補充
        if (other.gameObject.CompareTag("DropBullet"))
        {
            // 弾を補充
            _nowBulletNum++;

            // アイテムを親ごと消す
            Destroy(other.transform.parent.gameObject);
        }

    }
}

