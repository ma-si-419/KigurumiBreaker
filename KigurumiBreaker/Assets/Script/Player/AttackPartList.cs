using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackPartList")]
public class AttackPartList : ScriptableObject
{
    public List<AttackPart> attackDataList;
}
[System.Serializable]
public class AttackPart
{
    [Header("名前")]
    [SerializeField] private string AttackPartName;
    [Header("攻撃部位のリグの名前")]
    [SerializeField] private string ObjectRigName;

    // 読み取り専用プロパティ

    public string attackPartName => AttackPartName;
    public string objectRigName => ObjectRigName;

}
