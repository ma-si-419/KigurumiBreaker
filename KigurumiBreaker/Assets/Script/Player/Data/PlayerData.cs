using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("�ړ����x")]
    [SerializeField] private float MoveSpeed;
    [Header("��𑬓x")]
    [SerializeField] private float DodgeSpeed;
    [Header("�ړ����͊��m��臒l")]
    [SerializeField] private float MoveInputLength;
    [Header("����ňړ����n�߂�܂ł̎���")]
    [SerializeField] private float DodgeStartTime;
    [Header("�������")]
    [SerializeField] private int DodgeTime;
    [Header("����N�[���^�C��")]
    [SerializeField] private int DodgeCoolTime;
    [Header("�ő�̗�")]
    [SerializeField] private int MaxHp;
    [Header("���ߍU���̔����܂ł̎���")]
    [SerializeField] private int ChargeAttackTime;
    [Header("�ړ��x�N�g���̉�]�x")]
    [SerializeField] private float MoveDirAngle;
    [Header("����U���̍ő�̃`���[�W����")]
    [SerializeField] private int MaxSpecialChargeTime;


    // �ǂݎ���p
    public float moveSpeed => MoveSpeed;
    public float dodgeSpeed => DodgeSpeed;
    public float moveInputLength => MoveInputLength;
    public float dodgeStartTime => DodgeStartTime;
    public int dodgeTime => DodgeTime;
    public int dodgeCoolTime => DodgeCoolTime;
    public int maxHp => MaxHp;
    public int chargeAttackTime => ChargeAttackTime;
    public float moveDirAngle => MoveDirAngle;
    public int maxSpecialChargeTime => MaxSpecialChargeTime;

}
