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

    public void OnClick()
    {
        ChangeScene("TestSceneTransition");
    }
}
