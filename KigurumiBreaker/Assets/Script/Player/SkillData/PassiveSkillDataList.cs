using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PassiveSkillDataList")]
public class PassiveSkillDataList : ScriptableObject
{
    public List<PassiveSkillData> passiveSkillDataList;
}
[System.Serializable]
public class PassiveSkillData
{
    public enum PassiveStatusKind
    {
        MaxHp,
        LowAttackDamage,
        ChargeAttackDamage,
        RangedAttackDamage,
        RangedAttackBullet,
        MoveSpeed,
        DashCount,
        DamageCutRate,
        DodgeRate
    }

    public enum GameObjectPopTiming
    {
        None,
        Damage,
        Dodge,
        Attack
    }


    [System.Serializable]
    public class UpStatus
    {
        public PassiveStatusKind statusKind;
        public float addNum;
    }

    [System.Serializable]
    public class PassiveObject
    {
        public GameObjectPopTiming popTiming;
        public GameObject gameObject;
    }

    [Header("名前")]
    [SerializeField] private string SkillName;
    [Header("ステータス上昇情報")]
    [SerializeField] private List<UpStatus> UpStatuses;
    [Header("ゲームオブジェクトが出るパッシブ情報")]
    [SerializeField] private List<PassiveObject> passiveObjects;


    public string skillName => SkillName;
    public List<UpStatus> upStatuses => UpStatuses;
    public List<PassiveObject> PassiveObjects => passiveObjects;
}
