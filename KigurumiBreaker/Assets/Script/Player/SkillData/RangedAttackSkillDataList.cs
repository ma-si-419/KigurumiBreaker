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
    [Header("ダメージ増加量")]
    [SerializeField] private int DamageIncreaseAmount;
    [Header("速度倍率")]
    [SerializeField] private float SpeedRate;
    [Header("敵を吹き飛ばすかどうか")]
    [SerializeField] private bool IsKnockBack;
    [Header("追尾するかどうか")]
    [SerializeField] private bool IsHoming;
    [Header("追撃攻撃オブジェクト")]
    [SerializeField] private GameObject RangedAttackObject;
    [Header("相手にかけるデバフの種類")]
    [SerializeField] private ZangiMove.EnemyDebuff debuffType;

    public string skillName => SkillName;
    public float speedRate => SpeedRate;
    public bool isKnockBack => IsKnockBack;
    public bool isHoming => IsHoming;
    public GameObject rangedAttackObject => RangedAttackObject;
    public ZangiMove.EnemyDebuff DebuffType => debuffType;

}
