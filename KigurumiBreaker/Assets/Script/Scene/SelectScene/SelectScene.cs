using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

# if UNITY_EDITOR
using Microsoft.Unity.VisualStudio.Editor;
#endif

public class SelectScene : MonoBehaviour
{
    //仮の選択オブジェクト
    [SerializeField] private GameObject _selectObject;    //選択で動くオブジェク
    [SerializeField] private List<GameObject> _menuUI;    //メニュー項目をリスト化
    [SerializeField] private float _inputDelay = 0.25f;   //入力受付の遅延
    private int _index = 0;                               //選択中のインデックス
    private float _lastInputTime;                         //最後に入力を受け付けた時間
    private float _joyStickL;                             //Lスティックの入力取得

    private int _bottonCount;                             //最終手段

    private bool _oneBotton;                              //一回ボタンが押されたかどうか

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("死ね");

        var menu = _menuUI[0].GetComponent<UnityEngine.UI.Image>();
        menu.color = Color.red;
        _oneBotton = false;
        _bottonCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //オプション中は操作できないようにする
        if (BaseSceneController.instance.isOption) return;

        _joyStickL = Input.GetAxis("Vertical");

        if (Time.time - _lastInputTime > _inputDelay)
        {
            //上選択
            if (_joyStickL >= 0.5)
            {
                MyMoveSelection(-1);
            }
            //下選択
            if (_joyStickL <= -0.5)
            {
                MyMoveSelection(1);
            }
        }

        //決定
        if (Input.GetButton("Submit") && !_oneBotton && _bottonCount == 0)
        {
            _oneBotton = true;

            MyOnSelect(_menuUI[_index]);

        }

    }

    private void MyMoveSelection(int direction)
    {
        _lastInputTime = Time.time;
        //インデックス更新
        _index += direction;

        //インデックスの範囲制限
        if (_index < 0) _index = _menuUI.Count - 1;
        else if (_index >= _menuUI.Count) _index = 0;

        //選択オブジェクト位置更新
        UpdateSelection();
    }


    /// <summary>
    /// 視覚的な選択更新
    /// </summary>
    private void UpdateSelection()
    {
        for (int i = 0; i < _menuUI.Count; i++)
        {
            var menu = _menuUI[i].GetComponent<UnityEngine.UI.Image>();
            if (menu != null)
            {
                menu.color = (i == _index) ? Color.red : Color.white;
            }
        }
    }

    /// <summary>
    /// 選択決定時の処理
    /// </summary>
    /// <param name="select"></param>
    private void MyOnSelect(GameObject select)
    {

        //一回だけ反応させる
        _oneBotton = true;

        Debug.Log(select);

        //プレイボタン
        if (select == _menuUI[0])
        {
            //安田オリジナルシーン遷移でゲームシーンへ
            BaseSceneController.instance.ChangeSceneWithFade(SceneType.GameScene);
            //BaseSceneController.instance.ChangeSceneWithFade(SceneType.TestYoshiyama_2);

            _bottonCount++;
        }

        //オプションボタン
        if (select == _menuUI[1])
        {
            //安田オリジナルシーン遷移でオプションシーンへ
            BaseSceneController.instance.ToggleOption();

            Debug.Log(_oneBotton);

            _oneBotton = true;
        }

        //終了ボタン
        if (select == _menuUI[2] && !_oneBotton)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
        }
    }
}
