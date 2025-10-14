using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//===================================
// 敵の種類をEnumで管理
//===================================
public enum EnemyKind
{
    Circle,
    Punch,
    Suicide,
    Tackle,
    Long
}

//===================================
// Wave内で出現する敵情報
//===================================
[System.Serializable]
public class PopEnemy
{
    public EnemyKind enemyKind;      // Enumで選択
    public Vector3 spawnPosition;    // 固定座標
    public bool randomizePosition = true;
    public int spawnCount = 1;       // 出現数
}

//===================================
// Wave
//===================================
[System.Serializable]
public class EnemyWave
{
    public List<PopEnemy> popEnemies = new List<PopEnemy>();
}

//===================================
// グループ
//===================================
[System.Serializable]
public class EnemyGroup
{
    public string groupName = "Group";
    public List<EnemyWave> waves = new List<EnemyWave>();
}

//===================================
// メインスクリプト
//===================================
public class WaveSpawner : MonoBehaviour
{
    [Header("Prefab管理 (EnumとPrefab紐付け)")]
    public List<EnemyPrefabEntry> enemyPrefabs = new List<EnemyPrefabEntry>();

    [Header("グループ設定")]
    public List<EnemyGroup> groups = new List<EnemyGroup>();

    [Header("出現範囲（全体共通）")]
    public Transform areaCenter;
    public Vector3 areaSize = new Vector3(10, 0, 10);

    [Header("出現間隔")]
    public float spawnInterval = 0.5f;

    private Dictionary<EnemyKind, GameObject> prefabDict;

    private void Awake()
    {
        // EnumとPrefabをDictionaryで紐付け
        prefabDict = new Dictionary<EnemyKind, GameObject>();
        foreach (var entry in enemyPrefabs)
        {
            if (!prefabDict.ContainsKey(entry.enemyKind))
                prefabDict.Add(entry.enemyKind, entry.prefab);
        }
    }

    private void Start()
    {
        // 各グループを独立してWave進行
        foreach (var group in groups)
        {
            StartCoroutine(HandleGroupWaves(group));
        }
    }

    //===================================
    // 1グループのWave管理
    //===================================
    private IEnumerator HandleGroupWaves(EnemyGroup group)
    {
        for (int w = 0; w < group.waves.Count; w++)
        {
            var wave = group.waves[w];
            List<GameObject> spawned = new List<GameObject>();

            foreach (var pop in wave.popEnemies)
            {
                GameObject prefab = GetPrefabByKind(pop.enemyKind);
                if (prefab == null)
                {
                    Debug.LogWarning($"'{pop.enemyKind}' がPrefabリストに存在しません。");
                    continue;
                }

                for (int i = 0; i < pop.spawnCount; i++)
                {
                    Vector3 pos = pop.randomizePosition ? GetRandomPositionInArea() : pop.spawnPosition;
                    GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);
                    spawned.Add(enemy);
                    yield return new WaitForSeconds(spawnInterval);
                }
            }

            // このWaveの敵全滅を待つ
            yield return new WaitUntil(() =>
            {
                spawned.RemoveAll(e => e == null);
                return spawned.Count == 0;
            });

            Debug.Log($"{group.groupName} Wave {w + 1} 全滅 → 次Waveへ");
        }

        Debug.Log($"{group.groupName} の全Wave完了");
    }

    private GameObject GetPrefabByKind(EnemyKind kind)
    {
        if (prefabDict.TryGetValue(kind, out GameObject prefab))
            return prefab;
        return null;
    }

    private Vector3 GetRandomPositionInArea()
    {
        if (areaCenter == null) return Vector3.zero;

        Vector3 offset = new Vector3(
            Random.Range(-areaSize.x / 2, areaSize.x / 2),
            Random.Range(-areaSize.y / 2, areaSize.y / 2),
            Random.Range(-areaSize.z / 2, areaSize.z / 2)
        );
        return areaCenter.position + offset;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (areaCenter == null) return;
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawCube(areaCenter.position, areaSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(areaCenter.position, areaSize);
    }
#endif
}

//===================================
// PrefabとEnumを紐付けるEntry
//===================================
[System.Serializable]
public class EnemyPrefabEntry
{
    public EnemyKind enemyKind;
    public GameObject prefab;
}
