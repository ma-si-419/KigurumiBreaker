using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PlayerState : Player<PlayerState>
{

    // ��Ԃ�enum
    public enum StateKind
    {
        IDLE,           // �ҋ@
        MOVE,           // �ړ�
        DODGE,          // ���
        MELEEATTACK,    // �ߐڍU��
        RANGEDATTACK,   // �������U��
        CHARGE,         // �`���[�W
        CHARGEATTACK,    // �`���[�W�U��
        DAMAGE,         // �_���[�W
        DEATH,          // ���S
        CINEMATIC,      // ���o��
    }

    // �U���f�[�^
    [SerializeField] private AttackDataList _attackData;

    // �v���C���[���g�p����萔�f�[�^
    [SerializeField] private PlayerData _playerData;

    // ���͏��
    private GameInputs _input;

    // ���݂̏��
    private StateKind _stateKind;

    // �ړ�����
    private Vector2 _moveInput;

    // ���݌����Ă������
    private Vector3 _currentDirection;

    // ���M�b�h�{�f�B
    private Rigidbody _rigidbody;

    // ���݉���ł��邩�ǂ���
    private bool _isAbleToDodge;

    // ���ݍU���ł��邩�ǂ���
    private bool _isAbleToAttack;

    // �U�����͂����ꂽ���ǂ���
    private bool _isAttackInput;

    // �U���{�^���𒷉������Ă��鎞��
    private int _normalChargeTime;

    // ����U���̃`���[�W�����Ă���ꍇtrue�ɂ���
    bool _isSpecialCharge;

    // �A�j���[�^�[
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        // Animator�R���|�[�l���g���擾
        _animator = GetComponent<Animator>();

        // Rigidbody�R���|�[�l���g���擾
        _rigidbody = GetComponent<Rigidbody>();

        // PlayerInput�R���|�[�l���g���擾
        _input = new GameInputs();

        // InputAction�̐ݒ�
        _input.Player.Move.started += Move;
        _input.Player.Move.performed += Move;
        _input.Player.Move.canceled += Move;
        _input.Player.Dodge.performed += Dodge;
        _input.Player.MeleeAttack.started += LowAttack;
        _input.Player.ChargeAttack.started += NormalCharge;
        _input.Player.ChargeAttack.canceled += ChargeAttack;

        // InputAction��L����
        _input.Enable();

        // ������Ԃ͑ҋ@��Ԃɐݒ�
        ChangeState(new IdleState(this));
    }

    // �eState�N���X

    // �ҋ@���
    public class IdleState : StateBase<PlayerState>
    {
        public IdleState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // �ҋ@��Ԃł͉���\
            state._isAbleToDodge = true;
            // �ҋ@��Ԃł͍U���\
            state._isAbleToAttack = true;

            // ��Ԃ�ҋ@�ɐݒ�
            state._stateKind = StateKind.IDLE;

            // �ҋ@�A�j���[�V�������Đ�
            state._animator.SetBool("Idle", true);

            // �ړ��x�N�g�������Z�b�g
            state._rigidbody.velocity = Vector3.zero;

        }
        public override void OnUpdate()
        {
            // �U�����͂�����΋ߐڍU����ԂɑJ��
            if (state._isAttackInput)
            {
                state._isAttackInput = false;
                state.ChangeState(new MeleeAttackState(state));
                return;
            }

            // �ړ����͂�����Έړ���ԂɑJ��
            float magnitude = state._moveInput.magnitude;
            if (magnitude > state._playerData.moveInputLength)
            {
                state.ChangeState(new MoveState(state));
                return;
            }
        }
        public override void OnExitState()
        {
            // �ҋ@�A�j���[�V�������~
            state._animator.SetBool("Idle", false);
        }
    }

    // �ړ����
    public class MoveState : StateBase<PlayerState>
    {
        public MoveState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // �ړ���Ԃł͉���\
            state._isAbleToDodge = true;
            // �ړ���Ԃł͍U���\
            state._isAbleToAttack = true;
            // ��Ԃ��ړ��ɐݒ�
            state._stateKind = StateKind.MOVE;
            // �ړ��A�j���[�V�������Đ�
            state._animator.SetBool("Move", true);
        }
        public override void OnUpdate()
        {
            // �ړ������̌v�Z
            Vector3 moveDirection = new Vector3(state._moveInput.x, 0, state._moveInput.y).normalized;

            // �ړ��x�N�g��
            Vector3 moveVelocity = moveDirection * state._playerData.moveSpeed;

            // �����̍X�V
            if (moveDirection != Vector3.zero)
            {
                state._currentDirection = moveDirection;
                state.transform.forward = state._currentDirection;
            }

            // ���W�b�h�{�f�B�̑��x��ݒ�
            state._rigidbody.velocity = moveVelocity;

            // �U�����͂�����΋ߐڍU����ԂɑJ��
            if (state._isAttackInput)
            {
                state._isAttackInput = false;
                state.ChangeState(new MeleeAttackState(state));
                return;
            }
            // �ړ����͂��Ȃ���Αҋ@��ԂɑJ��
            float magnitude = state._moveInput.magnitude;
            if (magnitude <= state._playerData.moveInputLength)
            {
                state.ChangeState(new IdleState(state));
            }
        }
        public override void OnExitState()
        {
            // �ړ��A�j���[�V�������~
            state._animator.SetBool("Move", false);

            // �ړ��x�N�g�������Z�b�g
            state._rigidbody.velocity = Vector3.zero;
        }
    }

    // ������
    public class DodgeState : StateBase<PlayerState>
    {
        // ������Ԃ̃J�E���g
        int dodgeTime;

        // ������
        Vector3 dodgeDirection;

        public DodgeState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // ��𒆂͉��s��
            state._isAbleToDodge = false;
            // ��𒆂͍U���s��
            state._isAbleToAttack = false;
            // ��Ԃ�����ɐݒ�
            state._stateKind = StateKind.DODGE;
            // ����A�j���[�V�������Đ�
            state._animator.SetTrigger("Dodge");
            // ������Ԃ�ݒ�
            dodgeTime = 0;

            // �ړ������̌v�Z
            dodgeDirection = new Vector3(state._moveInput.x, 0, state._moveInput.y).normalized;
            // �ړ��������Ȃ��ꍇ�͌��݂̌������g�p
            if (state._moveInput.magnitude < state._playerData.moveInputLength)
            {
                dodgeDirection = state._currentDirection;
            }

        }
        public override void OnUpdate()
        {
            // ������Ԃ��J�E���g
            dodgeTime++;

            // �������Ɍ�����
            if (dodgeDirection != Vector3.zero)
            {
                state.transform.forward = dodgeDirection;
            }

            // �ŏ��̐��t���[���͈ړ����Ȃ�
            if (dodgeTime < state._playerData.dodgeStartTime)
            {
                state._rigidbody.velocity = Vector3.zero;
                return;
            }

            // �ړ�����
            Vector3 dodgeVelocity = dodgeDirection * state._playerData.dodgeSpeed;
            state._rigidbody.velocity = dodgeVelocity;

            // ��莞�Ԍo�߂�����ҋ@��ԂɑJ��
            if (dodgeTime >= state._playerData.dodgeTime)
            {
                // �ړ����͂�����Έړ���ԂɑJ�ځA�Ȃ���Αҋ@��ԂɑJ��
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
            // ����A�j���[�V�������~
            state._animator.ResetTrigger("Dodge");
            // �ړ��x�N�g�������Z�b�g
            state._rigidbody.velocity = Vector3.zero;
        }
    }

    // �ߐڍU�����
    public class MeleeAttackState : StateBase<PlayerState>
    {
        private string _currentAttackName;
        private AttackData _currentAttackData;
        private int _currentFrame;
        public MeleeAttackState(PlayerState next) : base(next)
        {

        }
        public override void OnEnterState()
        {
            // ����ŃL�����Z���\�ɂ���
            state._isAbleToDodge = true;
            // �U���̓��͂��ꎞ�I�ɖ�����
            state._isAbleToAttack = false;
            // ���݂�StateKind���ߐڍU���ɐݒ�
            state._stateKind = StateKind.MELEEATTACK;

            // �ŏ��̍U����Low1�ɐݒ�
            _currentAttackName = "Low1";

            // �A�j���[�V�������Đ�
            state._animator.SetTrigger(_currentAttackName);

            // �U���̏���ݒ�
            _currentAttackData = state.SearchAttackData(_currentAttackName);

        }
        public override void OnUpdate()
        {
            _currentFrame++;
            // �U�����o������
            if (_currentFrame >= _currentAttackData.startFrame)
            {
                // �U���̓��͂��󂯕t����
                state._isAbleToAttack = true;

                // �d���t���[���̊Ԃ͉��s��
                if (_currentFrame <= _currentAttackData.stunFrame)
                {
                    state._isAbleToDodge = false;
                }
                else
                {
                    state._isAbleToDodge = true;
                }
            }
            // �U�����o���O
            else
            {
                // �U���̓��͂𖳌���
                state._isAbleToAttack = false;
            }

            // �U���L�����Z���t���[���ɒB�������ɍU�����͂�����Ύ��̍U���ɑJ��
            if (_currentFrame >= _currentAttackData.cancelFrame)
            {
                // �U�����͂�����Ύ��̍U���ɑJ��
                if (state._isAttackInput)
                {
                    // �U�����͂����Z�b�g
                    state._isAttackInput = false;
                    // ���̍U���f�[�^���擾
                    string nextAttackName = _currentAttackData.nextAttackName;
                    AttackData nextAttackData = state.SearchAttackData(nextAttackName);
                    // ���̍U���f�[�^�����݂���ꍇ�A���̍U���ɑJ��
                    if (nextAttackData != null)
                    {
                        _currentAttackName = nextAttackName;
                        _currentAttackData = nextAttackData;
                        _currentFrame = 0;
                        // ���̍U���A�j���[�V�������Đ�
                        state._animator.SetTrigger(_currentAttackName);
                    }
                }
            }


            // �U���̃g�[�^���t���[���ɒB������A�C�h����ԂɑJ��
            if (_currentFrame >= _currentAttackData.totalFrame)
            {
                state.ChangeState(new IdleState(state));
            }
        }
        public override void OnExitState()
        {
            // �U���A�j���[�V�������~
            state._animator.ResetTrigger(_currentAttackData.attackName);

            // �`���[�W���Ԃ����Z�b�g
            state._normalChargeTime = 0;
        }
    }

    // �`���[�W���
    public class ChargeState : StateBase<PlayerState>
    {
        public ChargeState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // �`���[�W���͉���\
            state._isAbleToDodge = true;
            // �`���[�W���͍U���s��
            state._isAbleToAttack = false;
            // ��Ԃ��`���[�W�ɐݒ�
            state._stateKind = StateKind.CHARGE;
            // �ʏ�`���[�W�Ɠ���`���[�W�������ŕ�����
            if (state._isSpecialCharge)
            {
                // ����`���[�W�A�j���[�V�������Đ�
                state._animator.SetTrigger("SpecialCharge");
            }
            else
            {
                // �ʏ�`���[�W�A�j���[�V�������Đ�
                state._animator.SetTrigger("NormalCharge");
            }
        }
        public override void OnUpdate()
        {
            //����`���[�W�̏ꍇ
            if (state._isSpecialCharge)
            {

            }
            // �ʏ�`���[�W�̏ꍇ
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

    // �`���[�W�U�����
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
            // ���s�\�ɂ���
            state._isAbleToDodge = false;
            // �U���̓��͂𖳌���
            state._isAbleToAttack = false;
            // ���݂�StateKind���ߐڍU���ɐݒ�
            state._stateKind = StateKind.CHARGEATTACK;

            // �ʏ�`���[�W�U���Ɠ���`���[�W�U���������ŕ�����
            if (state._isSpecialCharge)
            {

            }
            else
            {
                // �ŏ��̍U����ChargeAttack�ɐݒ�
                _currentAttackName = "ChargeAttack";
                // �A�j���[�V�������Đ�
                state._animator.SetTrigger(_currentAttackName);
                // �U���̏���ݒ�
                _currentAttackData = state.SearchAttackData(_currentAttackName);
            }
            _currentFrame = 0;
        }
        public override void OnUpdate()
        {
            _currentFrame++;

            // �U���̃L�����Z���t���[���ɒB�����Ƃ��ɉ���ŃL�����Z���ł���悤�ɂ���
            if (_currentFrame >= _currentAttackData.cancelFrame)
            {
                // ����\�ɂ���
                state._isAbleToDodge = true;
            }

            // �U���̃g�[�^���t���[���ɒB������
            if (_currentFrame >= _currentAttackData.totalFrame)
            {
                // �ړ����͂�����Έړ���ԂɑJ�ځA�Ȃ���Αҋ@��ԂɑJ��
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
            // �U���A�j���[�V�������~
            state._animator.ResetTrigger(_currentAttackData.attackName);
            // �`���[�W���Ԃ����Z�b�g
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
            // �`���[�W���Ԃ����Z�b�g
            _normalChargeTime = 0;

            // �����ԂɑJ��
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
        // ��莞�Ԉȏ�`���[�W���s���Ă�����`���[�W�U���Ɉڍs
        if (_normalChargeTime > _playerData.chargeAttackTime)
        {
            ChangeState(new ChargeAttackState(this));
        }
        // �����łȂ���Αҋ@��Ԃɖ߂�
        else
        {

            // ��𒆂ɃL�����Z����������Ă邩�炵�Ⴀ�Ȃ�if�ǉ�
            if (_stateKind == StateKind.DODGE)
            {
                return;
            }

            //�`���[�W���Ԃ����Z�b�g
            _normalChargeTime = 0;

            // �ړ����͂�����Έړ���ԂɑJ�ځA�Ȃ���Αҋ@��ԂɑJ��
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
}
