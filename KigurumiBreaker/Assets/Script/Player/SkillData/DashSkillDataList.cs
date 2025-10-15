using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DashSkillDataList")]
public class DashSkillDataList : ScriptableObject
{
    public List<DashSkillData> dashSkillDataList;
}
[System.Serializable]
public class DashSkillData
{
    [Header("名前")]
    [SerializeField] private string SkillName;
    [Header("属性")]
    [SerializeField] private SkillData.SkillElement SkillElement;
    [Header("ダッシュ開始地点に出す攻撃")]
    [SerializeField] private GameObject StartAttack;
    [Header("ダッシュ中に出す攻撃")]
    [SerializeField] private GameObject OnDashAttack;
    [Header("ダッシュ終了地点に出す攻撃")]
    [SerializeField] private GameObject EndAttack;
    [Header("スキルの説明文")]
    [TextArea][SerializeField] private string SkillContents;

    public string skillName => SkillName;
    public SkillData.SkillElement skillElement => SkillElement;
    public GameObject startAttack => StartAttack;
    public GameObject onDashAttack => OnDashAttack;
    public GameObject endAttack => EndAttack;
    public string skillContents => SkillContents;

}
