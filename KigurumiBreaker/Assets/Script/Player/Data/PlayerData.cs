using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("移動速度")]
    [SerializeField] private float MoveSpeed;
    [Header("回避速度")]
    [SerializeField] private float DodgeSpeed;
    [Header("移動入力感知の閾値")]
    [SerializeField] private float MoveInputLength;
    [Header("回避で移動し始めるまでの時間")]
    [SerializeField] private float DodgeStartTime;
    [Header("回避時間")]
    [SerializeField] private int DodgeTime;
    [Header("回避クールタイム")]
    [SerializeField] private int DodgeCoolTime;
    [Header("最大体力")]
    [SerializeField] private int MaxHp;
    [Header("ため攻撃の発動までの時間")]
    [SerializeField] private int ChargeAttackTime;
    [Header("移動ベクトルの回転度")]
    [SerializeField] private float MoveDirAngle;
    [Header("特殊攻撃の最大のチャージ時間")]
    [SerializeField] private int MaxSpecialChargeTime;


    // 読み取り専用
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
