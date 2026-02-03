using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleSceneController : MonoBehaviour
{
    [SerializeField] private EventManager _eventManager;    //イベントマネージャー

    private bool _oneBotton;      //ボタンを一回だけ

    private float _timer = 0.0f;    //タイマー

    //最初に呼ばれる
    public void Start()
    {
        // 最初に選ばれるボタンを選択状態にする
        //EventSystem.current.SetSelectedGameObject(_firstSelected.gameObject);

        _oneBotton = false;
        _timer = 0.0f;
    }

    public void OnStartGame()
    {


    }

    private void Update()
    {


        _timer += 0.1f;


        if(_timer >=  25.0f)
        {
            //普通に時間が来たらボタンを押せるようにする
            //フェード完了時間を代入
            if (Input.GetButton("Submit") || Input.GetKey(KeyCode.A))
            {
                if (!_oneBotton && _eventManager.EventButton)
                {
                    AudioManager.Instance.StopBGM();
                    //ゲームシーンへ
                    BaseSceneController.instance.ChangeSceneWithFade(SceneType.GameScene);

                    Debug.Log(_oneBotton);

                    _oneBotton = true;
                }
            }
        }
        
    }
}
