using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/SpecialChargeSkillDataList")]
public class SpecialChargeSkillDataList : ScriptableObject
{
    public List<SpecialChargeSkillData> specialChargeSkillDataList;
}
[System.Serializable]
public class SpecialChargeSkillData
{
    [Header("名前")]
    [SerializeField] private string SkillName;
    [Header("属性")]
    [SerializeField] private SkillData.SkillElement SkillElement;
    [Header("溜めている最中に出る攻撃オブジェクト")]
    [SerializeField] private GameObject ChargingAttackObject;
    [Header("ダメージ軽減率")]
    [SerializeField] private int DamageReductionRate;
    [Header("ダメージをくらったときノックバックしないならtrue")]
    [SerializeField] private bool IsNotKnockBack;
    [Header("何フレームごとに攻撃を出すか")]
    [SerializeField] private int AttackIntervalFrame;
    [Header("溜め速度倍率")]
    [SerializeField] private float ChargeSpeedRate;
    [Header("スキル説明文")]
    [TextArea] [SerializeField] private string SkillContents;

    public string skillName => SkillName;
    public SkillData.SkillElement skillElement => SkillElement;
    public GameObject chargingAttackObject => ChargingAttackObject;
    public int damageReductionRate => DamageReductionRate;
    public bool isNotKnockBack => IsNotKnockBack;
    public int attackIntervalFrame => AttackIntervalFrame;
    public float chargeSpeedRate => ChargeSpeedRate;
    public string skillContents => SkillContents;

}
