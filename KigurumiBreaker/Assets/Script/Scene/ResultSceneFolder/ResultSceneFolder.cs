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
        OnClick();
    }

    private void OnClick()
    {
        //���N���b�N
        if (Input.GetMouseButtonDown(0))
        {
            //�Q�[���V�[����
            BaseSceneController.instance.ChangeSceneWithFade(SceneType.TitleScene);
        }
        //�E�N���b�N
        if (Input.GetMouseButtonDown(1))
        {
            BaseSceneController.instance.TogglePause();
        }
    }
}
