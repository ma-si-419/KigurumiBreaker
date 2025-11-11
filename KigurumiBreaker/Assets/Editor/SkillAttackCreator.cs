using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillAttackCreateData))]
public class SkillAttackCreator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // インスペクターのものをキャスト
        SkillAttackCreateData data = (SkillAttackCreateData)target;

        EditorGUILayout.Space();

        // ボタンを作成
        if (GUILayout.Button("攻撃プレハブを生成（スクリプト付き）"))
        {
            for (int i = 0; i < data.skillDataList.Count; i++)
            {
                CreateAttackPrefab(data.skillDataList[i]);
            }
        }
    }

    private void CreateAttackPrefab(SkillAttackData data)
    {
        if (data.attackEffect == null)
        {
            Debug.LogError("Effect が設定されていません:" + data.attackName);
            return;
        }

        // 一時オブジェクト作成
        GameObject temp = new GameObject(data.attackName);

        // 当たり判定を追加
        SphereCollider collider = temp.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = data.scale;

        // 攻撃スクリプトを追加
        PlayerAttack attack = temp.AddComponent<PlayerAttack>();

        // 攻撃データを設定
        PlayerAttack.PlayerAttackData attackData = new PlayerAttack.PlayerAttackData();

        attackData.damage = data.damage;
        attackData.knockBackPower = data.knockBackPower;
        attackData.attackLifeTime = data.attackLifeTime;
        attackData.hitEffect = data.hitEffect;
        attackData.debuffType = data.debuff;
        attackData.isReflect = data.isReflect;
        attackData.isWeakAttack = data.isWeakAttack;

        attack.SetPlayerAttackData(attackData);

        // エフェクトを子に追加
        GameObject effectCopy = (GameObject)PrefabUtility.InstantiatePrefab(data.attackEffect);
        effectCopy.name = "Effect";
        effectCopy.transform.SetParent(temp.transform, false);

        // 保存フォルダ設定
        string folderPath = "Assets/Prefab/PlayerAttack/SkillAttack/";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 保存パス
        string prefabPath = $"{folderPath}{data.attackName}.prefab";

        // すでにPrefabが存在する場合 → 上書き保存
        if (File.Exists(prefabPath))
        {
            GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            PrefabUtility.SaveAsPrefabAssetAndConnect(temp, prefabPath, InteractionMode.AutomatedAction);
            Debug.Log("既存Prefabを上書きしました:" + prefabPath);
        }
        else
        {
            // 新規作成
            PrefabUtility.SaveAsPrefabAsset(temp, prefabPath);
            Debug.Log("新規Prefabを作成しました:" + prefabPath);
        }

        // 一時オブジェクト削除
        Object.DestroyImmediate(temp);

        AssetDatabase.Refresh();
    }
}