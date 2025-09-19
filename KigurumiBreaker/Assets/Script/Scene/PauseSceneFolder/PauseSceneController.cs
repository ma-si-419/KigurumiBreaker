using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseSceneController : MonoBehaviour
{
    [SerializeField] private Button[] _button; //ボタン配列
    private int _currnetIndex = 0; //現在の選択ボタン
    private float _inputCooldown = 0.23f; //入力のクールダウン時間
    private float _lastInputTime = 0f; //最後の入力時間

    // Start is called before the first frame update
    void Start()
    {
        HighlightButton(_currnetIndex);
    }

    // Update is called once per frame
    void Update()
    {
        //ポーズ中は操作できないようにする
        if (BaseSceneController.instance.isOption) return;

        float vertical = Input.GetAxis("Vertical");

        //クールダウン処理
        if(Time.unscaledTime - _lastInputTime > _inputCooldown)
        {
            //上カーソル
            if (vertical > 0.5f)
            {
                _currnetIndex = (_currnetIndex - 1 + _button.Length) % _button.Length;
                HighlightButton(_currnetIndex);
                _lastInputTime = Time.unscaledTime;
            }
            //下カーソル
            else if (vertical < -0.5f)
            {
                _currnetIndex = (_currnetIndex + 1) % _button.Length;
                HighlightButton(_currnetIndex);
                _lastInputTime = Time.unscaledTime;
            }
        }

        //決定(Aボタン)
        if(Input.GetButtonDown("Submit"))
        {
            _button[_currnetIndex].onClick.Invoke();
            Debug.Log("Aボタンが押されました");
        }
    }

    //選択中のボタンをハイライト表示(仮)
    //もっと良い演出を考える(アルファでかな)
    private void HighlightButton(int index)
    {
        for(int i = 0; i < _button.Length; i++)
        {
            var colors = _button[i].colors;
            colors.normalColor = (i == index) ? Color.yellow : Color.white;
            _button[i].colors = colors;
        }
    }

    public void RestartButton()
    {
        BaseSceneController.instance.TogglePause();
        Debug.Log("ゲームを続けた");
    }

    public void OptionButton()
    {
        //オプション画面を開く
        BaseSceneController.instance.ToggleOption();
        Debug.Log("オプション画面へ");
    }

    public void TitleButton()
    {
        BaseSceneController.instance.TogglePause();
        BaseSceneController.instance.ChangeSceneWithFade(SceneType.TitleScene);
        Debug.Log("タイトルに戻る");
    }

}
