using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SpawnEnemyData", menuName = "GameData/SpawnEnemyData")]
public class SpawnEnemyData : ScriptableObject
{
    [Header("グループ内で出てくる敵まとめ")]
    [SerializeField] private List<WaveEnemyData> _waveEnemyDataList = new List<WaveEnemyData>();

    [HideInInspector][SerializeField] private int _index = 0;

    public void SetIndex(int index)
    {
        _index = index;
    }

    // 読み取り専用プロパティ
    public int index => _index;

    public List<WaveEnemyData> waveEnemyDataList => _waveEnemyDataList;
}

[System.Serializable]
public class WaveEnemyData
{
    [Header("出てくる敵の種類と座標")]
    [SerializeField] private List<PopEnemyData> _popEnemies = new List<PopEnemyData>();

    // 読み取り専用プロパティ
    public List<PopEnemyData> popEnemies => _popEnemies;

}

[System.Serializable]
public class PopEnemyData
{
    public EnemyKind spawnKind;
    public Vector3 spawnPosition;
    public bool randomizePosition = true;
}
