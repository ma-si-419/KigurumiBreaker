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
        CHARGEATTACK,   // �`���[�W�U��
        SPECIALATTACK,  // ����U��
        DAMAGE,         // �_���[�W
        DEAD,           // ���S
        CINEMATIC,      // ���o��
    }

    public enum DamageKind
    {
        LOW,
        MIDDLE,
        HEAVY
    }

    // �U���f�[�^
    [SerializeField] private AttackDataList _attackData;

    // �U���I�u�W�F�N�g�̊�{�f�[�^
    [SerializeField] private GameObject _attackObjectPrefab;

    // �v���C���[���g�p����萔�f�[�^
    [SerializeField] private PlayerData _playerData;

    // �v���C���[�̍U������
    [SerializeField] private AttackPartList _attackPartData;

    // �v���C���[�̔�e���̃f�[�^
    [SerializeField] private DamageData _damageData;

    // ���͏��
    private GameInputs _input;

    // ���݂̏��
    private StateKind _stateKind;

    // �v���C���[�̗̑�
    private int _nowHp;

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

    // ���ݓ���U�����ł��邩�ǂ���
    private bool isAbleToSpecialAttack;

    // �U�����͂����ꂽ���ǂ���
    private bool _isAttackInput;

    // �U���{�^���𒷉������Ă��鎞��
    private int _normalChargeTime;

    // ����U���̃`���[�W�����Ă���ꍇtrue�ɂ���
    bool _isSpecialCharge;

    // ����U���̃`���[�W����
    private int _specialChargeTime;

    // �A�j���[�^�[
    private Animator _animator;

    // �o���Ă���U���I�u�W�F�N�g
    private GameObject _currentAttack;

    // �󂯂��_���[�W�̎��
    private DamageKind _damageKind;

    // �U����O����󂯂����ǂ���
    private bool _isFrontDamage;


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
        _input.Player.SpecialAttack.started += SpecialCharge;
        _input.Player.SpecialAttack.started += SpecialAttack;

        // InputAction��L����
        _input.Enable();

        // ������Ԃ͑ҋ@��Ԃɐݒ�
        ChangeState(new IdleState(this));

        // �̗͂��ő�̗͂ɐݒ�
        _nowHp = _playerData.maxHp;
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
            // �ҋ@��Ԃł͓���U���\
            state.isAbleToSpecialAttack = true;

            // ��Ԃ�ҋ@�ɐݒ�
            state._stateKind = StateKind.IDLE;

            // �ҋ@�A�j���[�V�������Đ�
            state._animator.SetBool("Idle", true);

            // �ړ��x�N�g�������Z�b�g
            state._rigidbody.velocity = Vector3.zero;

            // ���݂̌�����ۑ�
            state._currentDirection = state.transform.forward;


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
            // �ړ���Ԃł͓���U���\
            state.isAbleToSpecialAttack = true;
            // ��Ԃ��ړ��ɐݒ�
            state._stateKind = StateKind.MOVE;
            // �ړ��A�j���[�V�������Đ�
            state._animator.SetBool("Move", true);
        }
        public override void OnUpdate()
        {
            // �ړ������̌v�Z
            Vector3 direction = new Vector3(state._moveInput.x, 0, state._moveInput.y).normalized;
            Vector3 moveDirection = state.CalculateMoveDirection(direction);

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
            // ��𒆂͓���U���s��
            state.isAbleToSpecialAttack = false;
            // ��Ԃ�����ɐݒ�
            state._stateKind = StateKind.DODGE;
            // ����A�j���[�V�������Đ�
            state._animator.SetTrigger("Dodge");
            // ������Ԃ�ݒ�
            dodgeTime = 0;

            // �ړ������̌v�Z

            // �ړ��������Ȃ��ꍇ�͌��݂̌������g�p
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
        private GameObject _effectObject;
        // �U�����o�������ǂ���
        bool _isAttack;
        public MeleeAttackState(PlayerState next) : base(next)
        {

        }
        public override void OnEnterState()
        {
            // ����ŃL�����Z���\�ɂ���
            state._isAbleToDodge = true;
            // �U���̓��͂��ꎞ�I�ɖ�����
            state._isAbleToAttack = false;
            // ����U����s�ɂ���
            state.isAbleToSpecialAttack = false;
            // ���݂�StateKind���ߐڍU���ɐݒ�
            state._stateKind = StateKind.MELEEATTACK;

            // �ŏ��̍U����Low1�ɐݒ�
            _currentAttackName = "Low1";

            _isAttack = false;

            // �A�j���[�V�������Đ�
            state._animator.SetTrigger(_currentAttackName);

            // �U���̏���ݒ�
            _currentAttackData = state.SearchAttackData(_currentAttackName);

            // �U�����镔�ʂɃG�t�F�N�g���o��
            Vector3 effectPos = state.GetAttackPosition(_currentAttackData.attackPart);
            // ���W�������v���C���[���痣��
            Vector3 shiftVec = (effectPos - state.transform.position).normalized;
            effectPos += shiftVec * 0.5f;

            _effectObject = Instantiate(_currentAttackData.attackEffect, effectPos, Quaternion.identity);

            _currentFrame = 0;
        }
        public override void OnUpdate()
        {
            _currentFrame++;

            // �G�t�F�N�g�̈ʒu���X�V
            if (_effectObject)
            {
                Vector3 effectPos = state.GetAttackPosition(_currentAttackData.attackPart);
                // ���W�������v���C���[���痣��
                Vector3 shiftVec = (effectPos - state.transform.position).normalized;
                // Y���W���v�Z�ɓ���Ȃ�
                shiftVec.y = 0;

                effectPos += shiftVec * _currentAttackData.effectShiftScale;
                _effectObject.transform.position = effectPos;
            }

            // �U������t���[���ɒB������U���I�u�W�F�N�g�𐶐�
            if (_currentFrame >= _currentAttackData.startFrame && !_isAttack)
            {
                state.CreateAttack(_currentAttackData);
                _isAttack = true;
            }

            // �U�����o������
            if (_currentFrame > _currentAttackData.startFrame)
            {
                // �U���I�u�W�F�N�g�����݂���Ȃ�U���I�u�W�F�N�g�̍��W���X�V
                if (state._currentAttack)
                {
                    Vector3 position = state.GetAttackPosition(_currentAttackData.attackPart);

                    // ���炷�������Z
                    Vector3 shiftVec = state.transform.forward * _currentAttackData.effectShiftScale;
                    // Y���W���v�Z�ɓ���Ȃ�
                    shiftVec.y = 0;
                    position += shiftVec;

                    state._currentAttack.transform.position = position;
                }

                // �ړ��x�N�g�������Z�b�g
                state._rigidbody.velocity = Vector3.zero;

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

                // �����Ă�������ɐi��
                Vector3 attackVelocity = state._currentDirection * _currentAttackData.moveSpeed;
                state._rigidbody.velocity = attackVelocity;
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

                    // ���݂̍U���I�u�W�F�N�g���폜
                    if (state._currentAttack)
                    {
                        state._currentAttack = null;
                    }

                    // ���̍U���f�[�^�����݂���ꍇ�A���̍U���ɑJ��
                    if (nextAttackData != null)
                    {
                        _currentAttackName = nextAttackName;
                        _currentAttackData = nextAttackData;
                        _currentFrame = 0;

                        // �����̃G�t�F�N�g���폜
                        if (_effectObject)
                        {
                            Destroy(_effectObject);
                            _effectObject = null;
                        }

                        // �U�����镔�ʂɃG�t�F�N�g���o��
                        Vector3 effectPos = state.GetAttackPosition(_currentAttackData.attackPart);
                        _effectObject = Instantiate(_currentAttackData.attackEffect, effectPos, Quaternion.identity);

                        // ���̍U���A�j���[�V�������Đ�
                        state._animator.SetTrigger(_currentAttackName);

                        // �U���I�u�W�F�N�g�𐶐�����t���O�����Z�b�g
                        _isAttack = false;

                    }
                }
            }


            // �U���̃g�[�^���t���[���ɒB������A�C�h����ԂɑJ��
            if (_currentFrame >= _currentAttackData.totalFrame)
            {
                // �����̃G�t�F�N�g���폜
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
            // �U���A�j���[�V�������~
            state._animator.ResetTrigger(_currentAttackData.attackName);

            // �`���[�W���Ԃ����Z�b�g
            state._normalChargeTime = 0;

            // �����̃G�t�F�N�g���폜
            if (_effectObject)
            {
                Destroy(_effectObject);
                _effectObject = null;
            }
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
            // �`���[�W���͓���U���s��
            state.isAbleToSpecialAttack = false;
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
                // ����U���\
                state.isAbleToSpecialAttack = true;

                state._specialChargeTime++;

                // �ő�`���[�W���Ԃ𒴂�����A�C�h���ɑJ��
                if (state._specialChargeTime >= state._playerData.maxSpecialChargeTime)
                {
                    state._specialChargeTime = state._playerData.maxSpecialChargeTime;

                    state.ChangeState(new IdleState(state));
                    return;
                }
            }
            // �ʏ�`���[�W�̏ꍇ
            else
            {
                state._normalChargeTime++;

                // �ő�`���[�W���Ԃ𒴂�����`���[�W�U���ɑJ��
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
            // ����U����s�ɂ���
            state.isAbleToSpecialAttack = false;
            // ���݂�StateKind���ߐڍU���ɐݒ�
            state._stateKind = StateKind.CHARGEATTACK;

            // �`���[�W���Ԃ�50���ȏ�Ȃ狭�`���[�W�U���A�����łȂ���Ύ�`���[�W�U���ɐݒ�
            if (state._normalChargeTime >= state._playerData.maxChargeAttackTime / 2)
            {
                _currentAttackName = "ChargeAttack";
            }
            else
            {
                _currentAttackName = "LowChargeAttack";
            }

            // �A�j���[�V�������Đ�
            state._animator.SetTrigger(_currentAttackName);
            // �U���̏���ݒ�
            _currentAttackData = state.SearchAttackData(_currentAttackName);

            _currentFrame = 0;
        }
        public override void OnUpdate()
        {
            _currentFrame++;

            // �U���I�u�W�F�N�g�𐶐�����t���[���ɒB������U���I�u�W�F�N�g�𐶐�
            if (_currentFrame == _currentAttackData.startFrame)
            {
                state.CreateAttack(_currentAttackData);
                // �`���[�W���Ԃ����Z�b�g
                state._normalChargeTime = 0;
            }

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

    // ����U�����
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
            // ���s�\�ɂ���
            state._isAbleToDodge = false;
            // �U���̓��͂𖳌���
            state._isAbleToAttack = false;
            // ����U����s�ɂ���
            state.isAbleToSpecialAttack = false;
            // ���݂�StateKind���ߐڍU���ɐݒ�
            state._stateKind = StateKind.SPECIALATTACK;

            _stateTime = 0;

            // �`���[�W���Ԃ��ő�Ȃ狭����U���A�����łȂ���Ύ����U���ɐݒ�
            if (state._specialChargeTime == state._playerData.maxSpecialChargeTime)
            {
                _currentAttackName = "SpecialAttack";
            }
            else
            {
                _currentAttackName = "LowSpecialAttack";
            }

            Debug.Log(_currentAttackName);

            // ����U���A�j���[�V�������Đ�
            state._animator.SetTrigger(_currentAttackName);
            // �U���̏���ݒ�
            _currentAttackData = state.SearchAttackData(_currentAttackName);
        }
        public override void OnUpdate()
        {
            _stateTime++;

            // �U���I�u�W�F�N�g�𐶐�����t���[���ɒB������U���I�u�W�F�N�g�𐶐�
            if (_stateTime == _currentAttackData.startFrame)
            {
                state.CreateAttack(_currentAttackData);

                // TODO : �e�𐶐�����

                // �`���[�W���Ԃ����Z�b�g
                state._specialChargeTime = 0;

            }
            // �U���̃g�[�^���t���[���ɒB������
            if (_stateTime >= _currentAttackData.totalFrame)
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
            state._animator.ResetTrigger(_currentAttackName);
            // �`���[�W���Ԃ����Z�b�g
            state._specialChargeTime = 0;
        }
    }

    // �_���[�W���
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
            // �_���[�W���͉��s��
            state._isAbleToDodge = false;
            // �_���[�W���͍U���s��
            state._isAbleToAttack = false;
            // ��Ԃ��_���[�W�ɐݒ�
            state._stateKind = StateKind.DAMAGE;

            // �_���[�W�̎�ނŃX�^�����ԂƃA�j���[�V������ύX

            // ���_���[�W
            if (state._damageKind == DamageKind.LOW)
            {
                _stunDuration = state._damageData.lowStanTime;
                _knockbackTime = 0;
                _knockBackScale = 0;

                // �y�_���[�W�A�j���[�V�������Đ�
                _damageAnim = "LowHit";
            }
            // ���_���[�W
            else if (state._damageKind == DamageKind.MIDDLE)
            {
                _stunDuration = state._damageData.middleStanTime;
                _knockbackTime = state._damageData.middleKnockBackTime;
                _knockBackScale = state._damageData.middleKnockBackScale;

                // �_���[�W��O����󂯂����ǂ����ŃA�j���[�V������ς���
                if (state._isFrontDamage)
                {
                    _damageAnim = "FrontMiddleHit";
                }
                else
                {
                    _damageAnim = "BackMiddleHit";
                }
            }
            // ��_���[�W
            else if (state._damageKind == DamageKind.HEAVY)
            {
                _stunDuration = state._damageData.highStanTime;
                _knockbackTime = state._damageData.highKnockBackTime;
                _knockBackScale = state._damageData.highKnockBackScale;

                // �_���[�W��O����󂯂����ǂ����ŃA�j���[�V������ς���
                if (state._isFrontDamage)
                {
                    _damageAnim = "FrontHeavyHit";
                }
                else
                {
                    _damageAnim = "BackHeavyHit";
                }
            }
            // ���Ԃ̃J�E���g�����Z�b�g
            _stateTime = 0;

            // �_���[�W�A�j���[�V�������Đ�
            state._animator.SetTrigger(_damageAnim);
        }
        public override void OnUpdate()
        {
            // �X�^�����Ԃ��J�E���g�_�E��
            _stateTime++;

            // �m�b�N�o�b�N���ԓ��Ȃ�m�b�N�o�b�N������
            if (_stateTime <= _knockbackTime)
            {
                // �O��������̍U���Ȃ���Ƀm�b�N�o�b�N
                if (state._isFrontDamage)
                {
                    Vector3 knockbackVelocity = -state.transform.forward * _knockBackScale;
                    state._rigidbody.velocity = knockbackVelocity;
                }
                // ��납��̍U���Ȃ�O�Ƀm�b�N�o�b�N
                else
                {
                    Vector3 knockbackVelocity = state.transform.forward * _knockBackScale;
                    state._rigidbody.velocity = knockbackVelocity;
                }
            }
            else
            {
                // �m�b�N�o�b�N���Ԃ��I��������ړ��x�N�g�������Z�b�g
                state._rigidbody.velocity = Vector3.zero;
            }


            // �X�^�����Ԃ��I��������ҋ@��ԂɑJ��
            if (_stateTime >= _stunDuration)
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
            // �_���[�W�A�j���[�V�������~
            state._animator.ResetTrigger(_damageAnim);
        }
    }

    // ���S���
    public class DeadState : StateBase<PlayerState>
    {
        public DeadState(PlayerState next) : base(next)
        {
        }
        public override void OnEnterState()
        {
            // ��Ԃ����S�ɐݒ�
            state._stateKind = StateKind.DEAD;
            // ���S�A�j���[�V�������Đ�
            state._animator.SetTrigger("Dead");
            // �ړ��x�N�g�������Z�b�g
            state._rigidbody.velocity = Vector3.zero;
            // ���s�ɂ���
            state._isAbleToDodge = false;
            // �U���s�ɂ���
            state._isAbleToAttack = false;
            // ����U���s�ɂ���
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
        if (_isAbleToAttack && _stateKind != StateKind.MELEEATTACK)
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
        // ����U���`���[�W���Ȃ����U����ԂɑJ��
        if (_stateKind == StateKind.CHARGE && _isSpecialCharge)
        {
            // �A�j���[�V����������U���J�n�A�j���ł����
            if(_animator.GetCurrentAnimatorStateInfo(0).IsName("SpecialChargeStart"))
            {
                // �������Ȃ�
                return;
            }

            ChangeState(new SpecialAttackState(this));
        }
        // ����Q�[�W���ő�Ȃ����U����ԂɑJ��
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

    private Vector3 GetAttackPosition(string partName)
    {
        Vector3 result = Vector3.zero;

        // �������^�[��
        if (partName == "None") return result;

        // �U�����ʂ̃��O�̖��O���擾
        string rigName = null;

        for (int i = 0; i < _attackPartData.attackDataList.Count; i++)
        {
            if (_attackPartData.attackDataList[i].attackPartName == partName)
            {
                rigName = _attackPartData.attackDataList[i].objectRigName;
                break;
            }
        }

        // �������^�[��
        if (rigName == null) return result;

        Vector3 rigPos = Vector3.zero;

        // ���O��Transform���擾
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform t in allChildren)
        {
            if (t.name == rigName) // ���f���̃{�[����
            {
                rigPos = t.position;
            }
        }

        result = rigPos;
        return result;
    }

    private void CreateAttack(AttackData data)
    {
        // �Q�[���I�u�W�F�N�g�𐶐�
        GameObject attackObject = Instantiate(_attackObjectPrefab);

        // ���̓����蔻���ݒ�
        attackObject.GetComponent<SphereCollider>().radius = data.scale;

        // �U���̏���ݒ�
        attackObject.GetComponent<PlayerAttack>().SetAttackData(data);

        // �U���̈ʒu��ݒ�
        Vector3 position = GetAttackPosition(data.attackPart);

        // �U���̍��W��ݒ�
        attackObject.GetComponent<PlayerAttack>().SetPlayerPos(position);

        // ���炷�������Z
        Vector3 shiftVec = transform.forward * data.shiftPosZ;

        attackObject.transform.position = position + shiftVec;

        // �U���̌�����ݒ�
        attackObject.transform.forward = transform.forward;

        // �U���I�u�W�F�N�g��ۑ�
        _currentAttack = attackObject;
    }

    private Vector3 CalculateMoveDirection(Vector3 direction)
    {
        Vector3 moveDirection = direction;

        // �Œ�l����]������
        moveDirection = Quaternion.Euler(0, _playerData.moveDirAngle, 0) * moveDirection;

        return moveDirection;
    }
    public int GetMaxHp()
    {
        return _playerData.maxHp;
    }
    public  int GetNowHp()
    {
        return _nowHp;
    }
    public int GetMaxSpecialChargeTime()
    {
        return _playerData.maxSpecialChargeTime;
    }
    public int GetNowSpecialChargeTime()
    {
        return _specialChargeTime;
    }
    void OnTriggerEnter(Collider other)
    {
        // �G�̍U���ɓ���������_���[�W��ԂɑJ��
        if (other.gameObject.CompareTag("EnemyAttack"))
        {
            // ���S���̓_���[�W���󂯂Ȃ�
            if (_stateKind == StateKind.DEAD) return;
            // ��𒆂̓_���[�W���󂯂Ȃ�
            if (_stateKind == StateKind.DODGE) return;
            // ��e���̓_���[�W���󂯂Ȃ�
            if (_stateKind == StateKind.DAMAGE) return;

            // �U����O����󂯂����ǂ���
            Vector3 toEnemy = other.transform.position - transform.position;
            toEnemy.y = 0; // y�����𖳎����Đ����ʂł̕������l����
            toEnemy.Normalize();
            float dot = Vector3.Dot(transform.forward, toEnemy);
            if (dot > 0)
            {
                _isFrontDamage = true;

                // �����U����Heavy�Ȃ�U���̕���������
                if (_damageKind == DamageKind.HEAVY)
                {
                    transform.forward = toEnemy;
                    _currentDirection = toEnemy;
                }
            }
            else
            {
                _isFrontDamage = false;

                // �����U����Heavy�Ȃ�U���̕����Ƌt������
                if (_damageKind == DamageKind.HEAVY)
                {
                    transform.forward = -toEnemy;
                    _currentDirection = -toEnemy;
                }
            }

            // �ő嗭�߂̓���U�����s���Ă���Ƃ��̓_���[�W��ԂɑJ�ڂ��Ȃ�
            if (_stateKind == StateKind.SPECIALATTACK && _specialChargeTime == _playerData.maxSpecialChargeTime)
            {
                // �_���[�W���J�b�g����
                int damage = (int)((float)other.gameObject.GetComponent<ZangiAttack>().GetDamage() * _playerData.maxSpecialAttackDamegeCutRate);
            
                _nowHp -= damage;

                // HP��1�ȉ��ɂ��Ȃ�
                if (_nowHp <= 0)
                {
                    _nowHp = 1;
                }

            }
            else
            {
                // HP�����炷
                _nowHp -= other.gameObject.GetComponent<ZangiAttack>().GetDamage();

                // �_���[�W�̎�ނ��擾
                _damageKind = other.gameObject.GetComponent<ZangiAttack>().GetDamageKind();

                // �����蔻��ƍU���̓����蔻�肪�d�Ȃ����ʒu�ɃG�t�F�N�g�𐶐�
                Vector3 hitPosition = other.ClosestPoint(transform.position);

                // �_���[�W�G�t�F�N�g�𐶐�
                Instantiate(other.gameObject.GetComponent<ZangiAttack>().GetHitEffectPrefab(), hitPosition, Quaternion.identity);

                // HP��0�ȉ��Ȃ玀�S��ԂɑJ��
                if (_nowHp <= 0)
                {
                    _nowHp = 0;
                    ChangeState(new DeadState(this));
                    return;
                }
                else
                {
                    // �_���[�W��ԂɑJ��
                    ChangeState(new DamageState(this));
                }
            }
        }
    }
}

