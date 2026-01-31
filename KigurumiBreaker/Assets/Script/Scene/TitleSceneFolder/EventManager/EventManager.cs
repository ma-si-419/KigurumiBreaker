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

    private bool _oneButton = false;        //ボタンを一回反応させる    

    public bool EventButton = false;               //イベント完了変数

    // Update is called once per frame
    void Update()
    {
        //壁に日々が入った瞬間に揺らす
        if (_breakBlock.breakMoment && EventButton == false)
        {
            //Seを鳴らす
            AudioManager.Instance.PlaySE(SoundID.WallBreak);

            //カメラを揺らす
            StartCoroutine(_shakeCamera.MyShake(0.5f, 0.2f));

            //イベント完了にする
            EventButton = true;

            //BGMを鳴らす
            AudioManager.Instance.PlaySE(SoundID.Title);
        }

        //なんでもボタンを押したら
        if(!_oneButton)
        {
            if(Input.GetButton("Submit") || Input.GetKey(KeyCode.A))
            {
                //壁にヒビが入る前にボタンを押すとムービーを飛ばせる
                if (!_breakBlock.breakMoment)
                {
                    //ムービーを飛ばすコルーチンを呼ぶ
                    StartCoroutine(FadeCoroutine());
                }
                //一回だけ押す
                _oneButton = true;
            }
            
        }
        
    }

    //フェードのコルーチン
    private IEnumerator FadeCoroutine()
    {
        //フェードアウト
        yield return StartCoroutine(MyFade(1f));

        _titleCameraMove.isStop = true;
        _breakBlock.breakMoment = true;

        //fadeCanvasを再取得
        if(_fadeCanvas == null)
        {
            _fadeCanvas = FindObjectOfType<CanvasGroup>();
        }

        //フェードイン
        if (_fadeCanvas != null)
        {
            //ムービースキップ後のフェードイン
            yield return StartCoroutine(MyFade(0f));
        }
    }

    private IEnumerator MyFade(float targetAlpha)
    {
        //現在のアルファ値
        float startAlpha = _fadeCanvas.alpha;    //アルファ値を取得
        float time = 0f;

        while (time < _fadeSpeed)
        {
            time += Time.unscaledDeltaTime;
            _fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / _fadeSpeed);
            yield return null;
        }

        _fadeCanvas.alpha = targetAlpha; //目標のアルファ値にする
    }
}
