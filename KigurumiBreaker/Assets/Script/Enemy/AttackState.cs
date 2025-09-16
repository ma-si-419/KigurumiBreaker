using UnityEngine;

public class AttackState : IState
{
    private Enemy _enemy;   //�G�̎Q��
    private float _timer;   //�^�C�}�[

    public AttackState(Enemy enemy) 
    {
        //�R���X�g���N�^��Enemy�̎Q�Ƃ��󂯎��
        _enemy = enemy;
    }  

    public void Init()
    {
        _timer = 0.0f;
        Debug.Log("AttackState: Init");
    }

    public void Update()
    {
        //�^�C�}�[��i�߂�
        _timer += Time.deltaTime;
        Debug.Log("AttackState: Update");

        //�v���C���[���U��������ҋ@��Ԃ�
        if (_timer > 10.0f)
        {
            //��Ԃ�ύX����
            Debug.Log("AttackState: Change to IdleState");
            _enemy.ChangeState(new IdleState(_enemy));
        }
    }

    public void End()
    {
        Debug.Log("AttackState: End");
    }

}
