using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[CreateAssetMenu(menuName = "EnemyData")]
public class EnemyDataList : ScriptableObject
{
    public List<EnemyData> lowAttackSkillDataList;
}

[System.Serializable]
public class EnemyData
{
    [Header("Å‘åHP")]
    [SerializeField] private float MaxHp;
    [Header("‘Ï‹v—Í")]
    [SerializeField] private float MaxTrunk;
    [Header("UŒ‚—Í")]
    [SerializeField] private float AttackPower;
    [Header("ˆÚ“®‘¬“x")]
    [SerializeField] private float MoveSpeed;
    [Header("“G‚Ìõ“G”ÍˆÍ")]
    [SerializeField] private float DetectionRange;
    [Header("“G‚ÌUŒ‚‚·‚é”ÍˆÍ")]
    [SerializeField] private float AttackRange;

    //public string skillName => SkillName;
    //public float damageAddRate => DamageAddRate;
    //public float attackRangeAddRate => AttackRangeAddRate;
    //public float addKnockBackPower => AddKnockBackPower;
    //public Enemy.EnemyDebuff debuffType => DebuffType;
    //public GameObject chaseAttack => ChaseAttack;
}
