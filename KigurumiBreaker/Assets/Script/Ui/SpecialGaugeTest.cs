using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialGaugeTest : MonoBehaviour
{
    [SerializeField] private Image _specialGaugeImage;           //ゲージ画像
    //[SerializeField] private Image _auraImage;                 //オーラの画像
    [SerializeField] private Color _normalColor = Color.yellow;  //通常時の色
    [SerializeField] private Color _maxColor = Color.red;        //マックス時の色
    [SerializeField] private float _flashSpeed;                  //点滅速度    
    [SerializeField] private float _auraRotateSpeed;             //オーラの回転速度


    private float _current = 0f;    //現在のゲージ量
    private float _max = 100f;      //ゲージの最大量

    // Update is called once per frame
    void Update()
    {
        // --- キーボード操作 ---
        if (Input.GetKey(KeyCode.UpArrow))    // ↑ で増加
        {
            _current += 30f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))  // ↓ で減少
        {
            _current -= 30f * Time.deltaTime;
        }


        // 値の範囲を制限
        _current = Mathf.Clamp(_current, 0f, _max);

        // ゲージ反映
        float ratio = _current / _max;
        _specialGaugeImage.fillAmount = ratio;

        //Max時判定
        if(ratio >= 1f)
        {
            //点滅
            float flash = (Mathf.Sin(Time.time * _flashSpeed) + 1f) / 2f; // 0~1の点滅値
            _specialGaugeImage.color = Color.Lerp(_maxColor,Color.white, flash); // 点滅

            //オーラを表示

        }

    }
}
