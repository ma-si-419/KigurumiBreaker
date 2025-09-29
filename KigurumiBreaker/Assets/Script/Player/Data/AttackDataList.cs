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
    public enum AttackType
    {
        LowAttack,      // 通常攻撃
        ChargeAttack,   // 溜め攻撃
        RangedAttack,   // 遠距離攻撃
        SpecialAttack   // 特殊攻撃
    }



    [Header("名前")]
    [SerializeField] private string AttackName;
    [Header("ダメージ係数")]
    [SerializeField] private int Damage;
    [Header("発生フレーム")]
    [SerializeField] private int StartFrame;
    [Header("硬直フレーム")]
    [SerializeField] private int StunFrame;
    [Header("キャンセルフレーム")]
    [SerializeField] private int CancelFrame;
    [Header("トータルフレーム")]
    [SerializeField] private int TotalFrame;
    [Header("前に進む速度")]
    [SerializeField] private float MoveSpeed;
    [Header("攻撃判定の大きさ")]
    [SerializeField] private float Scale;
    [Header("攻撃判定を前方向にずらす大きさ")]
    [SerializeField] private float ShiftPosZ;
    [Header("攻撃判定の持続時間")]
    [SerializeField] private int AttackLifeTime;
    [Header("次に出てくる攻撃の名前(コンボ用)")]
    [SerializeField] private string NextAttackName;
    [Header("攻撃を出す部位")]
    [SerializeField] private string AttackPart;
    [Header("攻撃を出す部位に出すエフェクト")]
    [SerializeField] private GameObject AttackEffect;
    [Header("エフェクトをプレイヤーから離す距離")]
    [SerializeField] private float EffectShiftScale;
    [Header("攻撃があたった時に出すエフェクト")]
    [SerializeField] private GameObject HitEffect;
    [Header("攻撃の種類")]
    [SerializeField] private AttackType AttackKind;

    // 読み取り専用プロパティ

    public string attackName { get; }
    public int damage { get; set; }
    public int startFrame { get; }
    public int stunFrame { get; }
    public int cancelFrame { get; }
    public int totalFrame { get; }
    public float moveSpeed { get; }
    public float scale { get; set; }
    public float shiftPosZ { get; }
    public int attackLifeTime { get; }
    public string nextAttackName { get; }
    public string attackPart { get; }
    public GameObject attackEffect { get; }
    public float effectShiftScale { get; }
    public GameObject hitEffect { get; set; }
    public AttackType attackKind { get; }
}
