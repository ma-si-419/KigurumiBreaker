using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    
    /* �ϐ� */
    private IState _currentState;               // ���݂̃X�e�[�g
    [SerializeField] public NavMeshAgent agent; // NavMeshAgent�̎Q��
    [SerializeField] public GameObject player;  // �v���C���[�̎Q��
    public string targetTag = "Player";         // �v���C���[�̃^�O
    public Transform playerTrans { get; private set; }                    // �v���C���[��Transform


    private void Start()
    {
        playerTrans = GameObject.FindGameObjectWithTag(targetTag).transform;
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeState(new ChaseState(this));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ChangeState(new IdleState(this));
        }
    }

}

