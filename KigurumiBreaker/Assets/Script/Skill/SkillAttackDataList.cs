using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Skill/SkillAttackData")]
public class SkillAttackDataList : ScriptableObject
{
    public List<SkillAttackData> attackDataList;
}
[System.Serializable]
public class SkillAttackData
{
    [Header("名前")]
    [SerializeField] private string AttackName;
    [Header("ダメージ")]
    [SerializeField] private int Damage;
    [Header("攻撃判定の大きさ")]
    [SerializeField] private float Scale;
    [Header("生存時間")]
    [SerializeField] private int AttackLifeTime;
    [Header("攻撃時に出すエフェクト")]
    [SerializeField] private GameObject AttackEffect;
    [Header("攻撃があたった時に出すエフェクト")]
    [SerializeField] private GameObject HitEffect;
    [Header("ノックバックの大きさ")]
    [SerializeField] private float KnockBackPower;
    [Header("敵に与えるデバフ")]
    [SerializeField] private Enemy.EnemyDebuff Debuff;
    [Header("敵の攻撃を跳ね返すか")]
    [SerializeField] private bool IsReflect;
    [Header("弱攻撃ならtrue")]
    [SerializeField] private bool IsWeakAttack;

    // 読み取り専用プロパティ

    public string attackName => AttackName;
    public int damage => Damage;
    public float scale => Scale;
    public int attackLifeTime => AttackLifeTime;
    public GameObject attackEffect => AttackEffect;
    public GameObject hitEffect => HitEffect;
    public float knockBackPower => KnockBackPower;
    public Enemy.EnemyDebuff debuff => Debuff;
    public bool isReflect => IsReflect;
    public bool isWeakAttack => IsWeakAttack;

}
