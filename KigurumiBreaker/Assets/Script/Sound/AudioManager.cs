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

    // ここを SoundData（個別エントリ）ではなく、SoundIDList（一覧の ScriptableObject）にする
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
            Debug.LogWarning("[AudioManager] Duplicate instance detected and destroyed.");
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
    public void PlaySound(SoundID id)
    {
        if (audioDatabase == null)
        {
            Debug.LogError("[AudioManager] audioDatabase is null. Assign your SoundIDList asset in the inspector.");
            return;
        }

        var entry = audioDatabase.SoundEntry(id); // ← SoundIDList に追加したメソッドを使用
        if (entry == null || entry.clip == null)
        {
            Debug.LogWarning($"[AudioManager] Sound not found or clip null for id: {id}");
            return;
        }

        // ここは BGM扱いで再生（もし SE と BGM を分けたいなら判定を追加してください）
        if (bgmSource.clip == entry.clip && bgmSource.isPlaying) return;

        bgmSource.clip = entry.clip;
        bgmSource.volume = entry.volume;
        bgmSource.loop = true;
        Debug.Log($"Play BGM: {id}");
        bgmSource.Play();
    }

    public void PlaySE(SoundID id)
    {
        if (audioDatabase == null)
        {
            Debug.LogError("[AudioManager] audioDatabase is null. Assign your SoundIDList asset in the inspector.");
            return;
        }

        var entry = audioDatabase.SoundEntry(id);
        if (entry == null || entry.clip == null)
        {
            Debug.LogWarning($"[AudioManager] SE not found or clip null for id: {id}");
            return;
        }

        seSource.PlayOneShot(entry.clip, entry.volume);
    }

    // ============================================================
    // 音量制御（略、既存のまま）
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
        PlayerPrefs.Save();
    }

    public void SetBGMVolume(float value)
    {
        SetMixerVolume(BGM_PARAM, value);
        PlayerPrefs.SetFloat(BGM_PARAM, value);
        PlayerPrefs.Save();
    }

    public void SetSEVolume(float value)
    {
        SetMixerVolume(SE_PARAM, value);
        PlayerPrefs.SetFloat(SE_PARAM, value);
        PlayerPrefs.Save();
    }

    private void SetMixerVolume(string param, float value)
    {
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(param, volume);
    }

    public void SaveVolumeSettings()
    {
        PlayerPrefs.Save();
    }
}
