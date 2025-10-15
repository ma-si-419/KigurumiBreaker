using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BulletNumUiController : MonoBehaviour
{

    [SerializeField] private TMP_Text _shootNumText;
    [SerializeField] private TMP_Text _shootNumMaxText;
    [SerializeField] private GameObject _player;

    private int _shootNum;      //プレイヤーの弾の数
    private int _shootMaxNum;   //プレイヤーの弾の数の最大値

    private PlayerState _playerState;

    void Start()
    {
        _playerState = _player.GetComponent<PlayerState>();
        //初期化
        //プレイヤーの弾の数
        _shootNum = _playerState.GetNowBulletNum();
        //プレイヤーの弾の最大数
        _shootMaxNum = _playerState.GetMaxBulletNum();
        //テキストに反映
        _shootNumText.text = _shootNum.ToString();
        _shootNumMaxText.text = _shootMaxNum.ToString();
    }

    void Update()
    {
        //プレイヤーの弾の数
        _shootNum = _playerState.GetNowBulletNum();
        //プレイヤーの弾の最大数
        _shootMaxNum = _playerState.GetMaxBulletNum();
        //テキストに反映
        _shootNumText.text = _shootNum.ToString();
        _shootNumMaxText.text = _shootMaxNum.ToString();
    }
}
