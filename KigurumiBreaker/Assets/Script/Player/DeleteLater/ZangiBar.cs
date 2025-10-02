using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZangiBar : MonoBehaviour
{
    [SerializeField] private GameObject _player; // プレイヤーオブジェクトへの参照

    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _spSlider;
    [SerializeField] private Slider _bulletSlider;

    private PlayerState _playerState;

    void Start()
    {
        // PlayerStateコンポーネントを取得
        _playerState = _player.GetComponent<PlayerState>();

    }

    void Update()
    {
        int maxHp = _playerState.GetMaxHp();
        int nowHp = _playerState.GetNowHp();

        float maxSp = _playerState.GetMaxSpecialChargeTime();
        float nowSp = _playerState.GetNowSpecialChargeTime();

        int maxBullet = _playerState.GetMaxBulletNum();
        int nowBullet = _playerState.GetNowBulletNum();

        // スライダーに現在のSPを反映
        _spSlider.value = nowSp / maxSp;

        // スライダーに現在のHPを反映
        _hpSlider.value = (float)nowHp / (float)maxHp;

        // スライダーに現在の弾数を反映
        _bulletSlider.value = (float)nowBullet / (float)maxBullet;
    }
}
