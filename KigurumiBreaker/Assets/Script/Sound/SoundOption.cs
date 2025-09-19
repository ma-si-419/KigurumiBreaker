using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer本体")]
    public AudioMixer audioMixer; // UnityのAudioMixerをアタッチする

    [Header("UIスライダー")]
    public Slider bgmSlider; // BGM用のスライダー
    public Slider seSlider;  // SE用のスライダー

    void Start()
    {
        // スライダーの初期値を0.5に設定
        bgmSlider.value = 0.5f;
        seSlider.value = 0.5f;

        // スライダーが動いた時に呼ばれる処理を登録
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        seSlider.onValueChanged.AddListener(SetSEVolume);

        // 起動時に初期音量を反映させる
        SetBGMVolume(bgmSlider.value);
        SetSEVolume(seSlider.value);
    }

    // BGMの音量を設定する処理
    public void SetBGMVolume(float value)
    {
        // スライダーの値(0.0～1.0)をデシベルに変換
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;

        // AudioMixerに適用 (Exposeして名前を "BGM" にすること)
        audioMixer.SetFloat("BGM", volume);

        // デバッグ用ログ
        Debug.Log("BGM Volume: " + value);
    }

    // SEの音量を設定する処理
    public void SetSEVolume(float value)
    {
        // スライダーの値(0.0～1.0)をデシベルに変換
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;

        // AudioMixerに適用 (Exposeして名前を "SE" にすること)
        audioMixer.SetFloat("SE", volume);

        // デバッグ用ログ
        Debug.Log("SE Volume: " + value);
    }
}
