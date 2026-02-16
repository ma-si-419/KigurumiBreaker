using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class StageSet
{
    public enum StageKind
    {
        [InspectorName("自宅")]
        Home,
        [InspectorName("チュートリアル")]
        Tutorial,
        [InspectorName("ステージ1　森")]
        Forest,
        [InspectorName("ステージ1　Boss")]
        Forest_Boss,
        [InspectorName("ステージ2　洞窟")]
        Cave,
        [InspectorName("ステージ2　Boss")]
        Cave_Boss,
    }

    private const int SECTIONS_MAX = 5;

    [Header("ステージ情報")]
    [SerializeField] private StageKind _stageKind;

    [HideInInspector][SerializeField] private int _index;

    [Header("出てくるステージの種類")]
    public GameObject[] stagePrefabs;
    public void SetIndex(int index)
    {
        _index = index;
    }

    // 読み取り専用プロパティ
    public StageKind stageKind => _stageKind;
}

public class StageSpawner : MonoBehaviour
{
    [Header("全体のステージすべて")]
    [SerializeField] private StageSet[] _stageSets;
    [Header("プレイヤーのTransform")]
    [SerializeField] private Transform _player;

    private string _beforeSkill;
    private string _afterSkill;
    private List<SkillData.SkillElement> _acquiredSkills = new List<SkillData.SkillElement>();

    [Header("スキル選択マネージャー")]
    [SerializeField] private SkillSelectManager _skillSelectManager;

    [Header("バトルマネージャー")]
    [SerializeField] private BattleManager _battleManager;

    [Header("ムービーマネージャー")]
    [SerializeField] private MovieManager _movieManager;


    private int _currentStageIndex = 0;
    private GameObject _currentStageInstance;
    private int _waveStageIndex = 0;
    private StageSet.StageKind _previousStageKind;

    private Image fadeImage;  // 自動生成 or 既存のImageを使用
    private Coroutine fadeRoutine;
    private static bool isFirstLoad = true;
    private StageSet.StageKind? _currentStageKind = null;


    //過去に選ばれたPrefabのインデックスリスト
    private Dictionary<int, HashSet<int>> _usedPrefabs = new Dictionary<int, HashSet<int>>();


    // ステージデータのインデックスを設定
    private void OnValidate()
    {
        ///// ステージデータの設定 /////
        // ステージ森から始める
        StageSet.StageKind stageKind = StageSet.StageKind.Forest;

        // ステージごとにセクションを設定
        int sectionNumber = 0;

        for (int i = 0; i < _stageSets.Length; i++)
        {
            if (_stageSets[i] != null)
            {
                // ステージの種類が変わったらステージ番号を更新し、セクション番号をリセット
                if (stageKind != _stageSets[i].stageKind)
                {
                    stageKind = _stageSets[i].stageKind;
                    sectionNumber = 0;
                }

                sectionNumber++;
                // ステージ1から始めたいので+1する
                _stageSets[i].SetIndex(sectionNumber);
            }
        }
        ////////////////////////////////

    }

    /// <summary>
    /// 指定したインデックスのステージを生成する
    /// </summary>
    /// <param name="index"></param>
    public void SpawnStage(int index)
    {
        if (index < 0 || index >= _stageSets.Length) return;

        StageSet nextStage = _stageSets[index];
        bool isBossStage = IsBossStage(nextStage.stageKind);

        if (isFirstLoad)
        {
            isFirstLoad = false;

            if (isBossStage)
            {
                PlayBossMovieThenSpawn(index);
            }
            else
            {
                SpawnStageInternal(index);
            }
        }
        else
        {
            FadeOut(1.0f, () =>
            {
                if (isBossStage)
                {
                    PlayBossMovieThenSpawn(index);
                }
                else
                {
                    SpawnStageInternal(index);
                    FadeIn(1.0f);
                }
            });
        }

        _currentStageIndex = index;
    }


    // 実際のステージ生成処理
    private void SpawnStageInternal(int index)
    {
        // === 前ステージの削除 ===
        if (_currentStageInstance != null)
        {
            var oldSurfaces = _currentStageInstance.GetComponentsInChildren<NavMeshSurface>();
            foreach (var s in oldSurfaces)
                s.RemoveData();

            Destroy(_currentStageInstance);
            _battleManager.OnMoveStage();
        }

        // === 新ステージ生成 ===
        StageSet stageSet = _stageSets[index];

        if (stageSet.stageKind != _previousStageKind)
            _waveStageIndex = 0;
        else
            _waveStageIndex++;

        _previousStageKind = stageSet.stageKind;

        if (!_usedPrefabs.ContainsKey(index))
            _usedPrefabs[index] = new HashSet<int>();

        HashSet<int> used = _usedPrefabs[index];
        List<int> availableIndexes = new List<int>();

        for (int i = 0; i < stageSet.stagePrefabs.Length; i++)
            if (!used.Contains(i)) availableIndexes.Add(i);

        if (availableIndexes.Count == 0)
        {
            used.Clear();
            for (int i = 0; i < stageSet.stagePrefabs.Length; i++)
                availableIndexes.Add(i);
        }

        int prefabIndex = availableIndexes[Random.Range(0, availableIndexes.Count)];
        used.Add(prefabIndex);

        _currentStageInstance = Instantiate(stageSet.stagePrefabs[prefabIndex]);

        var cameraMoveArea =
    _currentStageInstance.GetComponentInChildren<CapsuleCollider>();

        if (cameraMoveArea != null)
        {
            Camera.main
                .GetComponent<CameraMove>()
                .SetMoveArea(cameraMoveArea);
        }


        var newSurfaces = _currentStageInstance.GetComponentsInChildren<NavMeshSurface>();
        foreach (var surface in newSurfaces)
        {
            surface.RemoveData();
            surface.BuildNavMesh();
        }

        Transform spawnPoint = _currentStageInstance.transform.Find("SpawnPoint");
        if (spawnPoint != null && _player != null)
        {
            _player.position = spawnPoint.position;
            _player.rotation = spawnPoint.rotation;
        }

        WaveSpawner[] waveSpawners = _currentStageInstance.GetComponentsInChildren<WaveSpawner>();
        foreach (var waveSpawner in waveSpawners)
        {
            waveSpawner.SetSkillSelect(_skillSelectManager);
            waveSpawner.SetBattleManager(_battleManager);

            if (!string.IsNullOrEmpty(_beforeSkill))
                waveSpawner.SetBeforeSkill(_beforeSkill);

            waveSpawner.SetStageSpawner(this);
        }
    }

