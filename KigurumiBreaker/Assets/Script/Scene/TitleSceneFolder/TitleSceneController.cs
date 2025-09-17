using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneController : MonoBehaviour
{
    //最初に呼ばれる
    public void Start()
    {
    }

    private void Update()
    {
        OnClick();
    }

    

    private void OnClick()
    {
        //左クリック
        if (Input.GetMouseButtonDown(0))
        {
            //ゲームシーンへ
            BaseSceneController.instance.ChangeScene(SceneType.GameScene);
        }
        //右クリック
        if (Input.GetMouseButtonDown(1))
        {
            BaseSceneController.instance.TogglePause();
        }
    }
}
