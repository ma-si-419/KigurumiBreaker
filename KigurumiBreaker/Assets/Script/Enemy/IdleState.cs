//using UnityEngine;
//using UnityEngine.XR;

//public class IdleState : IState
//{
//    private EnemyBase _enemy;   //�G�̎Q��
//    private float _timer;   //�^�C�}�[

//    public IdleState(EnemyBase enemy) { _enemy = enemy; }   //�R���X�g���N�^��Enemy�̎Q�Ƃ��󂯎��

//    public void Init()
//    {
//        _timer = 0.0f;
//        Debug.Log("IdleState: Enter");
//    }

//    public void Update()
//    {
//        //�^�C�}�[��i�߂�
//        _timer += Time.deltaTime;

//        //�v���C���[�𔭌�������ǐՏ�Ԃ�
//        if (_timer > 20.0f)
//        {
//            //��Ԃ�ύX����
//            _enemy.ChangeState(new ChaseState(_enemy));
//            Debug.Log("IdleState: Change to ChaseState");
//        }


//        Debug.Log("IdleState: Update");
//    }

//    public void End()
//    {
//        Debug.Log("IdleState: Exit");
//    }

//}
