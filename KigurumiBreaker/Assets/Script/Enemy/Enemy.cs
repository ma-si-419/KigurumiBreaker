using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private IState _currentState;

    //public Transform Player;
    public float Speed = 3f;

    private void Init()
    {
        ChangeState(new IdleState(this));
    }

    private void Update()
    {
        _currentState?.Update();
    }

    public void ChangeState(IState newState)
    {
        _currentState?.End();   // ���݂̃X�e�[�g�𔲂���
        _currentState = newState;
        _currentState.Init();   // �V�����X�e�[�g�ɓ���
    }

    public void Attack()
    {
        Debug.Log("�G���U�������I");
        // �����ɍU������������
    }
}


//using UnityEngine;

//public class Enemy : MonoBehaviour
//{
//    private EnemyState currentState;

//    [Header("��{���")]
//    public Transform player;
//    public float chaseRange = 5f;
//    public float attackRange = 2f;
//    public int hp = 10;

//    [Header("�U���^�C�v")]
//    public EnemyAttackBase attackBehavior;

//    void Start()
//    {
//        // �����X�e�[�g��Idle
//        ChangeState(new IdleState(this));
//    }

//    void Update()
//    {
//        currentState?.Update();
//    }

//    public void ChangeState(EnemyState newState)
//    {
//        currentState?.Exit();
//        currentState = newState;
//        currentState?.Enter();
//    }

//    public void TakeDamage(int damage)
//    {
//        hp -= damage;
//        if (hp <= 0)
//        {
//            ChangeState(new DeadState(this));
//        }
//    }
//}
