using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialGaugeTest : MonoBehaviour
{
    [SerializeField] private Image specialGaugeImage;           //ゲージ画像
    [SerializeField] private Color normalColor = Color.yellow;  //通常時の色
    [SerializeField] private Color maxColor = Color.red;        //マックス時の色

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
        specialGaugeImage.fillAmount = ratio;

        // 色切り替え
        specialGaugeImage.color = (ratio >= 1f) ? maxColor : normalColor;
    }
}
