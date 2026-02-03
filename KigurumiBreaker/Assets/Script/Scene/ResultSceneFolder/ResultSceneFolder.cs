using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultSceneFolder : MonoBehaviour
{
    private bool _oneButton = false;
    private float _time = 0.0f;
    private float _timer = 50.0f;

    // Start is called before the first frame update
    void Start()
    {
        //初期化
        _time = 0.0f;
        _oneButton = false;
    }

    // Update is called once per frame
    void Update()
    {

        _time += 0.1f;

        if (Input.GetButton("Submit") || Input.GetKey(KeyCode.A))
        {
            if (!_oneButton && _timer / 3.0f <= _time)
            {
                //BGM停止
                AudioManager.Instance.StopBGM();

                //タイトルシーンへ
                BaseSceneController.instance.ChangeSceneWithFade(SceneType.TitleScene);

                _oneButton = true;
            }
        }



        //自動でタイトルへ戻る
        if (_timer <= _time && !_oneButton)
        {
            //BGM停止
            AudioManager.Instance.StopBGM();

            //タイトルシーンへ
            BaseSceneController.instance.ChangeSceneWithFade(SceneType.TitleScene);

            _oneButton = true;
        }
    }
}
