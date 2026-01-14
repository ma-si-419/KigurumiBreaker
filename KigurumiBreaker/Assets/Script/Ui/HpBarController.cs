using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBarController : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    // 現在のHpを表示するImage
    [SerializeField] private Image _currentHpGaugeImage;
    // 遅れて減る赤バー
    [SerializeField] private Image _delayedHpGaugeImg;

    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private TMP_Text _shadowHpText;

    private PlayerState _playerState;

    // 硬直時間
    private float _delayRigidityTime = 0.5f;
    // 補間速度
    private float _lerpSpeed = 1.5f;

    // コルーチン管理用
    private Coroutine _hpCoroutine;

    private float _currentHp = 0f;    // 現在の体力
    private float _maxHp = 100f;      // 最大体力

    // Start is called before the first frame update
    private void Start()
    {
        // 初期化
        _playerState = _player.GetComponent<PlayerState>();
        _currentHp = _playerState.GetNowHp();

        _currentHpGaugeImage.fillAmount = _playerState.GetNowHp() / _playerState.GetMaxHp();
        _delayedHpGaugeImg.fillAmount = _currentHpGaugeImage.fillAmount;

    }

    // Update is called once per frame
    private void Update()
    {
        // 現在のHPと最大HPを取得
        _currentHp = _playerState.GetNowHp();
        _maxHp = _playerState.GetMaxHp();

        float targetHpRatio = _currentHp / _maxHp;

        // HPゲージの更新
        //_currentHpGaugeImage.fillAmount = _currentHp / _maxHp;

        // HPテキストの更新
        _hpText.text = ((int)_currentHp).ToString();
        _shadowHpText.text = ((int)_currentHp).ToString();

        // 現在のHpが目標値と異なる場合
        if (_currentHpGaugeImage.fillAmount != targetHpRatio)
        {
            // 即座に減らす
            _currentHpGaugeImage.fillAmount = targetHpRatio;

            // 遅延バーの減少をストップ
            if (_hpCoroutine != null) StopCoroutine(_hpCoroutine);

            // 遅延バーの減少を開始
            _hpCoroutine = StartCoroutine(HpDelayDecrease(targetHpRatio));

        }

        // HPの割合に応じて色を変更
        float ratio = _currentHp / _maxHp;

        if (ratio > 0.5f)
        {
            Color color = new Color32(71, 200, 100, 255);
            _currentHpGaugeImage.color = color;
        }
        else if (ratio > 0.2f)
        {
            _currentHpGaugeImage.color = new Color32(229, 145, 42, 255);
        }
        else
        {
            _currentHpGaugeImage.color = new Color32(229, 49, 49, 255);
        }
    }

    private IEnumerator HpDelayDecrease(float target)
    {
        // 1秒硬直
        yield return new WaitForSeconds(_delayRigidityTime);

        // ゆっくり減少
        while (_delayedHpGaugeImg.fillAmount > target)
        {
            _delayedHpGaugeImg.fillAmount -= Time.deltaTime * _lerpSpeed;
            yield return null;
        }

        // 目標値に合わせる
        _delayedHpGaugeImg.fillAmount = target;
    }
}