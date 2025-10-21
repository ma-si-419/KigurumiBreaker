using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class BossAttackDataList : ScriptableObject
//{
//    public List<BossAttackData> bossAttackDataList;
//}

[CreateAssetMenu(menuName = "BossAttackData")]
//[System.Serializable]
public class BossAttackData : ScriptableObject
{
    
    public enum BossAttackType
    {
        MeleeAttack,    // 近接攻撃
        SpecialAttack,  // 特殊攻撃
        RangedAttack,   // 遠距離攻撃
        ChargeAttack,   // 範囲攻撃
    }

    [Header("攻撃の名前")]
    [SerializeField] private string AttackName;
    [Header("前に進む速度")]
    [SerializeField] private float MoveSpeed;
    [Header("攻撃判定の大きさ")]
    [SerializeField] private float Scale;
    [Header("攻撃判定を前方向にずらす大きさ")]
    [SerializeField] private float ShiftPosZ;
    [Header("攻撃を出す部位")]
    [SerializeField] private string AttackPart;
    [Header("攻撃を出す部位に出すエフェクト")]
    [SerializeField] private GameObject AttackEffect;
    [Header("エフェクトをボスから離す距離")]
    [SerializeField] private float EffectShiftScale;
    [Header("攻撃があたった時に出すエフェクト")]
    [SerializeField] private GameObject HitEffect;
    [Header("通常攻撃の範囲")]
    [SerializeField] private float MeleeAttackRange;
    [Header("特殊攻撃の範囲")]
    [SerializeField] private float SpecialAttackRange;


    // 読み取り専用プロパティ
    public string attackName => AttackName;
    public float moveSpeed => MoveSpeed;
    public float scale => Scale;
    public float shiftPosZ => ShiftPosZ;
    public string attackPart => AttackPart;
    public GameObject attackEffect => AttackEffect;
    public float effectShiftScale => EffectShiftScale;
    public GameObject hitEffect => HitEffect;
    public float meleeAttackRange => MeleeAttackRange;
    public float specialAttackRange => SpecialAttackRange;



    //public BossAttackType attackKind => AttackKind;
}
