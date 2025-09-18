using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//�V�[���̎��
public enum SceneType
{
    //�V�[�����ƍ��킹�Ȃ��ƃ_��
    TitleScene,     //�^�C�g��
    GameScene,      //�Q�[�����
    ResultScene,    //���U���g
    PauseScene,     //�|�[�Y
    OptionScene,    //�I�v�V����
    LoadingScene,   //�񓯊����[�h�V�[��
}

public class BaseSceneController : MonoBehaviour
{
    //�V���O���g���p�̕ϐ�
    public static BaseSceneController instance { get; private set; }
    private bool _isPaused = false; //�|�[�Y�����ǂ���
    //private bool _isOption = false; //�I�v�V���������ǂ���

    //�t�F�[�h���
    [SerializeField] private CanvasGroup fadeCanvas;   //�t�F�[�h�p��UI
    [SerializeField] private float _fadeSpeed = 0.5f;   //�t�F�[�h�̑��x



    //�V�[���̎��
    private void Awake()
    {
        //���łɃC���X�^���X�����݂��Ă��邩�m�F����
        if (instance != null && instance != this)
        {
            Destroy(gameObject);    //�d�����Ă���ꍇ�͔j������
            return;
        }

        //���݂��Ă��Ȃ�������o�^����
        instance = this;
        //�V�[����؂�ւ��Ă��j�����Ȃ�
        DontDestroyOnLoad(gameObject);

        //�t�F�[�h�p��Canvas���ݒ肳��Ă��Ȃ�������T��
        //�q�I�u�W�F�N�g���������ߗv��Ȃ���
        //if (fadeCanvas != null)
        //{
        //    DontDestroyOnLoad(fadeCanvas.gameObject);
        //}

    }

    //���[�h�Ȃ��̃t�F�[�h�����؂�ւ�
    public void ChangeSceneWithFade(SceneType nextScene)
    {
        StartCoroutine(FadeSceneCoroutine(nextScene));
    }

    //�V�[���؂�ւ��̃R���[�`��
    private IEnumerator FadeSceneCoroutine(SceneType nextScene)
    {
        //�t�F�[�h�A�E�g
        yield return StartCoroutine(Fade(1f));

        //�V�[���؂�ւ�
        SceneManager.LoadScene(nextScene.ToString());

        //�V�[���؂�ւ���1�t���[���҂�
        yield return null;

        //fadeCanvas���Ď擾
        if(fadeCanvas == null)
        {
            fadeCanvas = FindObjectOfType<CanvasGroup>();
        }

        //�t�F�[�h�C��
        if(fadeCanvas != null)
        {
            yield return StartCoroutine(Fade(0f));
        }
    }

    //���[�h��ʕt���̐؂�ւ�
    //public void ChangeSceneWithLoading(SceneType nextScene)
    //{
    //    //StartCoroutine();
    //}

    //private IEnumerator LoadSceneCoroutine(SceneType nextType)
    //{



    //}


    //���ʃt�F�[�h����
    private IEnumerator Fade(float targetAlpha)
    {
        //���݂̃A���t�@�l
        float startAlpha = fadeCanvas.alpha;    //���̃A���t�@�l���擾
        float time = 0f;

        while (time < _fadeSpeed)
        {
            time += Time.unscaledDeltaTime;     //�|�[�Y���ł��i�߂�
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / _fadeSpeed);
            yield return null;
        }

        fadeCanvas.alpha = targetAlpha; //�Ō�ɖڕW�̃A���t�@�l�ɂ���
    }

    //�|�[�Y�̃I���I�t��؂�ւ���
    public void TogglePause()
    {
        if (_isPaused)
        {
            //�|�[�Y����
            SceneManager.UnloadSceneAsync(SceneType.PauseScene.ToString());
            Time.timeScale = 1f; //���Ԃ�߂�
            _isPaused = false;

            Debug.Log("�|�[�Y�I��");
        }
        else
        {
            //�|�[�Y�J�n
            SceneManager.LoadScene(SceneType.PauseScene.ToString(), LoadSceneMode.Additive);
            Time.timeScale = 0f; //���Ԃ��~�߂�
            _isPaused = true;

            Debug.Log("�|�[�Y�J�n");
        }
    }

}
