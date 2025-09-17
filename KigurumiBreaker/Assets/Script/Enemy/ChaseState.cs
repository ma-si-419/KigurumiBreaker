using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IState
{
    private Enemy _enemy;        //�G�̎Q��
    private float _timer;        //�^�C�}�[


    public ChaseState(Enemy enemy) 
    {
        //�R���X�g���N�^��Enemy�̎Q�Ƃ��󂯎��
        _enemy = enemy;
        _enemy.agent.isStopped = false; //�ǐՂ��~

    }

    public void Init()
    {
        _timer = 0.0f;
        Debug.Log("ChaseState: Init");
    }

    public void Update()
    {
        //�^�C�}�[��i�߂�
        _timer += Time.deltaTime;
        Debug.Log("ChaseState: Update");

        _enemy.agent.SetDestination(_enemy.player.transform.position); //�v���C���[�̈ʒu��ړI�n�ɐݒ�

        //�U�������ɓ���ƍU����Ԃ�
        if (_timer > 18.0f)
        {
            //��Ԃ�ύX����
            Debug.Log("ChaseState: Change to AttackState");
            _enemy.agent.isStopped = true; //�ǐՂ��~
            _enemy.ChangeState(new AttackState(_enemy));
        }
    }

    public void End()
    {
        Debug.Log("ChaseState: End");
    }

}


