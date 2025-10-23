using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;

    // SoundDataではなく、SoundIDListにする
    [Header("Audio Database (ScriptableObject)")]
    [SerializeField] private SoundIDList audioDatabase;

    [Header("UI Elements (Option Menu)")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    private const string MASTER_PARAM = "Master";
    private const string BGM_PARAM = "BGM";
    private const string SE_PARAM = "SE";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        float master = PlayerPrefs.GetFloat(MASTER_PARAM, 1f);
        float bgm = PlayerPrefs.GetFloat(BGM_PARAM, 1f);
        float se = PlayerPrefs.GetFloat(SE_PARAM, 1f);

        if (masterSlider != null)
        {
            masterSlider.value = master;
            bgmSlider.value = bgm;
            seSlider.value = se;

            masterSlider.onValueChanged.AddListener(SetMasterVolume);
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
            seSlider.onValueChanged.AddListener(SetSEVolume);
        }

        ApplyVolumes();
    }

    // ============================================================
    // 再生処理
    // ============================================================
    public void PlayBGM(SoundID id)
    {
        if (audioDatabase == null)
        {
            return;
        }

        var entry = audioDatabase.SoundFind(id); // ← SoundIDList に追加したメソッドを使用
        if (entry == null || entry.clip == null)
        {
            return;
        }

        // BGM再生中に同じBGMがリクエストされた場合は無視
        if (bgmSource.clip == entry.clip && bgmSource.isPlaying) return;

        bgmSource.clip = entry.clip;
        bgmSource.volume = entry.volume;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySE(SoundID id)
    {
        if (audioDatabase == null)
        {
            return;
        }

        var entry = audioDatabase.SoundFind(id);
        if (entry == null || entry.clip == null)
        {
            return;
        }

        seSource.PlayOneShot(entry.clip, entry.volume);
    }

    // ============================================================
    // 音量制御
    // ============================================================
    /// <summary>
    /// PlayerPrefs から音量設定を取得して AudioMixer に適用する
    /// </summary>
    private void ApplyVolumes()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(MASTER_PARAM, 1f));
        SetBGMVolume(PlayerPrefs.GetFloat(BGM_PARAM, 1f));
        SetSEVolume(PlayerPrefs.GetFloat(SE_PARAM, 1f));
    }
    /// <summary>
    /// マスターボリューム設定
    /// </summary>
    /// <param name="value"></param>

    public void SetMasterVolume(float value)
    {
        SetMixerVolume(MASTER_PARAM, value);
        PlayerPrefs.SetFloat(MASTER_PARAM, value);
        PlayerPrefs.Save();
    }
    /// <summary>
    /// BGMボリューム設定
    /// </summary>
    /// <param name="value"></param>

    public void SetBGMVolume(float value)
    {
        SetMixerVolume(BGM_PARAM, value);
        PlayerPrefs.SetFloat(BGM_PARAM, value);
        PlayerPrefs.Save();
    }
    /// <summary>
    /// SEボリューム設定
    /// </summary>
    /// <param name="value"></param>
    public void SetSEVolume(float value)
    {
        SetMixerVolume(SE_PARAM, value);
        PlayerPrefs.SetFloat(SE_PARAM, value);
        PlayerPrefs.Save();
    }
    /// <summary>
    /// AudioMixer に対してデシベル変換を行い音量設定を適用する
    /// </summary>
    /// <param name="param"></param>
    /// <param name="value"></param>

    private void SetMixerVolume(string param, float value)
    {
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(param, volume);
    }
    /// <summary>
    /// 音量設定を PlayerPrefs に保存する
    /// </summary>
    public void SaveVolumeSettings()
    {
        PlayerPrefs.Save();
    }
}
