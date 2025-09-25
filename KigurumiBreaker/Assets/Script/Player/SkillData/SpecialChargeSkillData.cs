using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpecialChargeSkillDataList")]
public class SpecialChargeSkillDataList : ScriptableObject
{
    public List<SpecialChargeSkillData> specialChargeSkillDataList;
}
[System.Serializable]
public class SpecialChargeSkillData
{
    [Header("名前")]
    [SerializeField] private string SkillName;
    [Header("溜めている最中に出る攻撃オブジェクト")]
    [SerializeField] private GameObject ChargingAttackObject;
    [Header("ダメージ軽減率")]
    [SerializeField] private int DamageReductionRate;
    [Header("ダメージをくらったときノックバックしないならtrue")]
    [SerializeField] private bool IsNotKnockBack;
    [Header("溜めている最中敵の弾を跳ね返すかどうか")]
    [SerializeField] private bool IsReflect;
    [Header("溜め速度倍率")]
    [SerializeField] private float ChargeSpeedRate;

    public string skillName => SkillName;
    public GameObject chargingAttackObject => ChargingAttackObject;
    public int damageReductionRate => DamageReductionRate;
    public bool isNotKnockBack => IsNotKnockBack;
    public bool isReflect => IsReflect;
    public float chargeSpeedRate => ChargeSpeedRate;

}
