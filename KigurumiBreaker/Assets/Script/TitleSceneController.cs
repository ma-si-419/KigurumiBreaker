using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneController : BaseSceneController
{
    //ç≈èâÇ…åƒÇŒÇÍÇÈ
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
