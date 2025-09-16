using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IState
{
    private Enemy _enemy;   //�G�̎Q��
    private float _timer;   //�^�C�}�[

    public DeadState(Enemy enemy)
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

        //���񂾏��������Ƃ�

    }

    public void End()
    {
        Debug.Log("AttackState: End");
    }
}
