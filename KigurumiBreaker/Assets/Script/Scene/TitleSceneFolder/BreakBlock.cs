using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BreakBlock : MonoBehaviour
{
    [SerializeField] private RenderTexture renderTexture;
    private Camera renderCamera;

    // Start is called before the first frame update
    void Start()
    {
        renderCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();   //映像
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            StartCoroutine("TraceOn");                    //画面割れを始めたいタイミングでやる
    }

    IEnumerator Trace()
    {
        renderCamera.enabled = true;                      //投影開始
        renderCamera.targetTexture = renderTexture;       
        yield return null;
        renderCamera.targetTexture = null;                //全行程、完了
        renderCamera.enabled = false;
    }
}
