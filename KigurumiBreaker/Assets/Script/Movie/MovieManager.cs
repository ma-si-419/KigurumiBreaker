using UnityEngine;
using UnityEngine.Video;
using System;
using System.Collections;
using System.Collections.Generic;

public class MovieManager : MonoBehaviour
{
    public static MovieManager instance;

    [Serializable]
    public class MovieData
    {
        public string key;
        public VideoClip clip;
    }

    [Header("Root")]
    [SerializeField] private GameObject movieRoot;
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Movie List")]
    [SerializeField] private MovieData[] movieList;

    [Header("Skip")]
    [SerializeField] private KeyCode skipKey = KeyCode.Space;
    [SerializeField] private float skipHoldTime = 2f;

    private Dictionary<string, VideoClip> movieDict;
    private float skipTimer;
    private bool isPlaying;
    private float beforeTimeScale;

    public bool IsPlaying => isPlaying;
    public Action OnMovieFinished;
    private List<Canvas> disabledCanvases = new List<Canvas>();


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        movieRoot.SetActive(false);

        movieDict = new Dictionary<string, VideoClip>();
        foreach (var m in movieList)
            movieDict[m.key] = m.clip;

        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.isLooping = false;
        videoPlayer.loopPointReached += OnMovieEnd;
    }

    private void Update()
    {
        if (!isPlaying) return;

        if (Input.GetKey(skipKey))
        {
            skipTimer += Time.unscaledDeltaTime;
            if (skipTimer >= skipHoldTime)
                Skip();
        }
        else skipTimer = 0f;
    }

    public void PlayMovie(string key)
    {
        if (!movieDict.TryGetValue(key, out var clip))
        {
            Debug.LogError($"Movie key not found: {key}");
            return;
        }

        StartCoroutine(PlayRoutine(clip));
    }

    private IEnumerator PlayRoutine(VideoClip clip)
    {
        isPlaying = true;
        skipTimer = 0f;

        beforeTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        DisableOtherCanvases();   
        movieRoot.SetActive(true);

        videoPlayer.clip = clip;
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared) yield return null;

        videoPlayer.Play();
    }


    private void Skip()
    {
        videoPlayer.Stop();
        OnMovieEnd(videoPlayer);
    }

    private void OnMovieEnd(VideoPlayer vp)
    {
        videoPlayer.Stop();

        movieRoot.SetActive(false);   // ÅöêeÇ≤Ç∆è¡Ç∑
        RestoreCanvases();            // ÅöUIïúäà

        Time.timeScale = beforeTimeScale;

        isPlaying = false;
        OnMovieFinished?.Invoke();
        OnMovieFinished = null;
    }

    private void DisableOtherCanvases()
    {
        disabledCanvases.Clear();

        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (var c in canvases)
        {
            if (c.transform.IsChildOf(movieRoot.transform)) continue;

            if (c.enabled)
            {
                c.enabled = false;
                disabledCanvases.Add(c);
            }
        }
    }
    private void RestoreCanvases()
    {
        foreach (var c in disabledCanvases)
        {
            if (c != null)
                c.enabled = true;
        }
        disabledCanvases.Clear();
    }


}
