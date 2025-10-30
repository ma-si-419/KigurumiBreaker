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
        //BaseSceneController.instance.ChangeSceneWithFade(SceneType.GameScene);
        BaseSceneController.instance.ChangeSceneWithFade(SceneType.ResultScene);
    }

    private void Update()
    {
        //普通に時間が来たらボタンを押せるようにする
        //フェード完了時間を代入

        OnClick();
    }

    

    private void OnClick()
    {

    }
}
