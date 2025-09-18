using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//シーンの種類
public enum SceneType
{
    //シーン名と合わせないとダメ
    TitleScene,     //タイトル
    GameScene,      //ゲーム画面
    ResultScene,    //リザルト
    PauseScene,     //ポーズ
    OptionScene,    //オプション
    LoadingScene,   //非同期ロードシーン
}

public class BaseSceneController : MonoBehaviour
{
    //シングルトン用の変数
    public static BaseSceneController instance { get; private set; }
    private bool _isPaused = false; //ポーズ中かどうか
    //private bool _isOption = false; //オプション中かどうか

    //フェード情報
    [SerializeField] private CanvasGroup fadeCanvas;   //フェード用のUI
    [SerializeField] private float _fadeSpeed = 0.5f;   //フェードの速度



    //シーンの種類
    private void Awake()
    {
        //すでにインスタンスが存在しているか確認する
        if (instance != null && instance != this)
        {
            Destroy(gameObject);    //重複している場合は破棄する
            return;
        }

        //存在していなかったら登録する
        instance = this;
        //シーンを切り替えても破棄しない
        DontDestroyOnLoad(gameObject);

        //フェード用のCanvasが設定されていなかったら探す
        //子オブジェクトだったため要らないす
        //if (fadeCanvas != null)
        //{
        //    DontDestroyOnLoad(fadeCanvas.gameObject);
        //}

    }

    //ロードなしのフェード高速切り替え
    public void ChangeSceneWithFade(SceneType nextScene)
    {
        StartCoroutine(FadeSceneCoroutine(nextScene));
    }

    //シーン切り替えのコルーチン
    private IEnumerator FadeSceneCoroutine(SceneType nextScene)
    {
        //フェードアウト
        yield return StartCoroutine(Fade(1f));

        //シーン切り替え
        SceneManager.LoadScene(nextScene.ToString());

        //シーン切り替え後1フレーム待つ
        yield return null;

        //fadeCanvasを再取得
        if(fadeCanvas == null)
        {
            fadeCanvas = FindObjectOfType<CanvasGroup>();
        }

        //フェードイン
        if(fadeCanvas != null)
        {
            yield return StartCoroutine(Fade(0f));
        }
    }

    //ロード画面付きの切り替え
    //public void ChangeSceneWithLoading(SceneType nextScene)
    //{
    //    //StartCoroutine();
    //}

    //private IEnumerator LoadSceneCoroutine(SceneType nextType)
    //{



    //}


    //共通フェード処理
    private IEnumerator Fade(float targetAlpha)
    {
        //現在のアルファ値
        float startAlpha = fadeCanvas.alpha;    //今のアルファ値を取得
        float time = 0f;

        while (time < _fadeSpeed)
        {
            time += Time.unscaledDeltaTime;     //ポーズ中でも進める
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / _fadeSpeed);
            yield return null;
        }

        fadeCanvas.alpha = targetAlpha; //最後に目標のアルファ値にする
    }

    //ポーズのオンオフを切り替える
    public void TogglePause()
    {
        if (_isPaused)
        {
            //ポーズ解除
            SceneManager.UnloadSceneAsync(SceneType.PauseScene.ToString());
            Time.timeScale = 1f; //時間を戻す
            _isPaused = false;

            Debug.Log("ポーズ終了");
        }
        else
        {
            //ポーズ開始
            SceneManager.LoadScene(SceneType.PauseScene.ToString(), LoadSceneMode.Additive);
            Time.timeScale = 0f; //時間を止める
            _isPaused = true;

            Debug.Log("ポーズ開始");
        }
    }

}
