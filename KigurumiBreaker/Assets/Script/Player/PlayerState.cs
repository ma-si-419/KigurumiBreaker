using System.Collections;
using System.Collections.Generic;
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
        DAMAGE,         // ダメージ
        DEATH,          // 死亡
        CINEMATIC,      // 演出中
    }

    // 攻撃データ
    [SerializeField] private AttackDataList _attackData;

    // 攻撃オブジェクトの基本データ
    [SerializeField] private GameObject _attackObjectPrefab;

    // プレイヤーが使用する定数データ
    [SerializeField] private PlayerData _playerData;

    // プレイヤーの攻撃部位
    [SerializeField] private AttackPartList _attackPartData;

    // 入力情報
    private GameInputs _input;

    // 現在の状態
    private StateKind _stateKind;

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

    // 攻撃入力がされたかどうか
    private bool _isAttackInput;

    // 攻撃ボタンを長押ししている時間
    private int _normalChargeTime;

    // 特殊攻撃のチャージをしている場合trueにする
    bool _isSpecialCharge;

    // アニメーター
    private Animator _animator;

    // 出している攻撃オブジェクト
    private GameObject _currentAttack;


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
        _input.Player.ChargeAttack.started += NormalCharge;
        _input.Player.ChargeAttack.canceled += ChargeAttack;

        // InputActionを有効化
        _input.Enable();

        // 初期状態は待機状態に設定
        ChangeState(new IdleState(this));
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
            // 状態を移動に設定
            state._stateKind = StateKind.MOVE;
            // 移動アニメーションを再生
            state._animator.SetBool("Move", true);
        }
        public override void OnUpdate()
        {
            // 移動方向の計算
            Vector3 direction = new Vector3(state._moveInput.x,0,state._moveInput.y).normalized;
            Vector3 moveDirection = state.CalculateMoveDirection(direction);

            // 移動ベクトル
            Vector3 moveVelocity = moveDirection * state._playerData.moveSpeed;

            // 向きの更新
            if (moveDirection != Vector3.zero)
            {
                state._currentDirection = moveDirection;
                state.transform.forward = state._currentDirection;
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

            // 一定時間経過したら待機状態に遷移
            if (dodgeTime >= state._playerData.dodgeTime)
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
            // 現在のStateKindを近接攻撃に設定
            state._stateKind = StateKind.MELEEATTACK;

            // 最初の攻撃はLow1に設定
            _currentAttackName = "Low1";

            _isAttack = false;

            // アニメーションを再生
            state._animator.SetTrigger(_currentAttackName);

            // 攻撃の情報を設定
            _currentAttackData = state.SearchAttackData(_currentAttackName);

        }
        public override void OnUpdate()
        {
            _currentFrame++;

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
                if (state._currentAttack != null)
                {
                    Vector3 position = state.GetAttackPosition(_currentAttackData.attackPart);

                    // ずらす分を加算
                    Vector3 shiftVec = state.transform.forward * _currentAttackData.shiftPosZ;
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
                    if (state._currentAttack != null)
                    {
                        state._currentAttack = null;
                    }

                    // 次の攻撃データが存在する場合、次の攻撃に遷移
                    if (nextAttackData != null)
                    {
                        _currentAttackName = nextAttackName;
                        _currentAttackData = nextAttackData;
                        _currentFrame = 0;
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
                state.ChangeState(new IdleState(state));
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

            }
            // 通常チャージの場合
            else
            {
                state._normalChargeTime++;
            }
        }
        public override void OnExitState()
        {
            state._animator.ResetTrigger("NormalCharge");
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
            // 現在のStateKindを近接攻撃に設定
            state._stateKind = StateKind.CHARGEATTACK;

            // 通常チャージ攻撃と特殊チャージ攻撃をここで分ける
            if (state._isSpecialCharge)
            {

            }
            else
            {
                // 最初の攻撃はChargeAttackに設定
                _currentAttackName = "ChargeAttack";
                // アニメーションを再生
                state._animator.SetTrigger(_currentAttackName);
                // 攻撃の情報を設定
                _currentAttackData = state.SearchAttackData(_currentAttackName);
            }
            _currentFrame = 0;
        }
        public override void OnUpdate()
        {
            _currentFrame++;

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
        if (_isAbleToAttack)
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

        // 球の当たり判定を設定
        attackObject.GetComponent<SphereCollider>().radius = data.scale;

        // 攻撃の情報を設定
        attackObject.GetComponent<PlayerAttack>().SetAttackData(data);

        // 攻撃の位置を設定
        Vector3 position = GetAttackPosition(data.attackPart);
        
        // ずらす分を加算
        Vector3 shiftVec = transform.forward * data.shiftPosZ;

        Debug.Log(transform.forward);
        Debug.Log(shiftVec);

        attackObject.transform.position = position + shiftVec;

        Debug.Log(position);

        // 攻撃の向きを設定
        attackObject.transform.forward = transform.forward;

        // 攻撃オブジェクトを保存
        _currentAttack = attackObject;
    }

    private Vector3 CalculateMoveDirection(Vector3 direction)
    {
        Vector3 moveDirection = direction;

        // 固定値分回転させる
        moveDirection = Quaternion.Euler(0, _playerData.moveDirAngle, 0) * moveDirection;

        return moveDirection;
    }
}
