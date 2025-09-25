using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SkillData")]
public class SkillData : ScriptableObject
{
    [Header("通常攻撃のスキルリスト")]
    [SerializeField] private LowAttackSkillDataList lowAttackSkillDataList;
    [Header("チャージ攻撃のスキルリスト")]
    [SerializeField] private ChargeAttackSkillDataList chargeAttackSkillDataList;
    [Header("特殊チャージ中のスキルリスト")]
    [SerializeField] private SpecialChargeSkillDataList specialChargeSkillDataList;
    public LowAttackSkillDataList LowAttackSkillDataList => lowAttackSkillDataList;
    public ChargeAttackSkillDataList ChargeAttackSkillDataList => chargeAttackSkillDataList;
    public SpecialChargeSkillDataList SpecialChargeSkillDataList => specialChargeSkillDataList;
}
