using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    
    /* �ϐ� */
    private IState _currentState;               // ���݂̃X�e�[�g
    [SerializeField] public NavMeshAgent agent; // NavMeshAgent�̎Q��
    [SerializeField] public GameObject player;  // �v���C���[�̎Q��
    public string targetTag = "Player";         // �v���C���[�̃^�O
    public Transform target;         // �v���C���[��Transform


    private void Start()
    {
        ChangeState(new IdleState(this));
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
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

