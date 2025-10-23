#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

[CustomEditor(typeof(SoundIDList))]
public class SoundIDListEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 通常のInspectorを描画
        base.OnInspectorGUI();

        // 区切り線
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Enum Generator", EditorStyles.boldLabel);

        if (GUILayout.Button("SoundIDEnum作成"))
        {
            GenerateEnum((SoundIDList)target);
        }
    }

    private void GenerateEnum(SoundIDList list)
    {
        // 出力先パス
        string directory = "Assets/Script/Sound/Data";
        string filePath = Path.Combine(directory, "SoundID.cs");

        // ディレクトリがなければ作成
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // enum定義の中身を作成
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("public enum SoundID");
        sb.AppendLine("{");

        // 各サウンドデータから名前を取得してenumの要素に追加
        foreach (SoundData data in list.soundDatas)
        {
            string name = data.name;
            if (string.IsNullOrWhiteSpace(name)) continue;
            string safeName = MakeSafeEnumName(name);
            sb.AppendLine($"    {safeName},");
        }

        sb.AppendLine("}");

        // ファイル書き込み
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

        // Unityに再インポートさせる
        AssetDatabase.Refresh();
    }

    // enumに使えない文字を安全に変換
    private string MakeSafeEnumName(string name)
    {
        string safe = name.Replace(" ", "_")
                          .Replace("-", "_")
                          .Replace("/", "_");
        return safe;
    }
}
#endif