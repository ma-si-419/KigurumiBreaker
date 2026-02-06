using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    [Header("プレイヤーのゲームオブジェクト")]
    [SerializeField] private PlayerState _player;
    [SerializeField] private int _maxTime = 0;
    private int _time = 0;

    private bool _isGameOver = false;

    void Start()
    {
        // プレイヤー参照が空なら自動で探す
        if (_player == null)
        {
            Debug.LogWarning($"{name}: Playerがシーンに見つかりませんでした！");
        }
    }

    // Update is called once per frame
    void Update()
    {

        // プレイヤーのHPが0以下ならゲームオーバーへ
        if (_player.GetNowHp() <= 0 && !_isGameOver)
        {
            // カウントアップ
            _time++;

            if (_time > _maxTime)
            {
                // ゲームオーバーシーンへ
                BaseSceneController.instance.ChangeSceneWithFade(SceneType.GameOverScene);
                _isGameOver = true;
            }

            Debug.Log("Game Over");
        }

    }

}
