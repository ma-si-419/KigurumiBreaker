using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    [SerializeField] private int _maxTime; //制限時間
    private int _time; //制限時間

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _time++;

        //ポーズ中は操作できないようにする
        if (BaseSceneController.instance.isPaused) return;
        //スキル選択中は操作できないようにする
        if(BaseSceneController.instance.isSkillSelect) return;

        if(_time > _maxTime)
        {
            //決定(Aボタン)
            if (Input.GetButtonDown("Submit"))
            {
                BaseSceneController.instance.ChangeSceneWithFade(SceneType.ResultScene);
                Debug.Log("Aボタンが押されました");
            }
        }

        if(Input.GetButtonDown("Start"))
        {
            BaseSceneController.instance.TogglePause();
        }
    }

}
