using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RangedAttackSkillDataList")]
public class RangedAttackSkillDataList : ScriptableObject
{
    public List<RangedAttackSkillData> rangedAttackSkillDataList;
}
[System.Serializable]
public class RangedAttackSkillData
{
    [Header("名前")]
    [SerializeField] private string SkillName;
    [Header("属性")]
    [SerializeField] private SkillData.SkillElement SkillElement;
    [Header("ダメージ増加量")]
    [SerializeField] private int DamageAddRate;
    [Header("速度倍率")]
    [SerializeField] private float SpeedRate;
    [Header("敵を吹き飛ばすかどうか")]
    [SerializeField] private bool IsKnockBack;
    [Header("追尾するかどうか")]
    [SerializeField] private bool IsHoming;
    [Header("追撃攻撃オブジェクト")]
    [SerializeField] private GameObject ChaseAttack;
    [Header("相手にかけるデバフの種類")]
    [SerializeField] private Enemy.EnemyDebuff DebuffType;
    [Header("スキルの説明文")]
    [TextArea] [SerializeField] private string SkillContents;

    public string skillName => SkillName;
    public SkillData.SkillElement skillElement => SkillElement;
    public int damageAddRate => DamageAddRate;
    public float speedRate => SpeedRate;
    public bool isKnockBack => IsKnockBack;
    public bool isHoming => IsHoming;
    public GameObject chaseAttack => ChaseAttack;
    public Enemy.EnemyDebuff debuffType => DebuffType;
    public string skillContents => SkillContents;

}
