using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultSceneFolder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //決定(Aボタン)
        if (Input.GetButtonDown("Submit"))
        {
            //BaseSceneController.instance.ChangeSceneWithFade(SceneType.TitleScene);
            Debug.Log("Aボタンが押されました");
        }

        if (Input.GetButtonDown("Start"))
        {
            BaseSceneController.instance.TogglePause();
        }
    }
}
