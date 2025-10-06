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
    [SerializeField] private float LowAttackDamageAddRate;
    [Header("溜め攻撃のダメージ上昇量(%)")]
    [SerializeField] private float ChargeAttackDamageAddRate;
    [Header("遠距離攻撃のダメージ上昇量(%)")]
    [SerializeField] private float RangedAttackDamageAddRate;
    [Header("遠距離攻撃の弾数増加量(個数)")]
    [SerializeField] private int RangedAttackBulletAddNum;
    [Header("移動速度上昇量(%)")]
    [SerializeField] private float MoveSpeedAddRate;
    [Header("ダッシュ回数増加量(回数)")]
    [SerializeField] private int DashCountAddNum;
    [Header("ダメージカット率上昇量(%)")]
    [SerializeField] private float DamageCutRateAddRate;
    [Header("回避率上昇量(%)")]
    [SerializeField] private float DodgeRateAddRate;

    public string skillName => SkillName;
    public int maxHpAddNum => MaxHpAddNum;
    public float lowAttackDamageAddRate => LowAttackDamageAddRate;
    public float chargeAttackDamageAddRate => ChargeAttackDamageAddRate;
    public float rangedAttackDamageAddRate => RangedAttackDamageAddRate;
    public int rangedAttackBulletAddNum => RangedAttackBulletAddNum;
    public float moveSpeedAddRate => MoveSpeedAddRate;
    public int dashCountAddNum => DashCountAddNum;
    public float damageCutRateAddRate => DamageCutRateAddRate;
    public float dodgeRateAddRate => DodgeRateAddRate;
}
