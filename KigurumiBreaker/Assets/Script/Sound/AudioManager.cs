using System.Collections;
using System.Collections.Generic;
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

    //ステージのBGM
    private StageSet.StageKind? currentStageKind = null;
    private Coroutine bgmFadeCoroutine;
    private bool isBGMStopped = false;

    // ===== 追加：制御可能SE(ID管理) =====
    private Dictionary<SoundID, AudioSource> seById = new Dictionary<SoundID, AudioSource>();

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
    // ===== 追加：制御可能SE(ID管理) =====
    public void RegisterSE(SoundID id)
    {
        if (seById.ContainsKey(id)) return;

        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.outputAudioMixerGroup = seSource.outputAudioMixerGroup;

        seById.Add(id, source);
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
        if (audioDatabase == null) return;

        var entry = audioDatabase.SoundFind(id);
        if (entry == null || entry.clip == null) return;

        if (!seById.TryGetValue(id, out var source))
        {
            // 念のため自動登録
            RegisterSE(id);
            source = seById[id];
        }

        source.clip = entry.clip;
        source.volume = entry.volume;
        source.loop = false;
        source.Play();
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
    public void StopSE(SoundID id)
    {
        if (!seById.TryGetValue(id, out var source))
            return;

        source.Stop();
        Destroy(source);
        seById.Remove(id);
    }

    // ============================================================
    // BGMステージ切り替え処理
    // ============================================================
    /// <summary>
    /// 
    /// </summary>
    /// <param name="kind"></param>
    public void ChangeBGMByStageKind(StageSet.StageKind kind)
    {
        // 前回と同じステージなら何もしない
        if (currentStageKind.HasValue && currentStageKind.Value == kind)
            return;

        currentStageKind = kind;

        SoundID bgmID = ConvertStageKindToSoundID(kind);
        if (bgmID == SoundID.None) return;

        PlayBGM(bgmID);
    }

    private SoundID ConvertStageKindToSoundID(StageSet.StageKind kind)
    {
        switch (kind)
        {
            case StageSet.StageKind.Home:
                return SoundID.Home;

            case StageSet.StageKind.Tutorial:
                return SoundID.Forest;

            case StageSet.StageKind.Forest:
                return SoundID.Forest;

            case StageSet.StageKind.Forest_Boss:
                return SoundID.Boss1;

            case StageSet.StageKind.Cave:
                return SoundID.Cave;

            case StageSet.StageKind.Cave_Boss:
                return SoundID.Boss2;
            default:
                return SoundID.None;
        }
    }

    public void FadeOutBGM(float duration = 1.0f)
    {
        if (bgmSource == null || !bgmSource.isPlaying)
            return;

        if (bgmFadeCoroutine != null)
            StopCoroutine(bgmFadeCoroutine);

        bgmFadeCoroutine = StartCoroutine(FadeOutRoutine(duration));
    }


    private IEnumerator FadeOutBGMRoutine(float duration)
    {
        if (bgmFadeCoroutine != null)
            StopCoroutine(bgmFadeCoroutine);

        bgmFadeCoroutine = StartCoroutine(FadeOutRoutine(duration));
        yield return bgmFadeCoroutine;
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        float startVolume = bgmSource.volume;
        float time = 0f;

        while (time < duration)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.volume = startVolume;
        bgmSource.clip = null;
        currentStageKind = null;
    }
    public void FadeOutAndChangeBGM(StageSet.StageKind nextKind, float duration = 1f)
    {
        if (bgmFadeCoroutine != null)
            StopCoroutine(bgmFadeCoroutine);

        bgmFadeCoroutine = StartCoroutine(FadeOutAndChangeRoutine(nextKind, duration));
    }

    private IEnumerator FadeOutAndChangeRoutine(StageSet.StageKind nextKind, float duration)
    {
        yield return FadeOutRoutine(duration);
        ChangeBGMByStageKind(nextKind);
    }
    // ============================================================
    // フェードなしBGM停止
    // ============================================================
    public void StopBGM()
    {
        Debug.Log("StopBGM CALLED");

        // 全AudioSourceを取得
        AudioSource[] allSources = FindObjectsOfType<AudioSource>();

        Debug.Log("AudioSource count = " + allSources.Length);

        foreach (var src in allSources)
        {
            Debug.Log($"Stop -> {src.gameObject.name}, playing={src.isPlaying}, clip={src.clip}");
            src.Stop();
        }
    }




}
