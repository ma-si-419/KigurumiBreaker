using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer�{��")]
    public AudioMixer audioMixer; // Unity��AudioMixer���A�^�b�`����

    [Header("UI�X���C�_�[")]
    public Slider bgmSlider; // BGM�p�̃X���C�_�[
    public Slider seSlider;  // SE�p�̃X���C�_�[

    void Start()
    {
        // �X���C�_�[�̏����l��0.5�ɐݒ�
        bgmSlider.value = 0.5f;
        seSlider.value = 0.5f;

        // �X���C�_�[�����������ɌĂ΂�鏈����o�^
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        seSlider.onValueChanged.AddListener(SetSEVolume);

        // �N�����ɏ������ʂ𔽉f������
        SetBGMVolume(bgmSlider.value);
        SetSEVolume(seSlider.value);
    }

    // BGM�̉��ʂ�ݒ肷�鏈��
    public void SetBGMVolume(float value)
    {
        // �X���C�_�[�̒l(0.0�`1.0)���f�V�x���ɕϊ�
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;

        // AudioMixer�ɓK�p (Expose���Ė��O�� "BGM" �ɂ��邱��)
        audioMixer.SetFloat("BGM", volume);

        // �f�o�b�O�p���O
        Debug.Log("BGM Volume: " + value);
    }

    // SE�̉��ʂ�ݒ肷�鏈��
    public void SetSEVolume(float value)
    {
        // �X���C�_�[�̒l(0.0�`1.0)���f�V�x���ɕϊ�
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;

        // AudioMixer�ɓK�p (Expose���Ė��O�� "SE" �ɂ��邱��)
        audioMixer.SetFloat("SE", volume);

        // �f�o�b�O�p���O
        Debug.Log("SE Volume: " + value);
    }
}
