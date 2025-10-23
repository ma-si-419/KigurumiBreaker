using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundData2
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
    public List<SoundData2> soundDatas = new List<SoundData2>();
}
