using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneController : MonoBehaviour
{
    //�ŏ��ɌĂ΂��
    public void Start()
    {
    }

    private void Update()
    {
        OnClick();
    }

    

    private void OnClick()
    {
        //���N���b�N
        if (Input.GetMouseButtonDown(0))
        {
            //�Q�[���V�[����
            BaseSceneController.instance.ChangeScene(SceneType.GameScene);
        }
        //�E�N���b�N
        if (Input.GetMouseButtonDown(1))
        {
            BaseSceneController.instance.TogglePause();
        }
    }
}
