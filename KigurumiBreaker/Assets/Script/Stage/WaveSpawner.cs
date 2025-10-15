using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PopEnemy
{
    public EnemyKind spawnKind;            // 出したい敵の種類
    public Vector3 spawnPosition;
    public bool randomizePosition = true;
}

[System.Serializable]
public class EnemyWave
{
    public List<PopEnemy> popEnemies = new List<PopEnemy>();
}

[System.Serializable]
public class EnemyGroup
{
    public string groupName = "Group";
    public List<EnemyWave> waves = new List<EnemyWave>();
}

public class WaveSpawner : MonoBehaviour
{
    [Header("全体の出現範囲設定")]
    public Transform areaCenter;
    public Vector3 areaSize = new Vector3(10, 0, 10);

    [Header("敵データ（EnemySetData）")]
    public SpawnData enemySetData;

    [Header("グループ設定")]
    public List<EnemyGroup> groups = new List<EnemyGroup>();

    [Header("出現間隔")]
    public float spawnInterval = 0.5f;

    [Header("全Wave終了後に停止するエフェクト")]
    public ParticleSystem[] targetEffects;

    private bool allCleared = false;

    private void Start()
    {
        foreach (var group in groups)
        {
            StartCoroutine(HandleGroupWaves(group));
        }
    }

    private IEnumerator HandleGroupWaves(EnemyGroup group)
    {
        for (int w = 0; w < group.waves.Count; w++)
        {
            var wave = group.waves[w];
            List<GameObject> spawned = new List<GameObject>();

            foreach (var pop in wave.popEnemies)
            {
                GameObject prefabToSpawn = enemySetData?.GetPrefabByKind(pop.spawnKind);
                if (prefabToSpawn == null)
                {
                    Debug.LogWarning($"[{group.groupName}] {pop.spawnKind} のPrefabが見つかりません");
                    continue;
                }

                Vector3 spawnPos = pop.randomizePosition ? GetRandomNavMeshPosition() : pop.spawnPosition;

                GameObject enemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                spawned.Add(enemy);
                yield return new WaitForSeconds(spawnInterval);
            }

            // Waveの敵全滅待ち
            yield return new WaitUntil(() =>
            {
                spawned.RemoveAll(e => e == null);
                return spawned.Count == 0;
            });

            Debug.Log($"{group.groupName} Wave {w + 1} 終了");
        }

        Debug.Log($"{group.groupName} の全Wave完了");
        CheckAllGroupsCleared();
    }

    private void CheckAllGroupsCleared()
    {
        if (allCleared) return;
        allCleared = true;
        StopAllEffects();
    }

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

    private Vector3 GetRandomNavMeshPosition()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = areaCenter.position + new Vector3(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                0f,
                Random.Range(-areaSize.z / 2, areaSize.z / 2)
            );

            Collider[] cols = Physics.OverlapSphere(randomPos, 1f);
            bool invalid = false;
            foreach (var col in cols)
            {
                if (col.CompareTag("NoSpawn"))
                {
                    invalid = true;
                    break;
                }
            }

            if (invalid) continue;

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }
        return areaCenter.position;
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
