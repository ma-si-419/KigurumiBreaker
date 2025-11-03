using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[CreateAssetMenu(menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Å‘åHP")]
    [SerializeField] private float MaxHp;
    [Header("ƒA[ƒ}[‘•”õƒtƒ‰ƒO")]
    [SerializeField] private bool IsArmor;
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
    [Header("‘Ò‹@ó‘Ô‚©‚ç’ÇÕó‘Ô‚É‘JˆÚ‚·‚é‚Ü‚Å‚ÌŽžŠÔ")]
    [SerializeField] private float IdleToChaseTime;
    [Header("’ÇÕó‘Ô‚©‚çUŒ‚ó‘Ô‚É‘JˆÚ‚·‚é‚Ü‚Å‚ÌŽžŠÔ")]
    [SerializeField] private float ChaseToAttack;
    [Header("“G‚Ì‰ñ“]‘¬“x")]
    [SerializeField] private float RotateSpeed;
    [Header("UŒ‚‚Ì“–‚½‚è”»’èƒvƒŒƒnƒu")]
    [SerializeField] private GameObject AttackPrefab;
    [Header("ƒo[‚ÌYÀ•W(ƒ{ƒX‚Í–³‚µ)")]
    [SerializeField] private float BarYPosition;

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
    public GameObject attackPrefab => AttackPrefab;
    public float barYPosition => BarYPosition;

}
