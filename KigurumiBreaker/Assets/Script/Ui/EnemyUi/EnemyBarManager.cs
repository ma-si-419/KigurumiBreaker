using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBarManager : MonoBehaviour
{
    // 敵のHPバーのプレハブ
    [SerializeField] private GameObject _enemyBarPrefab;
    // 敵のビックリマークのプレハブ
    [SerializeField] private GameObject _detectionMarkPrefab;

    // 敵のHPバーのリスト
    private readonly List<EnemyBar> _enemyBars = new();
    // 敵のビックリマークのリスト
    private readonly List<DetectionMark> _detectionMarks = new();

    public void CreateEnemyBar(Enemy enemy)
    {
        //Canvasの子オブジェクトとして敵のHPバーを生成
        var barObject = Instantiate(_enemyBarPrefab, transform);
        var hpBar = barObject.GetComponent<EnemyBar>();
        hpBar.SetTarget(enemy);
        _enemyBars.Add(hpBar);
    }

    // 敵のHPバーを削除(一応)
    public void RemoveHpBar(EnemyBar enemybar)
    {
        _enemyBars.Remove(enemybar);
        Destroy(enemybar.gameObject);
    }

    // ビックリマークの生成
    public void CreateEnemyDetectionMark(Enemy enemy)
    {
        //Canvasの子オブジェクトとして敵のビックリマークを生成
        var detectionObject = Instantiate(_detectionMarkPrefab, transform);
        var detectionMark = detectionObject.GetComponent<DetectionMark>();
        detectionMark.SetTarget(enemy);
        _detectionMarks.Add(detectionMark);
    }

    // 敵のビックリマークを削除(一応)
    public void RemoveEnemyDetectionMark(DetectionMark detection)
    {
        _detectionMarks.Remove(detection);
        Destroy(detection.gameObject);
    }
}
