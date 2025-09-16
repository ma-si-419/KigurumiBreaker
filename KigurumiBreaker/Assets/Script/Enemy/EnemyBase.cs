//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// �X�e�[�g�p�^�[��
///// </summary>
//public interface IState
//{
//    //��Ԃɓ���Ƃ��ɌĂяo���֐�
//    void Init();
//    //���t���[���Ăяo���֐�
//    void Update();
//    //�I������Ƃ��ɌĂяo���֐�
//    void End();
//}

//public class StateMachine
//{
//    private IState _currentState;   //���݂̏��

//    //��Ԃ�ύX����֐�
//    public void ChangeState(IState newState)
//    {
//        //�V������ԂɕύX���A�������������Ăяo��
//        _currentState?.End();
//        _currentState = newState;
//        _currentState?.Init();
//    }
//    //���t���[���Ăяo���֐�
//    public void Update()
//    {
//        _currentState?.Update();
//    }
//}

///// <summary>
///// �X�g���e�W�[�p�^�[��
///// </summary>

//public interface IAttack
//{
//    //���s����֐�
//    void Execute(EnemyBase enemy);
//}

////�ߐڍU���֐�
//public class MeleeAttack : IAttack
//{
//    //���s����֐�
//    public void Execute(EnemyBase enemy)
//    {
//        Debug.Log("�ߐڍU��!");
//    }
//}

////�������U���֐�
//public class RangedAttack : IAttack
//{
//    //���s����֐�
//    public void Execute(EnemyBase enemy)
//    {
//        Debug.Log("�������U��!");
//    }
//}

////����U���֐�
//public class SpecialAttack : IAttack
//{
//    //���s����֐�
//    public void Execute(EnemyBase enemy)
//    {
//        Debug.Log("����U��!");
//    }
//}

///// <summary>
///// �G�̊��N���X
///// </summary>
//public abstract class EnemyBase : MonoBehaviour
//{
//    protected StateMachine _stateMachine;   //��ԃ}�V��
//    public Transform target;                //�^�[�Q�b�g(�v���C���[)
//    public float speed = 2.0f;              //�ړ����x

//    protected List<IAttack> _attacks = new List<IAttack>();   //�U�����@�̃��X�g

//    //������
//    protected virtual void Init()
//    {
//        _stateMachine = new StateMachine();     //��ԃ}�V���̏�����
//        //_stateMachine.ChangeState(new IdleState());  //������Ԃ�ҋ@��Ԃɐݒ�
//    }

//    //�X�V����
//    protected virtual void Update()
//    {
//        _stateMachine.Update();  //��ԃ}�V���̍X�V
//    }

//    //�U��
//    public virtual void Attack(int index)
//    {
//        if(index >= 0 && index < _attacks.Count)
//        {
//            _attacks[index].Execute(this);  //�w�肳�ꂽ�U�����@�����s
//        }
//    }


//}

///// <summary>
///// �X�e�[�g�p�^�[��
///// </summary>

//public class IdleState : IState
//{
//    private EnemyBase _enemy;   //�G�̎Q��
//    public IdleState(EnemyBase enemy) { _enemy = enemy; }   //�R���X�g���N�^��Enemy�̎Q�Ƃ��󂯎��

//    //�����֐�
//    public void Init()
//    {
//        Debug.Log("IdleState: Init");
//    }

//    //���t���[���Ăяo���֐�
//    public void Update()
//    {
//        Debug.Log("IdleState: Update");
//    }

//    //�G���h�֐�
//    public void End()
//    {
//        Debug.Log("IdleState: End");
//    }

//}

//public class ChaseState : IState
//{
//    private EnemyBase _enemy;   //�G�̎Q��

//    //�����֐�
//    public void Init()
//    {
//        Debug.Log("ChaseState: Init");
//    }

//    //���t���[���Ăяo���֐�
//    public void Update()
//    {
//        Debug.Log("ChaseState: Update");
//    }

//    //�G���h�֐�
//    public void End()
//    {
//        Debug.Log("ChaseState: End");
//    }

//}

//public class AttackState : IState
//{
//    private EnemyBase _enemy;   //�G�̎Q��

//    //�����֐�
//    public void Init()
//    {
//        Debug.Log("AttackState: Init");
//    }
//    //���t���[���Ăяo���֐�
//    public void Update()
//    {
//        Debug.Log("AttackState: Update");
//    }
//    //�G���h�֐�
//    public void End()
//    {
//        Debug.Log("AttackState: End");
//    }
//}

//public class DeadState : IState
//{
//    private EnemyBase _enemy;   //�G�̎Q��

//    //�����֐�
//    public void Init()
//    {
//        Debug.Log("DeadState: Init");
//    }
//    //���t���[���Ăяo���֐�
//    public void Update()
//    {
//        Debug.Log("DeadState: Update");
//    }
//    //�G���h�֐�
//    public void End()
//    {
//        Debug.Log("DeadState: End");
//    }
//}

///// <summary>
///// �G�̎��
///// </summary>

//public class  NormalEnemy : EnemyBase
//{
//    protected override void Init()
//    {
//        //������
//        base.Init();
//        //�U�����@�̌���
//        _attacks.Add(new MeleeAttack());
//    }
//}