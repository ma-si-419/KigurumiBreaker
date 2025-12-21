using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundData
{
    public string Name; // 識別名 (例: "TitleBGM")
    public AudioClip Clip; // 再生する AudioClip
    [Range(0f, 1f)] public float Volume = 1f; // 個別音量

    [Header("再生時間（秒）")]
    [Tooltip("0以下なら最後まで再生")]
    public float PlayTime = 0f;          

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
    public SoundData SoundFind(SoundID id)
    {
        if (soundDatas == null) return null;
        return soundDatas[(int)id];
    }
}
