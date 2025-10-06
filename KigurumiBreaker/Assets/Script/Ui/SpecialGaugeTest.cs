using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecialGaugeTest : MonoBehaviour
{
    [SerializeField] private GameObject _player;               //プレイヤーオブジェクト

    [SerializeField] private Image _specialGaugeImage;           //ゲージ画像
    [SerializeField] private Image _hpGaugeImage;                //ゲージ画像

    //[SerializeField] private Image _auraImage;                 //オーラの画像
    [SerializeField] private Color _normalColor = Color.yellow;  //通常時の色
    [SerializeField] private Color _maxColor = Color.red;        //マックス時の色
    [SerializeField] private float _flashSpeed;                  //点滅速度    
    [SerializeField] private float _auraRotateSpeed;             //オーラの回転速度

    [SerializeField] private TMP_Text _shootText;               //弾の数表示用テキスト

    private float _current = 0f;    //現在のゲージ量
    private float _currentHp = 0f;    //現在のゲージ量
    private float _max = 100f;      //ゲージの最大量
    private float _maxHp = 100f;      //ゲージの最大量

    private int _shootNum = 1;      //プレイヤーの弾の数
    private int _shootMaxNum = 2;   //プレイヤーの弾の数の最大値

    private void Start()
    {
        //初期化
        //プレイヤーの弾の数
        //_shootNum = _player.GetComponent<PlayerState>().GetNowBulletNum();  
        //プレイヤーの弾の最大数
        //_shootMaxNum = _player.GetComponent<PlayerState>().GetNowBulletMaxNum();  

        //プレイヤーの現在のゲージ量を取得
        //_current = _player.GetComponent<PlayerState>().GetNowSpecialGauge();
        //プレイヤーの現在のHPを取得
        //_currentHp = _player.GetComponent<PlayerState>().GetNowHp();
    }

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

        // --- キーボード操作 ---
        if (Input.GetKey(KeyCode.RightArrow))    // → で増加
        {
            _currentHp += 30f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))  // ← で減少
        {
            _currentHp -= 30f * Time.deltaTime;
        }

        // 値の範囲を制限
        _current = Mathf.Clamp(_current, 0f, _max);
        _currentHp = Mathf.Clamp(_currentHp, 0f, _maxHp);

        // ゲージ反映
        float ratio = _current / _max;
        _specialGaugeImage.fillAmount = ratio;

        float ratio2 = _currentHp / _maxHp;
        _hpGaugeImage.fillAmount = ratio2;

        //Max時判定
        if (ratio >= 1f)
        {
            //点滅
            float flash = (Mathf.Sin(Time.time * _flashSpeed) + 1f) / 2f; // 0~1の点滅値
            _specialGaugeImage.color = Color.Lerp(_maxColor,Color.white, flash); // 点滅

            //オーラを表示

        }
        else
        {
            //通常色
            _specialGaugeImage.color = _normalColor;

            //オーラを非表示

        }

        //弾数を表示
        _shootText.text = _shootNum.ToString() + " / " + _shootMaxNum.ToString();

    }
}
