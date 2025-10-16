using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PopEnemy
{
    public EnemyKind spawnKind;                                       // 出したい敵の種類
    public Vector3 spawnPosition;                                     // SpawnPositionを決める。
    public bool randomizePosition = true;                             // もしTrueならSpawnPositionの座標は無視され、ランダムに決定される。
}

[System.Serializable]
public class EnemyWave
{
    public List<PopEnemy> popEnemies = new List<PopEnemy>();          // このWaveで出現させる敵たち
}

[System.Serializable]
public class EnemyGroup
{
    public string groupName = "Group";                               // グループ名を決めさせる。(一応作っていた方がグループ分けする際などに便利)
    public List<EnemyWave> waves = new List<EnemyWave>();            // このグループのWaveたち
}

public class WaveSpawner : MonoBehaviour
{
    [Header("全体の出現範囲設定")]
    public Transform areaCenter;                                    // 出現範囲の中心
    public Vector3 areaSize = new Vector3(10, 0, 10);

    [Header("敵データ（EnemySetData）")]
    public SpawnData enemySetData;                                  // 敵の種類データベース

    [Header("グループ設定")]
    public List<EnemyGroup> groups = new List<EnemyGroup>();        //　出現させるグループ設定

    [Header("出現間隔")]
    public float spawnInterval = 0.5f;                              //　敵を出現させる間隔

    [Header("全Wave終了後に停止するエフェクト")]
    public ParticleSystem[] targetEffects;                          //　Wave終了後に停止するエフェクト

    private bool allCleared = false;                                // すべての敵が倒されたかどうかの判定

    private void Start()
    {
        foreach (var group in groups)                               // 各グループのWaveを順番に処理させる
        {
            StartCoroutine(HandleGroupWaves(group));               
        }
    }


    /// <summary>
    /// グループ内のWaveを順番に処理するコルーチン
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    private IEnumerator HandleGroupWaves(EnemyGroup group)
    {
        for (int w = 0; w < group.waves.Count; w++)                 // 各Waveを順番に処理していく
        {
            var wave = group.waves[w];                              //　各々の現在のWaveを取得する
            List<GameObject> spawned = new List<GameObject>();      // このWaveで出現させた敵を管理するリスト

            foreach (var pop in wave.popEnemies)                    // 各Wave内の敵を順番に出現させていく処理
            {
                GameObject prefabToSpawn = enemySetData?.GetPrefabByKind(pop.spawnKind);　   // 敵の種類に応じたPrefabを取得
                if (prefabToSpawn == null)                                                   //　Prefabが見つからなかった場合は警告を出してスキップ
                {
                    Debug.LogWarning($"[{group.groupName}] {pop.spawnKind} のPrefabが見つかりません");
                    continue;
                }

                Vector3 spawnPos = pop.randomizePosition ? GetRandomNavMeshPosition() : pop.spawnPosition;  // 出現位置を決定

                GameObject enemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);               //　敵を出現させる
                spawned.Add(enemy);                                                                         //　出現された敵をリストに追加
                yield return new WaitForSeconds(spawnInterval);                                             //　次の敵を出現させるまでの敵待ち
            }

            yield return new WaitUntil(() =>                                                                // Waveの敵全滅待ち
            {  
                spawned.RemoveAll(e => e == null);                                                          // 敵が倒されてnullになったものをリストから削除
                return spawned.Count == 0;                                                                  // すべて倒されたかどうかを返す
            });

            Debug.Log($"{group.groupName} Wave {w + 1} 終了");
        }

        Debug.Log($"{group.groupName} の全Wave完了");
        CheckAllGroupsCleared();                                    // グループがすべて終了したかどうかを確認する
    }

    private void CheckAllGroupsCleared()                            // すべてのグループが終了したかどうかを確認する。
    {
        if (allCleared) return;                                     // すでにすべて終了している場合は無視する
        allCleared = true;                                          // Trueにする
        StopAllEffects();                                           // すべてのエフェクトを停止する
    }

    private void StopAllEffects()                                   // すべてのエフェクトを停止する
    {
        foreach (var effect in targetEffects)                       //書くエフェクトを順番に処理する
        {
            if (effect != null)                                     // nullチェック
            {
                var main = effect.main;                             //　ParticleSystemのMainModuleを取得
                main.loop = false;                                  //　ループをオフにする
                effect.Stop();                                      //　エフェクトを停止する
            }
        }
    }

    private Vector3 GetRandomNavMeshPosition()                      // ナビメッシュ上のランダムな位置を取得する
    {
        for (int i = 0; i < 30; i++)                                // 最大30回試行(これに関しては適当な数字です)
        {
            Vector3 randomPos = areaCenter.position + new Vector3(  // ランダムな位置を計算
                Random.Range(-areaSize.x / 2, areaSize.x / 2),      
                0f,
                Random.Range(-areaSize.z / 2, areaSize.z / 2)
            );

            Collider[] cols = Physics.OverlapSphere(randomPos, 1f); // 半径1mの範囲にNoSpawnタグがついたオブジェクトがあるかどうかを確認
            bool invalid = false;                                   // 無効フラグ
            foreach (var col in cols)                               // 各コライダーをチェック
            {
                if (col.CompareTag("NoSpawn"))                      // NoSpawnタグがついていたら無効にする
                {
                    invalid = true;
                    break;
                }
            }

            if (invalid) continue;                                  // 無効なら次の試行へ

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))    // ナビメッシュ上の位置を取得
                return hit.position;                                                            // 見つかったらその位置を返す
        }
        return areaCenter.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()                                     // 出現範囲をエディタ上に表示(範囲が確認しやすいように)
    {
        if (areaCenter == null) return;
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawCube(areaCenter.position, areaSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(areaCenter.position, areaSize);
    }
#endif
}
