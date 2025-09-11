using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player<T> : MonoBehaviour where T : Player<T>
{
    // �ړ����͂�臒l
    protected const float MOVE_INPUT_LENGTH = 0.1f;
    // ����̎���
    protected const int DODGE_TIME = 40;


    //�ЂƂO�̏��
    private StateBase<T> _currentState;

    //���̏��
    private StateBase<T> _nextState;

    /// <summary>
    /// ��Ԃ�ύX����֐�
    /// </summary>
    /// <param name="next"></param>
    /// <returns></returns>
    public bool ChangeState(StateBase<T> next)
    {
        if (next == null) return false;

        _nextState = next;
        return true;
    }

    //�X�V
    private void FixedUpdate()
    {

        //���̏�Ԃ��ݒ肢��Ƃ�
        if (_nextState != null)
        {
            //���݂̏�Ԃ�����ΏI������
            if (_currentState != null)
            {
                _currentState.OnExitState();
            }

            //���݂̏�Ԃ�null�ɂ���
            _currentState = null;

            //���݂̏�Ԃ����̏�Ԃɐݒ�
            _currentState = _nextState;
            //���̏�Ԃ�null�ɂ���
            _nextState = null;

            //�J�n�������s��
            _currentState.OnEnterState();
        }

        //���݂̏�Ԃ�����΍X�V����
        if (_currentState != null)
        {
            //�X�V�������s��
            _currentState.OnUpdate();
        }
    }
}
