using UnityEngine;

public class AttackState : IState
{
    private Enemy _enemy;   //�G�̎Q��
    private float _timer;   //�^�C�}�[

    public AttackState(Enemy enemy) { _enemy = enemy; }   //�R���X�g���N�^��Enemy�̎Q�Ƃ��󂯎��

    public void Init()
    {
        _timer = 0.0f;
        Debug.Log("AttackState: Enter");
    }

    public void Update()
    {
        //�^�C�}�[��i�߂�
        _timer += Time.deltaTime;

        //�v���C���[���U��������ҋ@��Ԃ�
        if (_timer > 20.0f)
        {
            //��Ԃ�ύX����
            _enemy.ChangeState(new IdleState(_enemy));
            Debug.Log("AttackState: Change to IdleState");
        }

        Debug.Log("AttackState: Update");
    }

    public void End()
    {
        Debug.Log("AttackState: Exit");
    }

}
