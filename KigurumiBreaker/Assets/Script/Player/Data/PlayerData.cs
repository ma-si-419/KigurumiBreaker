using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("ˆÚ“®‘¬“x")]
    [SerializeField] private float MoveSpeed;
    [Header("‰ñ”ð‘¬“x")]
    [SerializeField] private float DodgeSpeed;
    [Header("ˆÚ“®“ü—ÍŠ´’m‚Ìè‡’l")]
    [SerializeField] private float MoveInputLength;
    [Header("‰ñ”ð‚ÅˆÚ“®‚µŽn‚ß‚é‚Ü‚Å‚ÌŽžŠÔ")]
    [SerializeField] private float DodgeStartTime;
    [Header("‰ñ”ðŽžŠÔ")]
    [SerializeField] private int DodgeTime;
    [Header("‰ñ”ðƒN[ƒ‹ƒ^ƒCƒ€")]
    [SerializeField] private int DodgeCoolTime;
    [Header("Å‘å‘Ì—Í")]
    [SerializeField] private int MaxHp;
    [Header("‚½‚ßUŒ‚‚Ì”­“®‚Ü‚Å‚ÌŽžŠÔ")]
    [SerializeField] private int ChargeAttackTime;
    [Header("ˆÚ“®ƒxƒNƒgƒ‹‚Ì‰ñ“]“x")]
    [SerializeField] private float MoveDirAngle;
    [Header("ŽãUŒ‚‚ðŽó‚¯‚½Žž‚Ìd’¼ŽžŠÔ")]
    [SerializeField] private int LowStanTime;
    [Header("’†UŒ‚‚ðŽó‚¯‚½Žž‚Ìd’¼ŽžŠÔ")]
    [SerializeField] private int MiddleStanTime;
    [Header("’†UŒ‚‚ðŽó‚¯‚½Žž‚ÌƒmƒbƒNƒoƒbƒN‚Ì‘å‚«‚³")]
    [SerializeField] private int MiddleKnockBackScale;
    [Header("’†UŒ‚‚ðŽó‚¯‚½‚ÉƒmƒbƒNƒoƒbƒN‚·‚éŽžŠÔ")]
    [SerializeField] private int MiddleKnockBackTime;
    [Header("‹­UŒ‚‚ðŽó‚¯‚½Žž‚Ìd’¼ŽžŠÔ")]
    [SerializeField] private int HighStanTime;
    [Header("‹­UŒ‚‚ðŽó‚¯‚½Žž‚ÌƒmƒbƒNƒoƒbƒN‚Ì‘å‚«‚³")]
    [SerializeField] private int HighKnockBackScale;
    [Header("‹­UŒ‚‚ðŽó‚¯‚½‚ÉƒmƒbƒNƒoƒbƒN‚·‚éŽžŠÔ")]
    [SerializeField] private int HighKnockBackTime;


    // “Ç‚ÝŽæ‚èê—p
    public float moveSpeed => MoveSpeed;
    public float dodgeSpeed => DodgeSpeed;
    public float moveInputLength => MoveInputLength;
    public float dodgeStartTime => DodgeStartTime;
    public int dodgeTime => DodgeTime;
    public int dodgeCoolTime => DodgeCoolTime;
    public int maxHp => MaxHp;
    public int chargeAttackTime => ChargeAttackTime;
    public float moveDirAngle => MoveDirAngle;
    public int lowStanTime => LowStanTime;
    public int middleStanTime => MiddleStanTime;
    public int middleKnockBackScale => MiddleKnockBackScale;
    public int middleKnockBackTime => MiddleKnockBackTime;
    public int highStanTime => HighStanTime;
    public int highKnockBackScale => HighKnockBackScale;
    public int highKnockBackTime => HighKnockBackTime;

}
