using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[CreateAssetMenu(menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("強敵かどうかのフラグ")]
    [SerializeField] private bool IsStrongEnemy;
    [Header("アーマー装備フラグ")]
    [SerializeField] private bool IsArmor;
    [Header("最大HP")]
    [SerializeField] private float MaxHp;
    [Header("耐久力")]
    [SerializeField] private float MaxTrunk;
    [Header("プレイヤーに与えるダメージ量")]
    [SerializeField] private float AttackPower;
    [Header("移動速度")]
    [SerializeField] private float MoveSpeed;
    [Header("敵の索敵範囲")]
    [SerializeField] private float DetectionRange;
    [Header("敵の攻撃する範囲")]
    [SerializeField] private float AttackRange;
    [Header("敵がどちらの攻撃をとるかの範囲(攻撃が複数の敵のみ)")]
    [SerializeField] private float AttackSwitchRange;
    [Header("待機状態から追跡状態に遷移するまでの時間")]
    [SerializeField] private float IdleToChaseTime;
    [Header("追跡状態から攻撃状態に遷移するまでの時間")]
    [SerializeField] private float ChaseToAttack;
    [Header("攻撃判定を生成する時間")]
    [SerializeField] private float MaxAttackTime;
    [Header("敵の回転速度")]
    [SerializeField] private float RotateSpeed;
    [Header("バーのY座標(ボスは無し)")]
    [SerializeField] private float BarYPosition;
    [Header("攻撃タイプ1プレハブ")]
    [SerializeField] private GameObject AttackType1Prefab;
    [Header("攻撃タイプ2プレハブ(ザコ敵はなしでOK)")]
    [SerializeField] private GameObject AttackType2Prefab;
    [Header("攻撃タイプ3プレハブ(ボス以外はなしでOK)")]
    [SerializeField] private GameObject AttackType3Prefab;
    [Header("攻撃タイプ4プレハブ(ボス以外はなしでOK)")]
    [SerializeField] private GameObject AttackType4Prefab;

    // 読み取り専用
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
    public GameObject attackType1Prefab => AttackType1Prefab;
    public GameObject attackType2Prefab => AttackType2Prefab;
    public GameObject attackType3Prefab => AttackType3Prefab;
    public GameObject attackType4Prefab => AttackType4Prefab;
    public float barYPosition => BarYPosition;
    public bool isStrongEnemy => IsStrongEnemy;
    public float maxAttackTime => MaxAttackTime;
    public float attackSwitchRange => AttackSwitchRange;
}
