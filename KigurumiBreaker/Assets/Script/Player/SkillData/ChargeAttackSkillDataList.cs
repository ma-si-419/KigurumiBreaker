using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/ChargeAttackSkillDataList")]
public class ChargeAttackSkillDataList : ScriptableObject
{
    public List<ChargeAttackSkillData> chargeAttackSkillDataList;
}
[System.Serializable]
public class ChargeAttackSkillData
{
    [Header("名前")]
    [SerializeField] private string SkillName;
    [Header("属性")]
    [SerializeField] private SkillData.SkillElement SkillElement;
    [Header("ダメージ増加率(%)")]
    [SerializeField] private int DamageAddRate;
    [Header("攻撃範囲増加率(%)")]
    [SerializeField] private int AttackRangeAddRate;
    [Header("溜め速度倍率")]
    [SerializeField] private float ChargeSpeedRate;
    [Header("ノックバック力")]
    [SerializeField] private float AddKnockBackPower;
    [Header("相手に与えるデバフの種類")]
    [SerializeField] private Enemy.EnemyDebuff DebuffType;
    [Header("追撃の攻撃プレハブ")]
    [SerializeField] private GameObject ChaseAttack;
    [Header("攻撃を跳ね返すかどうか")]
    [SerializeField] private bool IsReflect;
    [Header("スキルの説明文")]
    [TextArea][SerializeField] private string SkillContents;

    public string skillName => SkillName;
    public SkillData.SkillElement skillElement => SkillElement;
    public int damageAddRate => DamageAddRate;
    public int attackRangeAddRate => AttackRangeAddRate;
    public float addKnockBackPower => AddKnockBackPower;
    public Enemy.EnemyDebuff debuffType => DebuffType;
    public GameObject chaseAttack => ChaseAttack;
    public float chargeSpeedRate => ChargeSpeedRate;
    public bool isReflect => IsReflect;
    public string skillContents => SkillContents;

}
