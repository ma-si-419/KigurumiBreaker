using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    [SerializeField] private CanvasGroup _fadeCanvas;
    [SerializeField] private ShakeCamera _shakeCamera;
    [SerializeField] private TitleCameraMove _titleCameraMove;
    [SerializeField] private BreakBlock _breakBlock;

    [SerializeField] private float _fadeSpeed = 0.8f;
    // Update is called once per frame
    void Update()
    {
        //壁に日々が入った瞬間に揺らす
        if (_breakBlock.breakMoment)
        {
            //カメラを揺らす
            StartCoroutine(_shakeCamera.MyShake(0.5f, 0.2f));
           
            //Debug.Log("シェイク");
        }

        //なんでもボタンを押したら
        //if()
        //壁にヒビが入る前にボタンを押すとムービーを飛ばせる
        //if(!_breakBlock.breakMoment)
        //{
        //    //ムービーを飛ばすコルーチンを呼ぶ
        //    StartCoroutine(FadeCoroutine());
        //}
    }

    //フェードのコルーチン
    //private IEnumerator FadeCoroutine()
    //{
    //    //フェードアウト
    //    yield return StartCoroutine(MyFade(1f));

    //    _titleCameraMove.isStop = true;

    //    //フェードイン
    //    if (_fadeCanvas == null)
    //    {
    //        yield return StartCoroutine(MyFade(0f));
    //    }
    //}

    //private IEnumerator MyFade(float targetAlpha)
    //{
    //    //現在のアルファ値
    //    float startAlpha = _fadeCanvas.alpha;    //アルファ値を取得
    //    float time = 0f;

    //    while(time < _fadeSpeed)
    //    {
    //        time += Time.unscaledDeltaTime;
    //        _fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / _fadeSpeed);
    //        yield return null;
    //    }

    //    _fadeCanvas.alpha = targetAlpha; 
    //}
}
