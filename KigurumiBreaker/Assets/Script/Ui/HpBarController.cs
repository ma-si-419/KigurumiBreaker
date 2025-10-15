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
        // プレイヤーの現在のHP状況を取得
        _currentHp = _playerState.GetNowHp();
        _maxHp = _playerState.GetMaxHp();

        // HPゲージの更新
        _hpGaugeImage.fillAmount = _currentHp / _maxHp;
        // HPテキストの更新
        _hpText.text = ((int)_currentHp).ToString();
        _hpMaxText.text = ((int)_maxHp).ToString();

        // HPゲージの色を変更
        float ratio = _currentHp / _maxHp;

        if (ratio > 0.5f)
        {
            _hpGaugeImage.color = Color.green; // HPが50%以上なら緑
        }
        else if (ratio > 0.2f)
        {
            _hpGaugeImage.color = Color.yellow; // HPが20%以上50%以下なら黄色
        }
        else
        {
            _hpGaugeImage.color = Color.red; // HPが20%以下なら赤
        }
    }
}
