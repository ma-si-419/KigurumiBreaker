using UnityEngine;

public class TitleSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float delaySeconds = 10f;

    private float timer = 0f;
    private bool hasStarted = false;

    void FixedUpdate()
    {
        // すでに再生開始してたら何もしない
        if (hasStarted) return;

        timer += Time.deltaTime;

        if (timer >= delaySeconds)
        {
            audioSource.Play();
            hasStarted = true; // ← これが超重要
        }
    }
}
