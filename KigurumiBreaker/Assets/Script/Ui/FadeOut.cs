using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    [SerializeField] private GameObject _sprite;

    [SerializeField] private float _fadeOutTime = 120f;

    bool _isFadeOut = false;

    private Image _image;

    private void Start()
    {
        _image = _sprite.GetComponent<Image>();
    }

    private void FixedUpdate()
    {
        if (_isFadeOut)
        {
            Color color = _image.color;
            color.a += 1.0f / _fadeOutTime;
            _image.color = color;
            if (color.a >= 1f)
            {
                _isFadeOut = false;

                // ÉVÅ[Éìà⁄ìÆ
                //UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");

                


            }
        }
    }

    public void StartFadeOut()
    {
        if (_isFadeOut) return;
        _isFadeOut = true;
    }

}
