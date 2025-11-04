using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBarManager : MonoBehaviour
{
    // 敵のHPバーのプレハブ
    [SerializeField] private GameObject _enemyBarPrefab;
    // 敵のビックリマークのプレハブ
    [SerializeField] private GameObject _detectionMarkPrefab;

    // ボスのHPバーのプレハブ
    [SerializeField] private GameObject _bossEnemyBarPrefab;

    // 敵のHPバーのリスト
    private readonly List<EnemyBar> _enemyBars = new();
    // 敵のビックリマークのリスト
    private readonly List<DetectionMark> _detectionMarks = new();

    // ボスのHPバーのリスト
    private readonly List<BossEnemyBar> _bossEnemyBars = new();

    public void CreateEnemyBar(Enemy enemy)
    {
        //Canvasの子オブジェクトとして敵のHPバーを生成
        var barObject = Instantiate(_enemyBarPrefab, transform);
        var hpBar = barObject.GetComponent<EnemyBar>();
        hpBar.SetTarget(enemy);
        _enemyBars.Add(hpBar);
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

    public void CreateBossEnemyBar(BossEnemy bossenemy)
    {
        //Canvasの子オブジェクトとしてボスのHPバーを生成
        var barObject = Instantiate(_bossEnemyBarPrefab, transform);
        var bossHpBar = barObject.GetComponent<BossEnemyBar>();
        bossHpBar.SetTarget(bossenemy);
        _bossEnemyBars.Add(bossHpBar);
    }
}
