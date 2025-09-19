using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ポーズ中は操作できないようにする
        if (BaseSceneController.instance.isPaused) return;   

        //決定(Aボタン)
        if (Input.GetButtonDown("Submit"))
        {
            BaseSceneController.instance.ChangeSceneWithFade(SceneType.ResultScene);
            Debug.Log("Aボタンが押されました");
        }

        if(Input.GetButtonDown("Start"))
        {
            BaseSceneController.instance.TogglePause();
        }
    }

}
