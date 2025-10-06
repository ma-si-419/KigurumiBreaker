using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PassiveSkillDataList")]
public class PassiveSkillDataList : ScriptableObject
{
    public List<PassiveSkillData> passiveSkillDataList;
}
[System.Serializable]
public class PassiveSkillData
{
    [Header("名前")]
    [SerializeField] private string SkillName;
    [Header("最大体力増加値")]
    [SerializeField] private int MaxHpAddNum;
    [Header("通常攻撃のダメージ上昇量(%)")]
    [SerializeField] private int LowAttackDamageAddRate;
    [Header("溜め攻撃のダメージ上昇量(%)")]
    [SerializeField] private int ChargeAttackDamageAddRate;
    [Header("遠距離攻撃のダメージ上昇量(%)")]
    [SerializeField] private int RangedAttackDamageAddRate;
    [Header("遠距離攻撃の弾数増加量(個数)")]
    [SerializeField] private int RangedAttackBulletAddNum;
    [Header("移動速度上昇量(%)")]
    [SerializeField] private int MoveSpeedAddRate;
    [Header("ダッシュ回数増加量(回数)")]
    [SerializeField] private int DashCountAddNum;
    [Header("ダメージカット率上昇量(%)")]
    [SerializeField] private int DamageCutRateAddRate;
    [Header("回避率上昇量(%)")]
    [SerializeField] private int DodgeRateAddRate;

    public string skillName => SkillName;
    public int maxHpAddNum => MaxHpAddNum;
    public int lowAttackDamageAddRate => LowAttackDamageAddRate;
    public int chargeAttackDamageAddRate => ChargeAttackDamageAddRate;
    public int rangedAttackDamageAddRate => RangedAttackDamageAddRate;
    public int rangedAttackBulletAddNum => RangedAttackBulletAddNum;
    public int moveSpeedAddRate => MoveSpeedAddRate;
    public int dashCountAddNum => DashCountAddNum;
    public int damageCutRateAddRate => DamageCutRateAddRate;
    public int dodgeRateAddRate => DodgeRateAddRate;
}
