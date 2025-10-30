using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnemyPopPatern))]
public class EnemyPopPaternDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // WaveSpawnerのプロパティから_indexの値を取得
        SerializedProperty enemyPopPaternProperty = property.FindPropertyRelative("_index");

        // パターン1から始めたいので+1
        int paternIndex = enemyPopPaternProperty.intValue + 1;

        // ラベルをステージ+数字で描画
        EditorGUI.PropertyField(position, property, new GUIContent("パターン" + paternIndex), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 要素全体の高さを自動で調整
        return EditorGUI.GetPropertyHeight(property, true);
    }
}
