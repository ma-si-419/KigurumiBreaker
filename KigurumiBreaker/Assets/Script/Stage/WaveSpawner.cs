using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemyProbability
{
    public GameObject enemyPrefab;
    [Range(0f, 1f)]
    public float spawnChance; // 出現確率
    public string type;       // "Melee" / "Ranged" / "Suicide" / "Tackle"
}

public class WaveSpawner : MonoBehaviour
{
    [Header("敵の出現確率設定")]
    [SerializeField] private EnemyProbability[] enemyProbabilities;

    [Header("Spawnポイント")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("ウェーブ設定")]
    [SerializeField] private int totalWaves = 5;
    [SerializeField] private int enemiesPerWave = 6;
    [SerializeField] private float waveDelay = 2f;

    [Header("対象のエフェクト")]
    [SerializeField] private ParticleSystem[] targetEffects;

    [Header("Player参照")]
    [SerializeField] private Transform player; // PlayerのTransform

    [Header("近距離判定距離")]
    [SerializeField] private float meleeDistance = 5f;

    private int currentWave = 0;
    private bool spawning = false;
    private bool allWavesFinished = false;

    private Dictionary<Transform, Queue<GameObject>> spawnQueues = new Dictionary<Transform, Queue<GameObject>>();
    private Dictionary<Transform, GameObject> activeEnemyAtPoint = new Dictionary<Transform, GameObject>();

    void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning("Scene に Player タグのオブジェクトが存在しません");
        }
    }


    void Start()
    {
        foreach (var point in spawnPoints)
        {
            spawnQueues[point] = new Queue<GameObject>();
            activeEnemyAtPoint[point] = null;
        }
        StartCoroutine(SpawnWave());
       
    }

    void Update()
    {
        if (allWavesFinished) return;

        foreach (var point in spawnPoints)
        {
            if (activeEnemyAtPoint[point] == null && spawnQueues[point].Count > 0)
            {
                SpawnEnemyAt(point);
            }
        }

        bool allEmpty = true;
        foreach (var point in spawnPoints)
        {
            if (activeEnemyAtPoint[point] != null || spawnQueues[point].Count > 0)
            {
                allEmpty = false;
                break;
            }
        }

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

        for (int i = 0; i < enemiesPerWave; i++)
        {
            EnemyProbability selected = SelectEnemyByProbability();
            Transform spawnPoint = ChooseSpawnPointByDistance(selected.type);
            if (spawnPoint != null)
                spawnQueues[spawnPoint].Enqueue(selected.enemyPrefab);
        }

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

    private EnemyProbability SelectEnemyByProbability()
    {
        float total = 0f;
        foreach (var e in enemyProbabilities)
        {
            total += e.spawnChance;
        }

        float roll = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var e in enemyProbabilities)
        {
            cumulative += e.spawnChance;
            if (roll <= cumulative)
                return e;
        }

        return enemyProbabilities[enemyProbabilities.Length - 1]; // 万が一
    }

    private Transform ChooseSpawnPointByDistance(string type)
    {
        List<Transform> candidates = new List<Transform>();

        foreach (var point in spawnPoints)
        {
            float dist = Vector3.Distance(point.position, player.position);
            if (type == "Melee" || type == "Tackle" || type == "Suicide")
            {
                if (dist <= meleeDistance) candidates.Add(point); // Playerに近いポイント
            }
            else if (type == "Ranged")
            {
                if (dist > meleeDistance) candidates.Add(point);  // Playerから離れたポイント
            }
        }

        if (candidates.Count == 0)
            candidates.AddRange(spawnPoints); // 該当なしなら全体から

        return candidates[Random.Range(0, candidates.Count)];
    }

    private void SpawnEnemyAt(Transform point)
    {
        if (spawnQueues[point].Count == 0) return;

        GameObject prefab = spawnQueues[point].Dequeue();
        GameObject enemy = Instantiate(prefab, point.position, Quaternion.identity);

        StartCoroutine(TrackEnemyDeath(point, enemy));
        activeEnemyAtPoint[point] = enemy;
    }

    private System.Collections.IEnumerator TrackEnemyDeath(Transform point, GameObject enemy)
    {
        while (enemy != null)
            yield return null;

        activeEnemyAtPoint[point] = null;
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
        Debug.Log("全エフェクト停止完了");
    }
}
