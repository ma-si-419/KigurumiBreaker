using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BossAttackData")]
public class BossAttackDataList : ScriptableObject
{
    public List<BossAttackData> bossAttackDataList;
}

[System.Serializable]
public class BossAttackData
{
    
    public enum BossAttackType
    {
        MeleeAttack,    // 近接攻撃
        TackleAttack,   // 突進攻撃
        RangedAttack,   // 遠距離攻撃
        ChargeAttack,   // 範囲攻撃
    }


    [Header("攻撃の名前")]
    [SerializeField] private string AttackName;
    [Header("ダメージ量")]
    [SerializeField] private int Damage;
    [Header("発声フレーム")]
    [SerializeField] private int StartFrame;
    [Header("硬直フレーム")]
    [SerializeField] private int StunFrame;
    [Header("前に進む速度")]
    [SerializeField] private float MoveSpeed;
    [Header("攻撃判定の大きさ")]
    [SerializeField] private float Scale;
    [Header("攻撃判定を前方向にずらす大きさ")]
    [SerializeField] private float ShiftPosZ;
    [Header("攻撃判定の持続時間")]
    [SerializeField] private int AttackLifeTime;
    [Header("次に出す攻撃の名前(コンボ用)")]
    [SerializeField] private string NextAttackName;
    [Header("攻撃を出す部位")]
    [SerializeField] private string AttackPart;
    [Header("攻撃を出す部位に出すエフェクト")]
    [SerializeField] private GameObject AttackEffect;
    [Header("エフェクトをボスから離す距離")]
    [SerializeField] private float EffectShiftScale;
    [Header("攻撃があたった時に出すエフェクト")]
    [SerializeField] private GameObject HitEffect;
    [Header("攻撃の種類")]
    [SerializeField] private BossAttackType AttackKind;

    // 読み取り専用プロパティ
    public string attackName => AttackName;
    public int damage => Damage;
    public int startFrame => StartFrame;
    public int stunFrame => StunFrame;
    public float moveSpeed => MoveSpeed;
    public float scale => Scale;
    public float shiftPosZ => ShiftPosZ;
    public int attackLifeTime => AttackLifeTime;
    public string nextAttackName => NextAttackName;
    public string attackPart => AttackPart;
    public GameObject attackEffect => AttackEffect;
    public float effectShiftScale => EffectShiftScale;
    public GameObject hitEffect => HitEffect;
    public BossAttackType attackKind => AttackKind;

}
