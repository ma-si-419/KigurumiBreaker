using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class DetectionMark : MonoBehaviour
{
    // 敵の参照
    private Enemy _enemy;
    // ビックリマークのイメージコンポーネント
    [SerializeField] private SpriteRenderer _image;

    // ビックリマークの表示オフセット
    private Vector3 _detectionMarkOffset;
    // 経過時間
    private float _time = 0.0f; 

    private float _lifeTime = 2.0f;
    private float _fadeDuration = 1.0f;

    //private bool _isFading = false;

    public void SetTarget(Enemy enemy)
    {
        // 敵の参照
        _enemy = enemy;
        // ビックリマークの表示オフセットを設定
        _detectionMarkOffset.y = _enemy.enemyData.barYPosition;
    }

    private void Start()
    {
        // イメージコンポーネントの取得
        _image = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _time += Time.deltaTime;

        if(_enemy == null)
        {
            Destroy(gameObject);
            return;
        }

        // 一定時間経過後にフェード開始
        if (_time > _lifeTime)
        {
            Destroy(gameObject);
        }

        // ビックリマークの表示位置を計算
        transform.position = _enemy.transform.position + _detectionMarkOffset; // 敵の頭上に表示
        transform.forward = Camera.main.transform.forward;

    }

    //private IEnumerator FadeOut()
    //{
    //    float fadeTime = 0.0f;
    //    Color color = _image.color;

    //    while (fadeTime < _fadeDuration)
    //    {
    //        fadeTime += Time.deltaTime;
    //        float alpha = Mathf.Lerp(1.0f, 0.0f, fadeTime / _fadeDuration);
    //        color.a = alpha;
    //        _image.color = color;
    //        yield return null;
    //    }

    //    Destroy(gameObject);
    //}

}
