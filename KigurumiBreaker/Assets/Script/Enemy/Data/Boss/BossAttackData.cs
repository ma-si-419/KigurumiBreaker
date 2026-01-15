using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossAttackType
{
    Attack1,
    Attack2,
    Attack3,
    Attack4,
    Attack5,
    Attack6
}

public enum BossPhase
{
    Phase1,
    Phase2
}

[CreateAssetMenu(menuName = "Boss/BossAttackData")]
public class BossAttackData : ScriptableObject
{
    public List<BossAttack> bossAttackDataList;
}

[System.Serializable]
public class BossAttack
{
    [Header("攻撃ステートの名前")]
    [SerializeField] private string AttackName;          // 遷移するステート名
    [Header("攻撃タイプ")]
    [SerializeField] private BossAttackType BossAttackType; // 攻撃タイプ
    [Header("フェーズ")]
    [SerializeField] private BossPhase BossPhase;     // フェーズ
    [Header("射程")]
    [SerializeField] private float RangeSqr;      // 射程
    [Header("クールタイム")]
    [SerializeField] private float Cooldown;       // クールタイム
    [Header("抽選")]
    [SerializeField] private float Weight;         // 抽選の重み


    // 読み取り専用
    public string attackName => AttackName;
    public BossAttackType bossAttackType => BossAttackType;
    public BossPhase bossPhase => BossPhase;
    public float rangeSqr => RangeSqr;
    public float cooldown => Cooldown;
    public float weight => Weight;
}

