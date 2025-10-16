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

    private float _currentHp = 0f;    // 現在の体力
    private float _maxHp = 100f;      // 最大体力

    // Start is called before the first frame update
    private void Start()
    {
        // 初期化
        _playerState = _player.GetComponent<PlayerState>();
        _currentHp = _playerState.GetNowHp();
        _maxHp = _playerState.GetMaxHp();
    }

    // Update is called once per frame
    private void Update()
    {

        // 現在のHPと最大HPを取得
        _currentHp = _playerState.GetNowHp();
        _maxHp = _playerState.GetMaxHp();

        // HPゲージの更新
        _hpGaugeImage.fillAmount = _currentHp / _maxHp;
        // HPテキストの更新
        _hpText.text = ((int)_currentHp).ToString();
        _hpMaxText.text = ((int)_maxHp).ToString();

        // HPの割合に応じて色を変更
        float ratio = _currentHp / _maxHp;

        if (ratio > 0.5f)
        {
            _hpGaugeImage.color = Color.green;
        }
        else if (ratio > 0.2f)
        {
            _hpGaugeImage.color = Color.yellow;
        }
        else
        {
            _hpGaugeImage.color = Color.red;
        }
    }
}