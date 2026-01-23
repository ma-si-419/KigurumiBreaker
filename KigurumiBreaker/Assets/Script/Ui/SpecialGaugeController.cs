using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecialGaugeController : MonoBehaviour
{
    [SerializeField] private GameObject _player;                 //プレイヤーオブジェクト

    [SerializeField] private Image _gauge1;                      //ゲージ画像1
    [SerializeField] private Image _gauge2;                      //ゲージ画像2
    [SerializeField] private Image _gauge3;                      //ゲージ画像3
    [SerializeField] private Image _gauge4;                      //ゲージ画像4
    [SerializeField] private Image _auraImage;                   //オーラの画像

    [SerializeField] private SpecialGaugeUiData _specialGaugeUiData; //ゲージUIデータ

    private PlayerState _playerState;

    private float _currentGauge = 0f;                            //現在のゲージ量
    private float _maxGauge = 100f;                              //ゲージの最大量

    private void Start()
    {
        _playerState = _player.GetComponent<PlayerState>();

        _currentGauge = _playerState.GetNowSpecialChargeNum();
        _maxGauge = _playerState.GetMaxSpecialChargeNum();
    }

    // Update is called once per frame
    void Update()
    {
        //現在のゲージ量を取得
        _currentGauge = _playerState.GetNowSpecialChargeNum();

        //ゲージの割合を計算
        float fillAmount = Mathf.Clamp01(_currentGauge / _maxGauge);

        // 25%ごとに埋める画像を変更
        if (fillAmount <= 0.25f)
        {
            _gauge1.fillAmount = fillAmount / 0.25f;
            _gauge2.fillAmount = 0f;
            _gauge3.fillAmount = 0f;
            _gauge4.fillAmount = 0f;

            // ゲージの色を変更
            _gauge1.color = _specialGaugeUiData.normalColor;
            _gauge2.color = _specialGaugeUiData.normalColor;
            _gauge3.color = _specialGaugeUiData.normalColor;
            _gauge4.color = _specialGaugeUiData.normalColor;
        }
        else if (fillAmount <= 0.5f)
        {
            _gauge1.fillAmount = 1f;
            _gauge2.fillAmount = (fillAmount - 0.25f) / 0.25f;
            _gauge3.fillAmount = 0f;
            _gauge4.fillAmount = 0f;

            // ゲージの色を変更
            _gauge1.color = _specialGaugeUiData.maxColor;
        }
        else if (fillAmount <= 0.75f)
        {
            _gauge1.fillAmount = 1f;
            _gauge2.fillAmount = 1f;
            _gauge3.fillAmount = (fillAmount - 0.5f) / 0.25f;
            _gauge4.fillAmount = 0f;

            // ゲージの色を変更
            _gauge1.color = _specialGaugeUiData.maxColor;
            _gauge2.color = _specialGaugeUiData.maxColor;
        }
        else
        {
            _gauge1.fillAmount = 1f;
            _gauge2.fillAmount = 1f;
            _gauge3.fillAmount = 1f;
            _gauge4.fillAmount = (fillAmount - 0.75f) / 0.25f;

            // ゲージの色を変更
            _gauge1.color = _specialGaugeUiData.maxColor;
            _gauge2.color = _specialGaugeUiData.maxColor;
            _gauge3.color = _specialGaugeUiData.maxColor;
        }

        if (fillAmount == 1.0f)
        {
            _gauge4.color = _specialGaugeUiData.maxColor;

            _auraImage.color = _specialGaugeUiData.auraMaxColor;
        }
        else
        {
            _auraImage.color = _specialGaugeUiData.auraNormalColor;
        }


        /*
                //ゲージが最大値に達した場合、オーラを表示して点滅させる
                if (_currentGauge >= _maxGauge)
                {
                    _auraImage.gameObject.SetActive(true);
                    _auraImage2.gameObject.SetActive(true);

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
        */
    }


}
