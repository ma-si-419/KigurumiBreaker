using UnityEngine;
using System.Collections.Generic;

public class EnemyWaveSpawner : MonoBehaviour
{
    [Header("出現させる敵プレハブ（種類別に登録）")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("出現ポイント")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("ウェーブ設定")]
    [SerializeField] private int totalWaves = 3;         // 全ウェーブ数
    [SerializeField] private int enemiesPerWave = 6;     // 1ウェーブあたりの敵数
    [SerializeField] private float waveDelay = 2f;       // ウェーブ間の待機時間

    [Header("対象のエフェクト")]
    [SerializeField] private ParticleSystem[] targetEffects;

    private int currentWave = 0;
    private bool spawning = false;
    private bool allWavesFinished = false;

    // 各スポーンポイントの敵キュー（順番に出すため）
    private Dictionary<Transform, Queue<GameObject>> spawnQueues = new Dictionary<Transform, Queue<GameObject>>();
    // 現在生きている敵（追跡用）
    private Dictionary<Transform, GameObject> activeEnemyAtPoint = new Dictionary<Transform, GameObject>();

    void Start()
    {
        // スポーンポイントごとにキューと現在敵リストを初期化
        foreach (var point in spawnPoints)
        {
            spawnQueues[point] = new Queue<GameObject>();
            activeEnemyAtPoint[point] = null;
        }

        // 最初のWave開始
        StartCoroutine(SpawnWave());
    }

    void Update()
    {
        if (allWavesFinished) return;

        // 各ポイントの敵が死んでいたら次を生成
        foreach (var point in spawnPoints)
        {
            if (activeEnemyAtPoint[point] == null && spawnQueues[point].Count > 0)
            {
                SpawnEnemyAt(point);
            }
        }

        // 全ポイントのキューと敵が空なら次ウェーブへ
        bool allEmpty = true;
        foreach (var point in spawnPoints)
        {
            if (activeEnemyAtPoint[point] != null || spawnQueues[point].Count > 0)
            {
                allEmpty = false;
                break;
            }
        }

        // 全部消滅 → 次Wave
        if (!spawning && allEmpty)
        {
            if (currentWave < totalWaves)
            {
                StartCoroutine(SpawnWave());
            }
            else
            {
                allWavesFinished = true;
                Debug.Log("--- 全ウェーブ完了 ---");
                StopAllEffects();
            }
        }
    }

    private System.Collections.IEnumerator SpawnWave()
    {
        spawning = true;
        currentWave++;
        Debug.Log($"--- Wave {currentWave} 開始 ---");

        // Waveごとにキュー登録（同じ場所で順番出現）
        for (int i = 0; i < enemiesPerWave; i++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            spawnQueues[point].Enqueue(prefab);
        }

        // 各ポイントの最初の敵だけ出す
        foreach (var point in spawnPoints)
        {
            if (spawnQueues[point].Count > 0 && activeEnemyAtPoint[point] == null)
            {
                SpawnEnemyAt(point);
            }
        }

        yield return new WaitForSeconds(waveDelay);
        spawning = false;
    }

    private void SpawnEnemyAt(Transform point)
    {
        if (spawnQueues[point].Count == 0) return;

        GameObject prefab = spawnQueues[point].Dequeue();
        GameObject enemy = Instantiate(prefab, point.position, Quaternion.identity);

        // 敵破壊時にnullに戻すコルーチン
        StartCoroutine(TrackEnemyDeath(point, enemy));
        activeEnemyAtPoint[point] = enemy;
    }

    private System.Collections.IEnumerator TrackEnemyDeath(Transform point, GameObject enemy)
    {
        // Destroyされるまで待つ
        while (enemy != null)
            yield return null;

        // 死亡を確認したら空きにする
        activeEnemyAtPoint[point] = null;
    }

    /// <summary>
    /// 登録されたエフェクトを全停止
    /// </summary>
    private void StopAllEffects()
    {
        foreach (var effect in targetEffects)
        {
            if (effect != null && effect.isPlaying)
            {
                var main = effect.main;
                main.loop = false;
                effect.Stop();
            }
        }
        Debug.Log("全エフェクト停止完了");
    }
}
