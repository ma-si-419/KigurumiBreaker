using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HpBarController : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    [SerializeField] private Image _hpGaugeImage;

    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private TMP_Text _hpMaxText;

    private PlayerState _playerState;

    private float _currentHp = 0f;    //現在のゲージ量
    private float _maxHp = 100f;      //ゲージの最大量

    private void Start()
    {
        _playerState = _player.GetComponent<PlayerState>();
        //プレイヤーの現在のHPを取得
        _currentHp = _playerState.GetNowHp();
        //プレイヤーのHPの最大値を取得
        _maxHp = _playerState.GetMaxHp();
    }

    private void Update()
    {
        //プレイヤーの現在のHPを取得
        _currentHp = _playerState.GetNowHp();
        _maxHp = _playerState.GetMaxHp();
        //HPゲージの更新
        _hpGaugeImage.fillAmount = _currentHp / _maxHp;
        //テキストの更新
        _hpText.text = ((int)_currentHp).ToString();
        _hpMaxText.text = ((int)_maxHp).ToString();
    }
}
