//using UnityEngine;

//public class ChaseState : IState
//{
//    private EnemyBase _enemy;   //�G�̎Q��
//    private float _timer;   //�^�C�}�[

//    public ChaseState(EnemyBase enemy) { _enemy = enemy; }   //�R���X�g���N�^��Enemy�̎Q�Ƃ��󂯎��

//    public void Init()
//    {
//        _timer = 0.0f;
//        Debug.Log("ChaseState: Enter");
//    }

//    public void Update()
//    {
//        //�^�C�}�[��i�߂�
//        _timer += Time.deltaTime;

//        //�U�������ɓ���ƍU����Ԃ�
//        if (_timer > 20.0f)
//        {
//            //��Ԃ�ύX����
//            _enemy.ChangeState(new AttackState(_enemy));
//            Debug.Log("ChaseState: Change to AttackState");
//        }


//        Debug.Log("ChaseState: Update");
//    }

//    public void End()
//    {
//        Debug.Log("ChaseState: Exit");
//    }



//}
