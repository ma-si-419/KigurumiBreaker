using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SpawnEnemyData", menuName = "GameData/SpawnEnemyData")]
public class SpawnEnemyData : ScriptableObject
{
    [Header("敵の出現データ（ウェーブと敵情報まとめ）")]
    public List<WaveEnemyData> waveEnemyDataList = new List<WaveEnemyData>();
}

[System.Serializable]
public class WaveEnemyData
{
    public List<PopEnemyData> popEnemies = new List<PopEnemyData>();
}

[System.Serializable]
public class PopEnemyData
{
    public EnemyKind spawnKind;
    public Vector3 spawnPosition;
    public bool randomizePosition = true;
}
