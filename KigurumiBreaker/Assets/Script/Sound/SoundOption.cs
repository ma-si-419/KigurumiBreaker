using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundOption : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer; // MasterMixer���A�T�C��
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    private void Start()
    {
        // �����l�ݒ�i�ۑ����Ă���ꍇ��PlayerPrefs����j
        float bgmValue = PlayerPrefs.GetFloat("BGMVolume", 0f);
        float seValue = PlayerPrefs.GetFloat("SEVolume", 0f);

        bgmSlider.value = bgmValue;
        seSlider.value = seValue;

        SetBGMVolume(bgmValue);
        SetSEVolume(seValue);

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        seSlider.onValueChanged.AddListener(SetSEVolume);
    }

    public void SetBGMVolume(float value)
    {
        audioMixer.SetFloat("BGMVolume", value);
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void SetSEVolume(float value)
    {
        audioMixer.SetFloat("SEVolume", value);
        PlayerPrefs.SetFloat("SEVolume", value);
    }
}