using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecialGaugeController : MonoBehaviour
{
    [SerializeField] private GameObject _player;                 //プレイヤーオブジェクト

    [SerializeField] private Image _specialGaugeImage;           //ゲージ画像
    [SerializeField] private Image _auraImage;                   //オーラの画像
    [SerializeField] private Image _auraImage2;                  //オーラの画像

    [SerializeField] private SpecialGaugeUiData _specialGaugeUiData; //ゲージUIデータ

    private PlayerState _playerState;

    private float _currentGauge = 0f;                            //現在のゲージ量
    private float _maxGauge = 100f;                              //ゲージの最大量

    private void Start()
    {
        _playerState = _player.GetComponent<PlayerState>();

        _currentGauge = _playerState.GetNowSpecialChargeNum();
        _maxGauge = _playerState.GetMaxSpecialChargeNum();

        _auraImage.gameObject.SetActive(false); //オーラを非表示
        _auraImage2.gameObject.SetActive(false); //オーラを非表示
    }

    // Update is called once per frame
    void Update()
    {
        //現在のゲージ量を取得
        _currentGauge = _playerState.GetNowSpecialChargeNum();

        //ゲージの割合を計算
        float fillAmount = Mathf.Clamp01(_currentGauge / _maxGauge);
        _specialGaugeImage.fillAmount = fillAmount;
        //ゲージの色を変更
        _specialGaugeImage.color = Color.Lerp(_specialGaugeUiData.normalColor, _specialGaugeUiData.maxColor, fillAmount);
        //ゲージが最大値に達した場合、オーラを表示して点滅させる
        if (_currentGauge >= _maxGauge)
        {
            _auraImage.gameObject.SetActive(true);
            _auraImage2.gameObject.SetActive(true);

            //色を設定
            float flash = (Mathf.Sin(Time.time * _specialGaugeUiData.flashSpeed) + 1f) / 2f; // 0~1の点滅値
            _specialGaugeImage.color = Color.Lerp(_specialGaugeUiData.maxColor, Color.white, flash); // 点滅

            //オーラを回転させる
            _auraImage.transform.Rotate(0f, 0f, _specialGaugeUiData.auraRotateSpeed * Time.deltaTime);
            _auraImage2.transform.Rotate(0f, 0f, -_specialGaugeUiData.auraRotateSpeed * Time.deltaTime); //逆回転
        }
        else
        {
            //ゲージが最大値に達していない場合、オーラを非表示にする
            _auraImage.gameObject.SetActive(false);
            _auraImage2.gameObject.SetActive(false);
        }
    }

}
