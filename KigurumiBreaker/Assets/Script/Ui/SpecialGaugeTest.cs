using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialGaugeTest : MonoBehaviour
{
    [SerializeField] private Image _specialGaugeImage;           //�Q�[�W�摜
    //[SerializeField] private Image _auraImage;                 //�I�[���̉摜
    [SerializeField] private Color _normalColor = Color.yellow;  //�ʏ펞�̐F
    [SerializeField] private Color _maxColor = Color.red;        //�}�b�N�X���̐F
    [SerializeField] private float _flashSpeed;                  //�_�ő��x    
    [SerializeField] private float _auraRotateSpeed;             //�I�[���̉�]���x


    private float _current = 0f;    //���݂̃Q�[�W��
    private float _max = 100f;      //�Q�[�W�̍ő��

    // Update is called once per frame
    void Update()
    {
        // --- �L�[�{�[�h���� ---
        if (Input.GetKey(KeyCode.UpArrow))    // �� �ő���
        {
            _current += 30f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))  // �� �Ō���
        {
            _current -= 30f * Time.deltaTime;
        }


        // �l�͈̔͂𐧌�
        _current = Mathf.Clamp(_current, 0f, _max);

        // �Q�[�W���f
        float ratio = _current / _max;
        _specialGaugeImage.fillAmount = ratio;

        //Max������
        if(ratio >= 1f)
        {
            //�_��
            float flash = (Mathf.Sin(Time.time * _flashSpeed) + 1f) / 2f; // 0~1�̓_�Œl
            _specialGaugeImage.color = Color.Lerp(_maxColor,Color.white, flash); // �_��

            //�I�[����\��

        }

    }
}
