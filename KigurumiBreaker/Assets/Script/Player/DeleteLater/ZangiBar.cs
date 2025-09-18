using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZangiBar : MonoBehaviour
{
    [SerializeField] private GameObject _player; // �v���C���[�I�u�W�F�N�g�ւ̎Q��

    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _spSlider;

    void Start()
    {
        //Slider�𖞃^���ɂ���B
        _hpSlider.value = 1;
    }

    void Update()
    {
        int maxHp = _player.GetComponent<PlayerState>().GetMaxHp();
        int nowHp = _player.GetComponent<PlayerState>().GetNowHp();

        int maxSp = _player.GetComponent<PlayerState>().GetMaxSpecialChargeTime();
        int nowSp = _player.GetComponent<PlayerState>().GetNowSpecialChargeTime();

        // �X���C�_�[�Ɍ��݂�SP�𔽉f
        _spSlider.value = (float)nowSp / (float)maxSp;

        // �X���C�_�[�Ɍ��݂�HP�𔽉f
        _hpSlider.value = (float)nowHp / (float)maxHp;
    }
}
