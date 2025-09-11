using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseSceneController : MonoBehaviour
{

    protected enum SceneKinds
    {
        Title,
        Game,
        Win,
        Lose,
        MaxNum,
    }

    //フェード情報

    //現在のシーンは何か


    //最初に呼ばれる
    protected virtual void Start()
    {
            
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
