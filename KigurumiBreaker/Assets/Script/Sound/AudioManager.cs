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

    [Header("Audio Database")]
    [SerializeField] private SoundData audioDatabase;

    [Header("UI Elements (Option Menu)")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    private const string MASTER_PARAM = "Master";
    private const string BGM_PARAM = "BGM";
    private const string SE_PARAM = "SE";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // --- スライダー初期設定 ---
        if (masterSlider != null)
        {
            float master = PlayerPrefs.GetFloat(MASTER_PARAM, 1f);
            float bgm = PlayerPrefs.GetFloat(BGM_PARAM, 1f);
            float se = PlayerPrefs.GetFloat(SE_PARAM, 1f);

            masterSlider.value = master;
            bgmSlider.value = bgm;
            seSlider.value = se;

            masterSlider.onValueChanged.AddListener(SetMasterVolume);
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
            seSlider.onValueChanged.AddListener(SetSEVolume);

            ApplyVolumes();
        }
        else
        {
            ApplyVolumes();
        }
    }

    // ============================================================
    // 再生処理
    // ============================================================
    public void PlayBGM(string id)
    {
        var entry = audioDatabase.GetBGMEntry(id);
        if (entry == null || entry.clip == null) return;

        if (bgmSource.clip == entry.clip && bgmSource.isPlaying) return;

        bgmSource.clip = entry.clip;
        bgmSource.volume = entry.volume;
        bgmSource.loop = true;
        Debug.Log($"Play BGM: {id}");
        bgmSource.Play();
    }

    public void PlaySE(string id)
    {
        var entry = audioDatabase.GetSEEntry(id);
        if (entry == null || entry.clip == null) return;

        seSource.PlayOneShot(entry.clip, entry.volume);
    }

    // ============================================================
    // 音量制御
    // ============================================================
    private void ApplyVolumes()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(MASTER_PARAM, 1f));
        SetBGMVolume(PlayerPrefs.GetFloat(BGM_PARAM, 1f));
        SetSEVolume(PlayerPrefs.GetFloat(SE_PARAM, 1f));
    }

    public void SetMasterVolume(float value)
    {
        SetMixerVolume(MASTER_PARAM, value);
        PlayerPrefs.SetFloat(MASTER_PARAM, value);
    }

    public void SetBGMVolume(float value)
    {
        SetMixerVolume(BGM_PARAM, value);
        PlayerPrefs.SetFloat(BGM_PARAM, value);
    }

    public void SetSEVolume(float value)
    {
        SetMixerVolume(SE_PARAM, value);
        PlayerPrefs.SetFloat(SE_PARAM, value);
    }

    private void SetMixerVolume(string param, float value)
    {
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(param, volume);
    }

    // ============================================================
    // 保存反映用
    // ============================================================
    public void SaveVolumeSettings()
    {
        PlayerPrefs.Save();
    }
}
