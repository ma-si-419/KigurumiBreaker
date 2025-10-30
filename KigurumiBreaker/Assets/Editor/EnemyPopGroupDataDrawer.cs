using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnemyPopGroup))]
public class EnemyPopGroupDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // EnemyPopGroupのプロパティから_indexの値を取得
        SerializedProperty popGroupIndexProperty = property.FindPropertyRelative("_index");
        int groupIndex = popGroupIndexProperty.intValue + 1;

        // ラベルをステージ+数字で描画
        EditorGUI.PropertyField(position, property, new GUIContent("グループ" + groupIndex), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 要素全体の高さを自動で調整
        return EditorGUI.GetPropertyHeight(property, true);
    }
}