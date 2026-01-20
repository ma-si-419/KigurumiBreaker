using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[CreateAssetMenu(menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("‹­“G‚©‚Ç‚¤‚©‚Ìƒtƒ‰ƒO")]
    [SerializeField] private bool IsStrongEnemy;
    [Header("ƒA[ƒ}[‘•”õƒtƒ‰ƒO")]
    [SerializeField] private bool IsArmor;
    [Header("Å‘åHP")]
    [SerializeField] private float MaxHp;
    [Header("‘Ï‹v—Í")]
    [SerializeField] private float MaxTrunk;
    [Header("ƒvƒŒƒCƒ„[‚É—^‚¦‚éƒ_ƒ[ƒW—Ê")]
    [SerializeField] private float AttackPower;
    [Header("ˆÚ“®‘¬“x")]
    [SerializeField] private float MoveSpeed;
    [Header("“G‚Ìõ“G”ÍˆÍ")]
    [SerializeField] private float DetectionRange;
    [Header("“G‚ÌUŒ‚‚·‚é”ÍˆÍ")]
    [SerializeField] private float AttackRange;
    [Header("“G‚ª‚Ç‚¿‚ç‚ÌUŒ‚‚ð‚Æ‚é‚©‚Ì”ÍˆÍ(UŒ‚‚ª•¡”‚Ì“G‚Ì‚Ý)")]
    [SerializeField] private float AttackSwitchRange;
    [Header("‘Ò‹@ó‘Ô‚©‚ç’ÇÕó‘Ô‚É‘JˆÚ‚·‚é‚Ü‚Å‚ÌŽžŠÔ")]
    [SerializeField] private float IdleToChaseTime;
    [Header("’ÇÕó‘Ô‚©‚çUŒ‚ó‘Ô‚É‘JˆÚ‚·‚é‚Ü‚Å‚ÌŽžŠÔ")]
    [SerializeField] private float ChaseToAttack;
    [Header("UŒ‚”»’è‚ð¶¬‚·‚éŽžŠÔ")]
    [SerializeField] private float MaxAttackTime;
    [Header("“G‚Ì‰ñ“]‘¬“x")]
    [SerializeField] private float RotateSpeed;
    [Header("ƒo[‚ÌYÀ•W(ƒ{ƒX‚Í–³‚µ)")]
    [SerializeField] private float BarYPosition;
    [Header("UŒ‚ƒ^ƒCƒvƒvƒŒƒnƒu")]
    [SerializeField] private GameObject[] AttackPrefab;
    [Header("ƒGƒtƒFƒNƒgƒvƒŒƒnƒu")]
    [SerializeField] private GameObject[] EffectPrefab;

    // “Ç‚ÝŽæ‚èê—p
    public float maxHp => MaxHp;
    public bool isArmor => IsArmor;
    public float maxTrunk => MaxTrunk;
    public float attackPower => AttackPower;
    public float moveSpeed => MoveSpeed;
    public float detectionRange => DetectionRange;
    public float attackRange => AttackRange;
    public float idleToChaseTime => IdleToChaseTime;
    public float chaseToAttack => ChaseToAttack;
    public float rotateSpeed => RotateSpeed;
    public float barYPosition => BarYPosition;
    public bool isStrongEnemy => IsStrongEnemy;
    public float maxAttackTime => MaxAttackTime;
    public float attackSwitchRange => AttackSwitchRange;
    public GameObject[] attackPrefab => AttackPrefab;
    public GameObject[] effectPrefab => EffectPrefab;
}
