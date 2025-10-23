using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SoundEntry
{
    public string id; // 識別名 (例: "TitleBGM")
    public AudioClip clip; // 再生する AudioClip
    [Range(0f, 1f)] public float volume = 1f; // 個別音量
}

[CreateAssetMenu(fileName = "SoundData", menuName = "Game/Sound Data")]
public class SoundData : ScriptableObject
{
    [Header("BGMリスト")]
    public List<SoundEntry> bgmList = new List<SoundEntry>();

    [Header("SEリスト")]
    public List<SoundEntry> seList = new List<SoundEntry>();

    public SoundEntry GetBgmEntry(string id)
    {
        return bgmList.Find(bgmEntry => bgmEntry.id == id);
    }

    public SoundEntry GetSeEntry(string id)
    {
        return seList.Find(seEntry => seEntry.id == id);
    }
}
