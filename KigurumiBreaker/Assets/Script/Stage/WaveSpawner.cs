using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
}

[System.Serializable]
public class Wave
{
    public List<EnemySpawnData> enemies;
}

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private List<Wave> waves;
    [SerializeField] private ParticleSystem[] targetEffects;  //エフェクトをここで管理

    private int currentWaveIndex = 0;
    private List<GameObject> aliveEnemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(StartWave());
    }

    IEnumerator StartWave()
    {
        Wave wave = waves[currentWaveIndex];

        foreach (var spawnData in wave.enemies)
        {
            GameObject enemy = Instantiate(spawnData.enemyPrefab, spawnData.spawnPoint.position, Quaternion.identity);
            aliveEnemies.Add(enemy);

            EnemyDeathNotifier notifier = enemy.AddComponent<EnemyDeathNotifier>();
            notifier.onDeath += () => aliveEnemies.Remove(enemy);
        }

        // このWaveの敵が全滅するまで待機
        yield return new WaitUntil(() => aliveEnemies.Count == 0);

        // 次のWaveへ
        currentWaveIndex++;
        if (currentWaveIndex < waves.Count)
        {
            yield return new WaitForSeconds(2f);
            StartCoroutine(StartWave());
        }
        else
        {
            Debug.Log("全Waveクリア！");
            StopAllEffects(); //全Waveクリア時にエフェクト停止
        }
    }

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
    }
}

public class EnemyDeathNotifier : MonoBehaviour
{
    public System.Action onDeath;

    void OnDestroy()
    {
        onDeath?.Invoke();
    }
}
