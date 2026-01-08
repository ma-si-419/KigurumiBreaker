using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/PassiveSkillDataList")]
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
        AttackPower,
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
    [Header("属性")]
    [SerializeField] private SkillData.SkillElement SkillElement;
    [Header("ステータス上昇情報")]
    [SerializeField] private List<UpStatus> UpStatuses;
    [Header("ゲームオブジェクトが出るパッシブ情報")]
    [SerializeField] private List<PassiveObject> passiveObjects;
    [Header("スキルの説明文")]
    [TextArea] [SerializeField] private string SkillContents;


    public string skillName => SkillName;
    public SkillData.SkillElement skillElement => SkillElement;
    public List<UpStatus> upStatuses => UpStatuses;
    public List<PassiveObject> PassiveObjects => passiveObjects;
    public string skillContents => SkillContents;
}
