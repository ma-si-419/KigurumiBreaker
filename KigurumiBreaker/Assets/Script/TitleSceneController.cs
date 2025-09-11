using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneController : BaseSceneController
{
    //Å‰‚ÉŒÄ‚Î‚ê‚é
    protected override void Start()
    {
        base.Start();
    }

    public void OnClick()
    {
        ChangeScene("TestSceneTransition");
    }
}
