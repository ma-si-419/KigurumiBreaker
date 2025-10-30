using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StageSet))]
public class StageDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // ステージのプロパティから_stageKindの値を取得
        SerializedProperty stageKindProperty = property.FindPropertyRelative("_stageKind");
        int stageKind = stageKindProperty.enumValueIndex;

        // ステージのプロパティから_indexの値を取得
        SerializedProperty stageIndexProperty = property.FindPropertyRelative("_index");
        int stageIndex = stageIndexProperty.intValue;

        // ラベルをステージ+数字で描画
        EditorGUI.PropertyField(position, property, new GUIContent("ステージ" + stageKind + "-" + stageIndex), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 要素全体の高さを自動で調整
        return EditorGUI.GetPropertyHeight(property, true);
    }
}