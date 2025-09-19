using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AttackDataList")]
public class AttackDataList : ScriptableObject
{
    public List<AttackData> attackDataList;
}
[System.Serializable]
public class AttackData
{
    [Header("���O")]
    [SerializeField] private string AttackName;
    [Header("�_���[�W�W��")]
    [SerializeField] private int Damage;
    [Header("�����t���[��")]
    [SerializeField] private int StartFrame;
    [Header("�d���t���[��")]
    [SerializeField] private int StunFrame;
    [Header("�L�����Z���t���[��")]
    [SerializeField] private int CancelFrame;
    [Header("�g�[�^���t���[��")]
    [SerializeField] private int TotalFrame;
    [Header("�O�ɐi�ޑ��x")]
    [SerializeField] private float MoveSpeed;
    [Header("�U������̑傫��")]
    [SerializeField] private float Scale;
    [Header("�U�������O�����ɂ��炷�傫��")]
    [SerializeField] private float ShiftPosZ;
    [Header("�U������̎�������")]
    [SerializeField] private int AttackLifeTime;
    [Header("���ɏo�Ă���U���̖��O(�R���{�p)")]
    [SerializeField] private string NextAttackName;
    [Header("�U�����o������")]
    [SerializeField] private string AttackPart;
    [Header("�U�����o�����ʂɏo���G�t�F�N�g")]
    [SerializeField] private GameObject AttackEffect;    
    [Header("�U���������������ɏo���G�t�F�N�g")]
    [SerializeField] private GameObject HitEffect;

    // �ǂݎ���p�v���p�e�B
    public string attackName => AttackName;
    public int damage => Damage;
    public int startFrame => StartFrame;
    public int stunFrame => StunFrame;
    public int cancelFrame => CancelFrame;
    public int totalFrame => TotalFrame;
    public float moveSpeed => MoveSpeed;
    public float scale => Scale;
    public float shiftPosZ => ShiftPosZ;
    public int attackLifeTime => AttackLifeTime;
    public string nextAttackName => NextAttackName;
    public string attackPart => AttackPart;
    public GameObject attackEffect => AttackEffect;
    public GameObject hitEffect => HitEffect;
}
