using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneController : BaseSceneController
{
    //�ŏ��ɌĂ΂��
    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
    }

    private void OnClick()
    {
        ChangeScene("GameScene");
    }
}
