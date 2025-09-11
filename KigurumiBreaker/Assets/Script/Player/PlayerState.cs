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
        SUPERATTACK,    // チャージ攻撃
        DAMAGE,         // ダメージ
        DEATH,          // 死亡
        CINEMATIC,      // 演出中
    }

    // 入力情報
    private GameInputs _input;

    // 現在の状態
    private StateKind _stateKind;

    // 移動入力
    private Vector2 _moveInput;

    // 現在回避できるかどうか
    private bool _isDodge;

    // アニメーター
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        // Animatorコンポーネントを取得
        _animator = GetComponent<Animator>();

        // PlayerInputコンポーネントを取得
        _input = new GameInputs();

        // InputActionの設定
        _input.Player.Move.started += Move;
        _input.Player.Move.performed += Move;
        _input.Player.Move.canceled += Move;
        _input.Player.Dodge.performed += Dodge;

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
            state._isDodge = true; // 待機状態では回避可能
        }
        public override void OnEnterState()
        {
            state._stateKind = StateKind.IDLE;

            // 待機アニメーションを再生
            state._animator.SetBool("Idle", true);
        }
        public override void OnUpdate()
        {
            // 移動入力があれば移動状態に遷移
            float magnitude = state._moveInput.magnitude;
            if (magnitude > MOVE_INPUT_LENGTH)
            {
                state.ChangeState(new MoveState(state));
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
            state._isDodge = true; // 移動状態では回避可能
        }
        public override void OnEnterState()
        {
            state._stateKind = StateKind.MOVE;
            // 移動アニメーションを再生
            state._animator.SetBool("Move", true);
        }
        public override void OnUpdate()
        {
            // 移動入力がなければ待機状態に遷移
            float magnitude = state._moveInput.magnitude;
            if (magnitude <= MOVE_INPUT_LENGTH)
            {
                state.ChangeState(new IdleState(state));
            }
        }
        public override void OnExitState()
        {
            // 移動アニメーションを停止
            state._animator.SetBool("Move", false);
        }
    }

    // 回避状態
    public class DodgeState : StateBase<PlayerState>
    {
        int dodgeTime;

        public DodgeState(PlayerState next) : base(next)
        {
            state._isDodge = false; // 回避中は回避不可
        }
        public override void OnEnterState()
        {
            state._stateKind = StateKind.DODGE;
            // 回避アニメーションを再生
            state._animator.SetTrigger("Dodge");
            // 回避時間を設定
            dodgeTime = 0;
        }
        public override void OnUpdate()
        {
            // 回避時間をカウント
            dodgeTime++;

            // 一定時間経過したら待機状態に遷移
            if (dodgeTime >= DODGE_TIME)
            {
                // 回避時間が経過したら待機状態に遷移
                state.ChangeState(new IdleState(state));
            }
        }
        public override void OnExitState()
        {
            // 回避アニメーションを停止
            state._animator.ResetTrigger("Dodge");
        }
    }

    private void Move(InputAction.CallbackContext constext)
    {
        _moveInput = constext.ReadValue<Vector2>();
    }

    private void Dodge(InputAction.CallbackContext constext)
    {
        if (_isDodge)
        {
            ChangeState(new DodgeState(this));
        }
    }
}
