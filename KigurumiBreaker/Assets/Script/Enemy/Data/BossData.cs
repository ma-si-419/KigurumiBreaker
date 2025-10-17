using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BossData")]
public class BossData : ScriptableObject
{
    [Header("“G‚ªƒXƒ^ƒ“‚·‚éÅ‘åƒ_ƒ[ƒW—Ê")]
    [SerializeField] private float MaxStunDamage;
    [Header("“G‚ÌUŒ‚‚·‚é”ÍˆÍ")]
    [SerializeField] private float AttackRange;

    // “Ç‚ÝŽæ‚èê—p
    public float stunHp => MaxStunDamage;
    public float attackRange => AttackRange;

}
