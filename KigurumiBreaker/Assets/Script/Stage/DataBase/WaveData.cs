using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SpawnEnemyData", menuName = "GameData/SpawnEnemyData")]
public class WaveData : ScriptableObject
{
    [Header("敵の配置データ(いくつ配置データを持たせるか)")]
    public List<EnemyPopGroup> waveEnemyDataList = new List<EnemyPopGroup>();
}

[System.Serializable]
public class EnemyPopGroup
{
    [Header("敵の出現データ（ウェーブと敵情報まとめ）")]
    [SerializeField] private List<WaveEnemyData> _spawnDataList; // ← SpawnEnemyData のリストに

    [HideInInspector][SerializeField] int _index = 0;
    public void SetIndex(int index) { _index = index; }

    public int index => _index;
    public List<WaveEnemyData> spawnDataList => _spawnDataList;
}

[System.Serializable]
public class WaveEnemyData
{
    [Header("各敵グループの出現順データ")]
    public List<PopEnemyData> popEnemies = new List<PopEnemyData>();
}

[System.Serializable]
public class PopEnemyData
{
    public EnemyKind spawnKind;
    public Vector3 spawnPosition;
    public bool randomizePosition = true;
}

