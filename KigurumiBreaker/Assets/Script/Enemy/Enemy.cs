using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // ���݂̃X�e�[�g
    private IState _currentState;

    [SerializeField] public NavMeshAgent _agent; //NavMeshAgent�̎Q��
    [SerializeField] public GameObject _player;  //�v���C���[�̎Q��


    private void Start()
    {
        ChangeState(new IdleState(this));
    }

    private void Update()
    {
        // ���݂̃X�e�[�g���X�V
        _currentState?.Update();
    }

    public void ChangeState(IState newState)
    {
        _currentState?.End();   // ���݂̃X�e�[�g�𔲂���
        _currentState = newState;
        _currentState.Init();   // �V�����X�e�[�g�ɓ���
    }

}
