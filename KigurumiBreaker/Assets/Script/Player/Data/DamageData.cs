using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/DamageData")]
public class DamageData : ScriptableObject
{
    [Header("ŽãUŒ‚‚ðŽó‚¯‚½Žž‚Ìd’¼ŽžŠÔ")]
    [SerializeField] private int LowStanTime;
    [Header("ŽãUŒ‚‚ðŽó‚¯‚½Žž‚ÌƒqƒbƒgƒXƒgƒbƒvŽžŠÔ")]
    [SerializeField] private int LowHitStop;
    [Header("’†UŒ‚‚ðŽó‚¯‚½Žž‚Ìd’¼ŽžŠÔ")]
    [SerializeField] private int MiddleStanTime;
    [Header("’†UŒ‚‚ðŽó‚¯‚½Žž‚ÌƒmƒbƒNƒoƒbƒN‚Ì‘å‚«‚³")]
    [SerializeField] private int MiddleKnockBackScale;
    [Header("’†UŒ‚‚ðŽó‚¯‚½Žž‚ÉƒmƒbƒNƒoƒbƒN‚·‚éŽžŠÔ")]
    [SerializeField] private int MiddleKnockBackTime;
    [Header("’†UŒ‚‚ðŽó‚¯‚½Žž‚ÌƒqƒbƒgƒXƒgƒbƒvŽžŠÔ")]
    [SerializeField] private int MiddleHitStop;
    [Header("‹­UŒ‚‚ðŽó‚¯‚½Žž‚Ìd’¼ŽžŠÔ")]
    [SerializeField] private int HighStanTime;
    [Header("‹­UŒ‚‚ðŽó‚¯‚½Žž‚ÌƒmƒbƒNƒoƒbƒN‚Ì‘å‚«‚³")]
    [SerializeField] private int HighKnockBackScale;
    [Header("‹­UŒ‚‚ðŽó‚¯‚½Žž‚ÉƒmƒbƒNƒoƒbƒN‚·‚éŽžŠÔ")]
    [SerializeField] private int HighKnockBackTime;
    [Header("‹­UŒ‚‚ðŽó‚¯‚½Žž‚ÌƒqƒbƒgƒXƒgƒbƒvŽžŠÔ")]
    [SerializeField] private int HighHitStop;

    // “Ç‚ÝŽæ‚èê—p
    public int lowStanTime => LowStanTime;
    public int lowHitStop => LowHitStop;
    public int middleStanTime => MiddleStanTime;
    public int middleKnockBackScale => MiddleKnockBackScale;
    public int middleKnockBackTime => MiddleKnockBackTime;
    public int middleHitStop => MiddleHitStop;
    public int highStanTime => HighStanTime;
    public int highKnockBackScale => HighKnockBackScale;
    public int highKnockBackTime => HighKnockBackTime;
    public int highHitStop => HighHitStop;
}
