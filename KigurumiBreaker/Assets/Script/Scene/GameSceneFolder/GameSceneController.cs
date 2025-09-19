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
        //�|�[�Y���͑���ł��Ȃ��悤�ɂ���
        if (BaseSceneController.instance.isPaused) return;   

        //����(A�{�^��)
        if (Input.GetButtonDown("Submit"))
        {
            BaseSceneController.instance.ChangeSceneWithFade(SceneType.ResultScene);
            Debug.Log("A�{�^����������܂���");
        }

        if(Input.GetButtonDown("Start"))
        {
            BaseSceneController.instance.TogglePause();
        }
    }

}
