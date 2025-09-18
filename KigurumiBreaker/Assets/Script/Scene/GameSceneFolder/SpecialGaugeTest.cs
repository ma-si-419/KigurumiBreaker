using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialGaugeTest : MonoBehaviour
{
    [SerializeField] private Image specialGaugeImage;           //�Q�[�W�摜
    [SerializeField] private Color normalColor = Color.yellow;  //�ʏ펞�̐F
    [SerializeField] private Color maxColor = Color.red;        //�}�b�N�X���̐F

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
        specialGaugeImage.fillAmount = ratio;

        // �F�؂�ւ�
        specialGaugeImage.color = (ratio >= 1f) ? maxColor : normalColor;
    }
}
