using UnityEngine;
using UnityEngine.AI;
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
// PrefabとEnumを紐付けるEntry
//===================================
[System.Serializable]
public class EnemyPrefabEntry
{
    public EnemyKind enemyKind;
    public GameObject prefab;
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

    [Header("全グループ完了後に停止するエフェクト")]
    public ParticleSystem[] targetEffects;

    private Dictionary<EnemyKind, GameObject> prefabDict;
    private int groupsFinished = 0;
    private bool effectsStopped = false;

    private void Awake()
    {
        prefabDict = new Dictionary<EnemyKind, GameObject>();
        foreach (var entry in enemyPrefabs)
        {
            if (!prefabDict.ContainsKey(entry.enemyKind))
                prefabDict.Add(entry.enemyKind, entry.prefab);
        }
    }

    private void Start()
    {
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

                Vector3 pos = pop.randomizePosition ? GetRandomNavMeshPositionInArea() : pop.spawnPosition;
                GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);
                spawned.Add(enemy);
                yield return new WaitForSeconds(spawnInterval);
            }

            // Waveの敵全滅を待つ
            yield return new WaitUntil(() =>
            {
                spawned.RemoveAll(e => e == null);
                return spawned.Count == 0;
            });

            Debug.Log($"{group.groupName} Wave {w + 1} 全滅 → 次Waveへ");
        }

        Debug.Log($"{group.groupName} の全Wave完了");

        groupsFinished++;
        if (!effectsStopped && groupsFinished >= groups.Count)
        {
            effectsStopped = true;
            StopAllEffects();
        }
    }

    private GameObject GetPrefabByKind(EnemyKind kind)
    {
        if (prefabDict.TryGetValue(kind, out GameObject prefab))
            return prefab;
        return null;
    }

    //===================================
    // 範囲内でNavMeshに沿ったランダム位置を取得
    //===================================
    private Vector3 GetRandomNavMeshPositionInArea()
    {
        if (areaCenter == null) return Vector3.zero;

        Vector3 pos;
        int tries = 0;
        const int maxTries = 20;

        do
        {
            Vector3 offset = new Vector3(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                0f,
                Random.Range(-areaSize.z / 2, areaSize.z / 2)
            );
            pos = areaCenter.position + offset;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(pos, out hit, 2f, NavMesh.AllAreas))
                return hit.position;

            tries++;
        } while (tries < maxTries);

        return areaCenter.position;
    }

    //===================================
    // 全エフェクト停止
    //===================================
    private void StopAllEffects()
    {
        foreach (var effect in targetEffects)
        {
            if (effect != null)
            {
                var main = effect.main;
                main.loop = false;
                effect.Stop();
            }
        }
        Debug.Log("全エフェクト停止完了");
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
