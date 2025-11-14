using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AttackPart))]
public class AttackPartDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // StateTypeを取得
        SerializedProperty AttackPartTypeProp = property.FindPropertyRelative("AttackPartType");

        AttackPart.AttackPartKind attackPartKind = (AttackPart.AttackPartKind)AttackPartTypeProp.enumValueIndex;

        string labelText = attackPartKind.ToString();

        // ラベル名を変更
        EditorGUI.PropertyField(position, property, new GUIContent(labelText), true);

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 子要素込みの高さを正しく返す
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}