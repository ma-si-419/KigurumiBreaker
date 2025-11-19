using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSound : MonoBehaviour
{
    public static StageSound instance;

    public AudioSource bgmSource;
    public AudioClip[] bgmClips; // Enum 順番に対応

    private StageSet.StageKind? currentKind = null; // 今流れているステージ

    void Awake()
    {
        instance = this;
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;

        bgmSource.clip = clip;
        bgmSource.Play();
        bgmSource.loop = true;
    }

    // StageKind が変わった時だけ再生
    public void ChangeBGM_ByStageKind(StageSet.StageKind kind)
    {
        // 前回と同じなら何もしない
        if (currentKind.HasValue && currentKind.Value == kind)
            return;

        currentKind = kind;

        int index = (int)kind;
        if (index < 0 || index >= bgmClips.Length) return;

        PlayBGM(bgmClips[index]);
    }
}