    /// <summary>
    /// 次のステージを生成する
    /// </summary>
    public void NextStage()
    {
        int nextIndex = _currentStageIndex + 1;

        if (nextIndex >= _stageSets.Length)
        {
            AudioManager.Instance.FadeOutBGM(1.0f);
            BaseSceneController.instance.ChangeSceneWithFade(SceneType.ResultScene);
            return;
        }

        StageSet nextStage = _stageSets[nextIndex];

        // StageKindが変わったときだけBGM切り替え
        if (!_currentStageKind.HasValue || _currentStageKind.Value != nextStage.stageKind)
        {
            AudioManager.Instance.FadeOutAndChangeBGM(nextStage.stageKind, 1.0f);
            _currentStageKind = nextStage.stageKind;
        }

        SpawnStage(nextIndex);
    }




    private void Start()
    {
        _waveStageIndex = -1; //初期化を兼ねて-1に設定 
        SpawnStage(0);

        StageSet firstStage = _stageSets[0];
        _currentStageKind = firstStage.stageKind;
        AudioManager.Instance.ChangeBGMByStageKind(firstStage.stageKind);
    }

    /// <summary>
    /// WaveSpawner から通知される
    /// </summary>
    /// <param name="selectedSkill"></param>
    public void OnPathSelected(SkillData.SkillElement selectedSkill)
    {
        _beforeSkill = selectedSkill.ToString();
    }
    // フェード用の Canvas + Image を自動生成
    private void SetupFadeImage()
    {
        // Canvas
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;
        DontDestroyOnLoad(canvasObj);

        // Image
        GameObject imgObj = new GameObject("FadeImage");
        imgObj.transform.SetParent(canvasObj.transform, false);
        fadeImage = imgObj.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);

        RectTransform rt = fadeImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
    public void FadeOut(float duration, System.Action onComplete = null)
    {
        Fade(duration, 1f, null, onComplete);
    }

    public void FadeIn(float duration, System.Action onComplete = null)
    {
        Fade(duration, 0f, null, onComplete);
    }

    // コールバックを受け取るFade
    private void Fade(float duration, float targetAlpha, Color? color, System.Action onComplete)
    {
        if (fadeImage == null) SetupFadeImage();

        Color c = color ?? Color.black;
        fadeImage.color = new Color(c.r, c.g, c.b, fadeImage.color.a);

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeRoutine(duration, targetAlpha, onComplete));
    }

    // フェード完了時に onComplete を呼ぶ
    private IEnumerator FadeRoutine(float duration, float targetAlpha, System.Action onComplete)
    {
        float startAlpha = fadeImage.color.a;
        float timer = 0f;

        while (timer < duration)
        {
            float a = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, a);

            timer += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
        onComplete?.Invoke();
    }

    private bool IsBossStage(StageSet.StageKind kind)
    {
        return kind == StageSet.StageKind.Forest_Boss
            || kind == StageSet.StageKind.Cave_Boss;
    }
    private string GetBossMovieKey(StageSet.StageKind kind)
    {
        switch (kind)
        {
            case StageSet.StageKind.Forest_Boss:
                return "ForestBossMovie";

            case StageSet.StageKind.Cave_Boss:
                return "CaveBossMovie";

            default:
                return string.Empty;
        }
    }
    private void PlayBossMovieThenSpawn(int index)
    {
        StageSet stage = _stageSets[index];
        string movieKey = GetBossMovieKey(stage.stageKind);

        _movieManager.PlayMovie(movieKey);

        // ムービー終了後に呼ばれる
        _movieManager.OnMovieFinished = () =>
        {
            SpawnStageInternal(index);
            FadeIn(1.0f);
        };
    }


    /// <summary>
    /// WaveSpawner からスキル会得通知
    /// </summary>
    /// <param name="acquiredSkill"></param>
    public void AcquireSkill(SkillData.SkillElement acquiredSkill)
    {
        if (!_acquiredSkills.Contains(acquiredSkill))
        {
            _acquiredSkills.Add(acquiredSkill);
            _afterSkill = acquiredSkill.ToString();
        }
    }

    public string GetBeforeSkill()
    {
        return _beforeSkill;
    }
    public int GetCurrentStageIndex()
    {
        return _waveStageIndex;
    }
}
