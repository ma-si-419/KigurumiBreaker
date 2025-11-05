using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class DetectionMark : MonoBehaviour
{
    // 敵の参照
    private Enemy _enemy;

    private Vector3 _detectionMarkOffset; // ビックリマークの表示オフセット
    private float _time = 0.0f; // 経過時間

    public void SetTarget(Enemy enemy)
    {
        // 敵の参照
        _enemy = enemy;
        // ビックリマークの表示オフセットを設定
        _detectionMarkOffset.y = _enemy.enemyData.barYPosition;
    }

    private void Update()
    {
        _time += Time.deltaTime;

        if(_enemy == null)
        {
            Destroy(gameObject);
            return;
        }

        if(_time > 3.0f)
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log("DetectionMark Update");

        // ビックリマークの表示位置を計算
        transform.position = _enemy.transform.position + _detectionMarkOffset; // 敵の頭上に表示
        transform.forward = Camera.main.transform.forward;

    }

}
