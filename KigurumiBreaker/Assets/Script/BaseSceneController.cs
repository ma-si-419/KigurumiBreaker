using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseSceneController : MonoBehaviour
{

    //ç≈èâÇ…åƒÇŒÇÍÇÈ
    protected virtual void Start()
    {
        
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
