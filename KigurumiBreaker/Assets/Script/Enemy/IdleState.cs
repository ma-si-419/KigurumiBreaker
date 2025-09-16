using UnityEngine;

public class IdleState : IState
{
    private Enemy _enemy;   //�G�̎Q��
    private float _timer;   //�^�C�}�[
    

    public IdleState(Enemy enemy) 
    {
        //�R���X�g���N�^��Enemy�̎Q�Ƃ��󂯎��
        _enemy = enemy; 
    }   

    public void Init()
    {
        Debug.Log("IdleState: Init");
        _timer = 0.0f;
    }

    public void Update()
    {
        //�^�C�}�[��i�߂�
        _timer += Time.deltaTime;
        Debug.Log("IdleState: Update");

        //�v���C���[�𔭌�������ǐՏ�Ԃ�
        if (_timer > 10.0f)
        {
            //��Ԃ�ύX����
            Debug.Log("IdleState: Change to ChaseState");
            _enemy.ChangeState(new ChaseState(_enemy));
        }

    }

    public void End()
    {
        Debug.Log("IdleState: End");
    }

}

