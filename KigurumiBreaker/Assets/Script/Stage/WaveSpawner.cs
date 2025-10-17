using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PopEnemy
{
    public EnemyKind spawnKind;          // 出したい敵の種類
    public Vector3 spawnPosition;        // SpawnPositionを決める
    public bool randomizePosition = true; // TrueならSpawnPositionを無視してランダム生成
}

[System.Serializable]
public class EnemyWave
{
    public List<PopEnemy> popEnemies = new List<PopEnemy>(); // このWaveで出現させる敵たち
}

[System.Serializable]
public class EnemyGroup
{
    public string groupName = "Group";                       // グループ名（デバッグ用）
    public List<EnemyWave> waves = new List<EnemyWave>();    // このグループのWaveたち
}

public class WaveSpawner : MonoBehaviour
{
    [Header("全体の出現範囲設定")]
    public Transform areaCenter;
    public Vector3 areaSize = new Vector3(10, 0, 10);

    [Header("敵データ（SpawnData）")]
    public SpawnData enemySetData;

    [Header("グループ設定")]
    public List<EnemyGroup> groups = new List<EnemyGroup>();

    [Header("出現間隔")]
    public float spawnInterval = 0.5f;

    [Header("全Wave終了後に停止するエフェクト")]
    public ParticleSystem[] targetEffects;

    [Header("スキル関連")]
    public SkillSelectManager skillSelectManager;
    public SkillData.SkillElement nextSkillElement;

    [HideInInspector] public bool isAllWavesCleared = false;  // SkillSelectManagerが監視
    [HideInInspector] public bool skillSelectFinished = false; // Skill選択完了通知

    private int groupsClearedCount = 0;                       // 全グループ終了判定
    private Vector3 lastDeadEnemyPos;                         // 最後に倒れた敵の位置

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

            // 敵生成
            foreach (var pop in wave.popEnemies)
            {
                GameObject prefabToSpawn = enemySetData?.GetPrefabByKind(pop.spawnKind);
                if (prefabToSpawn == null) continue;

                Vector3 spawnPos = pop.randomizePosition ? GetRandomNavMeshPosition() : pop.spawnPosition;
                GameObject enemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                spawned.Add(enemy);
                yield return new WaitForSeconds(spawnInterval);
            }

            // Wave内敵の全滅待機（最後に生きていた敵の座標を記録）
            yield return StartCoroutine(WaitForWaveClear(spawned));

            Debug.Log($"{group.groupName} Wave {w + 1} 終了");
        }

        Debug.Log($"{group.groupName} の全Wave完了");
        OnGroupCleared();
    }

    /// <summary>
    /// Wave内最後に生きていた敵の座標を保持しつつ全滅待機
    /// </summary>
    private IEnumerator WaitForWaveClear(List<GameObject> spawned)
    {
        Vector3 lastPos = Vector3.zero;

        while (spawned.Count > 0)
        {
            for (int i = spawned.Count - 1; i >= 0; i--)
            {
                var go = spawned[i];
                if (go == null)
                {
                    spawned.RemoveAt(i);
                }
                else
                {
                    // 毎フレーム、最後に存在する敵の座標を更新
                    lastPos = go.transform.position;
                }
            }
            yield return null;
        }

        lastDeadEnemyPos = lastPos; // Wave終了時の最後の敵座標
    }

    private void OnGroupCleared()
    {
        groupsClearedCount++;

        // まだ他のグループが残っていたら終了
        if (groupsClearedCount < groups.Count) return;

        // 全グループ終了
        isAllWavesCleared = true;
        Debug.Log("全グループの敵を撃破。isAllWavesCleared = true");

        // 最後に倒れた敵の位置にスキル取得アイテム生成
        if (skillSelectManager != null)
        {
            Vector3 spawnPos = lastDeadEnemyPos != Vector3.zero ? lastDeadEnemyPos : (areaCenter != null ? areaCenter.position : transform.position);
            skillSelectManager.PopSkillGetObject(spawnPos, nextSkillElement, this);
            Debug.Log($"PopSkillGetObject() を呼び出しました。位置: {spawnPos}");
        }

        // Skill選択完了待機
        StartCoroutine(WaitForSkillSelectFinish());
    }

    private IEnumerator WaitForSkillSelectFinish()
    {
        yield return new WaitUntil(() => skillSelectFinished);

        Debug.Log("SkillSelect完了を検知。WaveSpawner側でエフェクト停止処理を実行。");
        StopAllEffects();

        // 次回用にフラグをリセット
        isAllWavesCleared = false;
        skillSelectFinished = false;
        groupsClearedCount = 0;
        lastDeadEnemyPos = Vector3.zero;
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
                if (col.CompareTag("Wall"))
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

    public void OnSkillSelectFinished()
    {
        skillSelectFinished = true;
        Debug.Log("WaveSpawner: スキル選択完了を受け取りました。");
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
