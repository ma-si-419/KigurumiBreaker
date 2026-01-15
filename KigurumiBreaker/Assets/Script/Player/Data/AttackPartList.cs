using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/AttackPartList")]
public class AttackPartList : ScriptableObject
{
    [SerializeField] private List<AttackPart> AttackPartDataList;

    // 読み取り専用プロパティ
    public List<AttackPart> attackPartDataList => AttackPartDataList;

#if UNITY_EDITOR
    // AttackPartの種類に合わせてリストを自動調整
    private void OnValidate()
    {
        var enumValues = (AttackPart.AttackPartKind[])Enum.GetValues(typeof(AttackPart.AttackPartKind));

        // 追加されていないAttackPartを補完
        foreach (var value in enumValues)
        {
            if (!AttackPartDataList.Exists(s => s.attackPartType == value))
            {
                // AttackPartの情報を設定
                AttackPart partData = new AttackPart();
                partData.SetAttackPartKind(value);


                // リストに追加
                AttackPartDataList.Add(partData);
            }
        }

        // Enumから削除されたStateを削除
        attackPartDataList.RemoveAll(s => Array.IndexOf(enumValues, s.attackPartType) < 0);

        // ソート（Enum順で見やすく）
        attackPartDataList.Sort((a, b) => a.attackPartType.CompareTo(b.attackPartType));

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif

}
[System.Serializable]
public class AttackPart
{
    public enum AttackPartKind
    {
        [InspectorName("頭")]
        Head,
        [InspectorName("左手")]
        LeftHand,
        [InspectorName("右手")]
        RightHand,
        [InspectorName("左前腕")]
        LeftForeArm,
        [InspectorName("右前腕")]
        RightForeArm,
        [InspectorName("左脚")]
        LeftLeg,
        [InspectorName("右脚")]
        RightLeg,
        [InspectorName("首")]
        Neck,
        [InspectorName("上のほうの背骨")]
        UpperSpine,
    }
    [Header("攻撃部位の種類")]
    [SerializeField] private AttackPartKind AttackPartType;
    [Header("攻撃部位のリグの名前")]
    [SerializeField] private string ObjectRigName;

    // 自動でリストに追加するための関数
    public void SetAttackPartKind(AttackPartKind type)
    {
        AttackPartType = type;
    }

    // 読み取り専用プロパティ
    public AttackPartKind attackPartType => AttackPartType;

    public string objectRigName => ObjectRigName;

}
