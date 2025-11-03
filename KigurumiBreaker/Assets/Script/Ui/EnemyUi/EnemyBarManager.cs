using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBarManager : MonoBehaviour
{
    // 敵のHPバーのプレハブ
    [SerializeField] private GameObject _enemyBarPrefab;
    // 敵のHPバーのリスト
    private readonly List<EnemyBar> _enemyBars = new();

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

}
