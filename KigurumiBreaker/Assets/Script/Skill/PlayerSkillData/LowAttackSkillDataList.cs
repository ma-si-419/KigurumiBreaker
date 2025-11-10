using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/LowAttackSkillDataList")]
public class LowAttackSkillDataList : ScriptableObject
{
    public List<LowAttackSkillData> lowAttackSkillDataList;
}
[System.Serializable]
public class LowAttackSkillData
{
    [Header("名前")]
    [SerializeField] private string SkillName;
    [Header("属性")]
    [SerializeField] private SkillData.SkillElement SkillElement;
    [Header("ダメージ増加率(%)")]
    [SerializeField] private float DamageAddRate;
    [Header("攻撃範囲増加率(%)")]
    [SerializeField] private float AttackRangeAddRate;
    [Header("ノックバック力")]
    [SerializeField] private float AddKnockBackPower;
    [Header("相手に与えるデバフの種類")]
    [SerializeField] private Enemy.EnemyDebuff DebuffType;
    [Header("追撃の攻撃プレハブ")]
    [SerializeField] private GameObject ChaseAttack;
    [Header("スキルの説明文")]
    [TextArea] [SerializeField] private string SkillContents;

    public string skillName => SkillName;
    public SkillData.SkillElement skillElement => SkillElement;
    public float damageAddRate => DamageAddRate;
    public float attackRangeAddRate => AttackRangeAddRate;
    public float addKnockBackPower => AddKnockBackPower;
    public Enemy.EnemyDebuff debuffType => DebuffType;
    public GameObject chaseAttack => ChaseAttack;
    public string skillContents => SkillContents;
}
