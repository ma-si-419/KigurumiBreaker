using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("ˆÚ“®“ü—ÍŠ´’m‚Ìè‡’l")]
    [SerializeField] private float MoveInputLength;
    [Header("1ƒtƒŒ[ƒ€‚Å‰ñ“]‚·‚é‚±‚Æ‚ª‚Å‚«‚éŠp“x")]
    [SerializeField] private float RotateAngle;
    [Header("‰ñ”ð‚ÅˆÚ“®‚µŽn‚ß‚é‚Ü‚Å‚ÌŽžŠÔ")]
    [SerializeField] private float DodgeStartTime;
    [Header("‰ñ”ðŽžŠÔ")]
    [SerializeField] private int DodgeTime;
    [Header("‰ñ”ðƒN[ƒ‹ƒ^ƒCƒ€")]
    [SerializeField] private int DodgeCoolTime;
    [Header("‰ñ”ð’†‚ÌƒXƒLƒ‹‚ðo‚·ŠÔŠu")]
    [SerializeField] private int DodgeSkillInterval;
    [Header("‚½‚ßUŒ‚‚Ì”­“®‚Ü‚Å‚ÌŽžŠÔ")]
    [SerializeField] private int ChargeAttackTime;
    [Header("‚½‚ßUŒ‚‚ÌÅ‘å—­‚ßŽžŠÔ")]
    [SerializeField] private int MaxChargeAttackTime;
    [Header("‚½‚ßUŒ‚‚Ì”ÍˆÍ‚ð•\Ž¦‚·‚é‚Æ‚«‚É‚¸‚ç‚·ƒxƒNƒgƒ‹")]
    [SerializeField] private Vector3 ChargeAttackAreaShiftVector;
    [Header("ˆÚ“®ƒxƒNƒgƒ‹‚Ì‰ñ“]“x")]
    [SerializeField] private float MoveDirAngle;
    [Header("“ÁŽêUŒ‚‚ÌÅ‘å‚Ìƒ`ƒƒ[ƒW—Ê")]
    [SerializeField] private float MaxSpecialChargeGauge;
    [Header("“ÁŽêUŒ‚‚Ì’iŠK”")]
    [SerializeField] private int SpecialAttackMaxLevel;
    [Header("“G‚Ìƒ_ƒ[ƒW‚Ì‰½Š„‚ð“ÁŽêUŒ‚‚ÌƒQ[ƒW‚É‰ÁŽZ‚·‚é‚©")]
    [SerializeField, Range(0.0f, 2.0f)] private float SpecialAttackChargeRate;
    [Header("‘O•ûŒü‚Æ‚·‚éŠp“x")]
    [SerializeField] private float ForwardAngle;
    [Header("‘O•ûŒü‚Ì“G‚ð”F’m‚·‚é‹——£")]
    [SerializeField] private float ForwardDistance;
    [Header("ƒ`ƒƒ[ƒWUŒ‚‚ÅUŒ‚‚·‚é•”ˆÊ‚ðŠg‘å‚·‚é‚Æ‚«‚É‚©‚¯‚éŽžŠÔ")]
    [SerializeField] private float ChargeAttackPartScaleUpTime;
    [Header("‰ñ”ð‚â”í’eŽž“™‚ÉŠg‘å‚µ‚Ä‚¢‚½•”ˆÊ‚ð1ƒtƒŒ[ƒ€‚Åk¬‚·‚é‘å‚«‚³")]
    [SerializeField] private float ChargeAttackPartScaleDownRatePerFrame;
    [Header("Ž€–SŽž‚ÌƒXƒ[ŽžŠÔ")]
    [SerializeField] private float DeathSlowTime;
    [Header("Ž€–SŽž‚ÌƒXƒ[‚Ìƒ^ƒCƒ€ƒXƒP[ƒ‹")]
    [SerializeField] private float DeathTimeScale;

    // “Ç‚ÝŽæ‚èê—p
    public float moveInputLength => MoveInputLength;
    public float rotateAngle => RotateAngle;
    public float dodgeStartTime => DodgeStartTime;
    public int dodgeTime => DodgeTime;
    public int dodgeSkillInterval => DodgeSkillInterval;
    public int dodgeCoolTime => DodgeCoolTime;
    public int chargeAttackTime => ChargeAttackTime;
    public int maxChargeAttackTime => MaxChargeAttackTime;
    public int specialAttackMaxLevel => SpecialAttackMaxLevel;
    public float specialAttackChargeRate => SpecialAttackChargeRate;
    public Vector3 chargeAttackAreaShiftVector => ChargeAttackAreaShiftVector;
    public float moveDirAngle => MoveDirAngle;
    public float maxSpecialChargeNum => MaxSpecialChargeGauge;
    public float forwardAngle => ForwardAngle;
    public float forwardDistance => ForwardDistance;
    public float chargeAttackPartScaleUpTime => ChargeAttackPartScaleUpTime;
    public float chargeAttackPartScaleDownRatePerFrame => ChargeAttackPartScaleDownRatePerFrame;
    public float deathSlowTime => DeathSlowTime;
    public float deathTimeScale => DeathTimeScale;

}
