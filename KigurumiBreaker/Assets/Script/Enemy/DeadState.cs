//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class DeadState : IState
//{
//    private EnemyBase _enemy;   //�G�̎Q��
//    private float _timer;   //�^�C�}�[

//    public DeadState(EnemyBase enemy) { _enemy = enemy; }   //�R���X�g���N�^��Enemy�̎Q�Ƃ��󂯎��

//    public void Init()
//    {
//        _timer = 0.0f;
//        Debug.Log("AttackState: Enter");
//    }

//    public void Update()
//    {
//        //�^�C�}�[��i�߂�
//        _timer += Time.deltaTime;

//        //���񂾏��������Ƃ�


//        Debug.Log("AttackState: Update");
//    }

//    public void End()
//    {
//        Debug.Log("AttackState: Exit");
//    }
//}
