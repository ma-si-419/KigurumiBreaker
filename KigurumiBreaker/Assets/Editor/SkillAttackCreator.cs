using System.IO;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillAttackCreateData))]
public class SkillAttackCreator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SkillAttackCreateData data = (SkillAttackCreateData)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("攻撃プレハブを生成（スクリプト付き）"))
        {
            CreateAttackPrefab(data);
        }
    }

    private void CreateAttackPrefab(SkillAttackCreateData data)
    {
        if (data.effectPrefab == null)
        {
            Debug.LogError("EffectPrefab が設定されていません。");
            return;
        }

        // メモリ上で生成（シーンに出さない）
        GameObject temp = new GameObject(data.prefabName);

        // 攻撃スクリプトを追加
        PlayerAttack attack = temp.AddComponent<PlayerAttack>();

        // ここで攻撃データを設定

        PlayerAttack.PlayerAttackData attackData = new PlayerAttack.PlayerAttackData();

        attackData.damage = data.damage;
        attackData.attackLifeTime = data.lifeTime;

        attack.SetPlayerAttackData(attackData);

        ////


        // エフェクトを子に追加
        GameObject effectCopy = (GameObject)PrefabUtility.InstantiatePrefab(data.effectPrefab);
        effectCopy.name = "Effect";
        effectCopy.transform.SetParent(temp.transform, false);

        // 保存フォルダ
        string folderPath = "Assets/Prefab/PlayerAttack/SkillAttack/";

        // フォルダがなければ作成
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 名前重複を避ける
        string prefabPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}{data.prefabName}.prefab");

        // Prefab保存（シーンに出さずに完結）
        PrefabUtility.SaveAsPrefabAsset(temp, prefabPath);

        // 一時オブジェクト削除
        Object.DestroyImmediate(temp);
        Object.DestroyImmediate(effectCopy);

        AssetDatabase.Refresh();

        Debug.Log("攻撃プレハブを生成しました" + prefabPath);
    }
}
