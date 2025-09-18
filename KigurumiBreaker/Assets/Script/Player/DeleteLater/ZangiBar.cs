using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZangiBar : MonoBehaviour
{
    [SerializeField] private GameObject _player; // プレイヤーオブジェクトへの参照

    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _spSlider;

    void Start()
    {
        //Sliderを満タンにする。
        _hpSlider.value = 1;
    }

    void Update()
    {
        int maxHp = _player.GetComponent<PlayerState>().GetMaxHp();
        int nowHp = _player.GetComponent<PlayerState>().GetNowHp();

        int maxSp = _player.GetComponent<PlayerState>().GetMaxSpecialChargeTime();
        int nowSp = _player.GetComponent<PlayerState>().GetNowSpecialChargeTime();

        // スライダーに現在のSPを反映
        _spSlider.value = (float)nowSp / (float)maxSp;

        // スライダーに現在のHPを反映
        _hpSlider.value = (float)nowHp / (float)maxHp;
    }
}
