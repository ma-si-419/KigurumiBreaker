using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("移動入力感知の閾値")]
    [SerializeField] private float MoveInputLength;
    [Header("1フレームで回転することができる角度")]
    [SerializeField] private float RotateAngle;
    [Header("回避で移動し始めるまでの時間")]
    [SerializeField] private float DodgeStartTime;
    [Header("回避時間")]
    [SerializeField] private int DodgeTime;
    [Header("回避クールタイム")]
    [SerializeField] private int DodgeCoolTime;
    [Header("回避中のスキルを出す間隔")]
    [SerializeField] private int DodgeSkillInterval;
    [Header("ため攻撃の発動までの時間")]
    [SerializeField] private int ChargeAttackTime;
    [Header("ため攻撃の最大溜め時間")]
    [SerializeField] private int MaxChargeAttackTime;
    [Header("ため攻撃の範囲を表示するときにずらすベクトル")]
    [SerializeField] private Vector3 ChargeAttackAreaShiftVector;
    [Header("移動ベクトルの回転度")]
    [SerializeField] private float MoveDirAngle;
    [Header("特殊攻撃の最大のチャージ時間")]
    [SerializeField] private float MaxSpecialChargeTime;
    [Header("特殊攻撃の最大溜め発動時のダメージカット率")]
    [SerializeField] private float MaxSpecialAttackDamegeCutRate;
    [Header("前方向とする角度")]
    [SerializeField] private float ForwardAngle;
    [Header("前方向の敵を認知する距離")]
    [SerializeField] private float ForwardDistance;

    // 読み取り専用
    public float moveInputLength => MoveInputLength;
    public float rotateAngle => RotateAngle;
    public float dodgeStartTime => DodgeStartTime;
    public int dodgeTime => DodgeTime;
    public int dodgeSkillInterval => DodgeSkillInterval;
    public int dodgeCoolTime => DodgeCoolTime;
    public int chargeAttackTime => ChargeAttackTime;
    public int maxChargeAttackTime => MaxChargeAttackTime;
    public Vector3 chargeAttackAreaShiftVector => ChargeAttackAreaShiftVector;
    public float moveDirAngle => MoveDirAngle;
    public float maxSpecialChargeTime => MaxSpecialChargeTime;
    public float maxSpecialAttackDamegeCutRate => MaxSpecialAttackDamegeCutRate;
    public float forwardAngle => ForwardAngle;
    public float forwardDistance => ForwardDistance;
}
