using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class DetectionMark : MonoBehaviour
{
    // 敵の参照
    private Enemy _enemy;
    // ビックリマークのイメージコンポーネント
    private SpriteRenderer _image;

    // ビックリマークの表示オフセット
    private Vector3 _detectionMarkOffset;
    // 経過時間
    [SerializeField] private float _time = 0.0f;
    [SerializeField] private float _lifeTime = 0.5f;

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

}
