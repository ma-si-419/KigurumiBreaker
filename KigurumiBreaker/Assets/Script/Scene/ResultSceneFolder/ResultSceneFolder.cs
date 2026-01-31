using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultSceneFolder : MonoBehaviour
{
    private bool _oneButton = false;
    private float _timer = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        _oneButton = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Submit") || Input.GetKey(KeyCode.A))
        {
            if (!_oneButton)
            {
                //BGM停止
                AudioManager.Instance.StopBGM();

                //タイトルシーンへ
                BaseSceneController.instance.ChangeSceneWithFade(SceneType.TitleScene);

                _oneButton = true;
            }
        }

        float time = Time.time;

        //自動でタイトルへ戻る
        if(_timer <= time)
        {
            //BGM停止
            AudioManager.Instance.StopBGM();

            //タイトルシーンへ
            BaseSceneController.instance.ChangeSceneWithFade(SceneType.TitleScene);
        }

       

        ////決定(Aボタン)
        //if (Input.GetButtonDown("Submit"))
        //{
        //    //BaseSceneController.instance.ChangeSceneWithFade(SceneType.TitleScene);
        //    Debug.Log("Aボタンが押されました");
        //}

        //if (Input.GetButtonDown("Start"))
        //{
        //    BaseSceneController.instance.TogglePause();
        //}
    }
}
