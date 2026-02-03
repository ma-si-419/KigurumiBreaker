using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    [Header("プレイヤーのゲームオブジェクト")]
    [SerializeField] private PlayerState _player;

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
        ////ポーズ中は操作できないようにする
        //if (BaseSceneController.instance.isPaused) return;
        ////スキル選択中は操作できないようにする
        //if(BaseSceneController.instance.isSkillSelect) return;

        //// スタートボタンでポーズ
        //if (Input.GetButtonDown("Start"))
        //{
        //    BaseSceneController.instance.TogglePause();
        //}

        // プレイヤーのHPが0以下ならゲームオーバーへ
        if (_player.GetNowHp() <= 0 && !_isGameOver)
        {
            // ゲームオーバーシーンへ
            BaseSceneController.instance.ChangeSceneWithFade(SceneType.GameOverScene);
            _isGameOver = true;

            Debug.Log("Game Over");
        }

    }

}
