using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseSceneController : MonoBehaviour
{
    [SerializeField] private Button[] _button; //�{�^���z��
    private int _currnetIndex = 0; //���݂̑I���{�^��
    private float _inputCooldown = 0.23f; //���͂̃N�[���_�E������
    private float _lastInputTime = 0f; //�Ō�̓��͎���

    // Start is called before the first frame update
    void Start()
    {
        HighlightButton(_currnetIndex);
    }

    // Update is called once per frame
    void Update()
    {
        //�|�[�Y���͑���ł��Ȃ��悤�ɂ���
        if (BaseSceneController.instance.isOption) return;

        float vertical = Input.GetAxis("Vertical");

        //�N�[���_�E������
        if(Time.unscaledTime - _lastInputTime > _inputCooldown)
        {
            //��J�[�\��
            if (vertical > 0.5f)
            {
                _currnetIndex = (_currnetIndex - 1 + _button.Length) % _button.Length;
                HighlightButton(_currnetIndex);
                _lastInputTime = Time.unscaledTime;
            }
            //���J�[�\��
            else if (vertical < -0.5f)
            {
                _currnetIndex = (_currnetIndex + 1) % _button.Length;
                HighlightButton(_currnetIndex);
                _lastInputTime = Time.unscaledTime;
            }
        }

        //����(A�{�^��)
        if(Input.GetButtonDown("Submit"))
        {
            _button[_currnetIndex].onClick.Invoke();
            Debug.Log("A�{�^����������܂���");
        }
    }

    //�I�𒆂̃{�^�����n�C���C�g�\��(��)
    //�����Ɨǂ����o���l����(�A���t�@�ł���)
    private void HighlightButton(int index)
    {
        for(int i = 0; i < _button.Length; i++)
        {
            var colors = _button[i].colors;
            colors.normalColor = (i == index) ? Color.yellow : Color.white;
            _button[i].colors = colors;
        }
    }

    public void RestartButton()
    {
        BaseSceneController.instance.TogglePause();
        Debug.Log("�Q�[���𑱂���");
    }

    public void OptionButton()
    {
        //�I�v�V������ʂ��J��
        BaseSceneController.instance.ToggleOption();
        Debug.Log("�I�v�V������ʂ�");
    }

    public void TitleButton()
    {
        BaseSceneController.instance.TogglePause();
        BaseSceneController.instance.ChangeSceneWithFade(SceneType.TitleScene);
        Debug.Log("�^�C�g���ɖ߂�");
    }

}
