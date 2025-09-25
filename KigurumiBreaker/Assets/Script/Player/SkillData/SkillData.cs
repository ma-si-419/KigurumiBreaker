using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SkillData")]
public class SkillData : ScriptableObject
{
    [Header("通常攻撃のスキルリスト")]
    [SerializeField] private LowAttackSkillDataList LowAttackSkill;
    [Header("チャージ攻撃のスキルリスト")]
    [SerializeField] private ChargeAttackSkillDataList ChargeAttackSkill;
    [Header("特殊チャージ中のスキルリスト")]
    [SerializeField] private SpecialChargeSkillDataList SpecialChargeSkill;
    [Header("遠距離攻撃のスキルリスト")]
    [SerializeField] private RangedAttackSkillDataList RangedAttackSkill;
    [Header("ダッシュのスキルリスト")]
    [SerializeField] private DashSkillDataList DashSkill;
    public LowAttackSkillDataList lowAttackSkill => LowAttackSkill;
    public ChargeAttackSkillDataList chargeAttackSkill => ChargeAttackSkill;
    public SpecialChargeSkillDataList specialChargeSkill => SpecialChargeSkill;
    public RangedAttackSkillDataList rangedAttackSkill => RangedAttackSkill;
    public DashSkillDataList dashSkill => DashSkill;
}
