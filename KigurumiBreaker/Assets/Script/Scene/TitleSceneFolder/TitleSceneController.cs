using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleSceneController : MonoBehaviour
{
    //�ŏ��ɌĂ΂��
    public void Start()
    {
        // �ŏ��ɑI�΂��{�^����I����Ԃɂ���
        //EventSystem.current.SetSelectedGameObject(_firstSelected.gameObject);
    }

    public void OnStartGame()
    {
        //�Q�[���V�[����
        BaseSceneController.instance.ChangeSceneWithFade(SceneType.GameScene);
    }

    private void Update()
    {
        OnClick();
    }

    

    private void OnClick()
    {
        ////���N���b�N
        //if (Input.GetMouseButtonDown(0))
        //{
        //    //�Q�[���V�[����
        //    BaseSceneController.instance.ChangeSceneWithFade(SceneType.GameScene);
        //}
        ////�E�N���b�N
        //if (Input.GetMouseButtonDown(1))
        //{
        //    BaseSceneController.instance.TogglePause();
        //}


    }
}
