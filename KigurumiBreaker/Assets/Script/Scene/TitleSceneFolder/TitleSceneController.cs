using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleSceneController : MonoBehaviour
{
    //最初に呼ばれる
    public void Start()
    {
        // 最初に選ばれるボタンを選択状態にする
        //EventSystem.current.SetSelectedGameObject(_firstSelected.gameObject);
    }

    public void OnStartGame()
    {
        //ゲームシーンへ
        BaseSceneController.instance.ChangeSceneWithFade(SceneType.GameScene);
    }

    private void Update()
    {
        OnClick();
    }

    

    private void OnClick()
    {
        ////左クリック
        //if (Input.GetMouseButtonDown(0))
        //{
        //    //ゲームシーンへ
        //    BaseSceneController.instance.ChangeSceneWithFade(SceneType.GameScene);
        //}
        ////右クリック
        //if (Input.GetMouseButtonDown(1))
        //{
        //    BaseSceneController.instance.TogglePause();
        //}


    }
}
