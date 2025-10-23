using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundData
{
    public string Name; // 識別名 (例: "TitleBGM")
    public AudioClip Clip; // 再生する AudioClip
    [Range(0f, 1f)] public float Volume = 1f; // 個別音量

    // 読み取り専用プロパティ
    public string name => Name;
    public AudioClip clip => Clip;
    public float volume => Volume;
}

[CreateAssetMenu(menuName = "Sound/SoundIDList")]
public class SoundIDList : ScriptableObject
{
    // サウンドの名前
    [Header("SEリスト")]
    public List<SoundData> soundDatas = new List<SoundData>();

    // Enum (SoundID) から SoundData を返す（名前が enum.ToString() と等しいことが前提）
    public SoundData SoundEntry(SoundID id)
    {
        if (soundDatas == null) return null;
        return soundDatas.Find(s => s != null && s.Name == id.ToString());
    }
}
