using UnityEngine;
using System.Collections.Generic;

public enum EnemyKind
{
    Circle,
    Punch,
    Bomb,
    Tackle,
    Long,
    ArmorCircle,
    ArmorPunch,
    ArmorBomb,
    ArmorTackle,
    ArmorLong,
    Boss1,
}

[System.Serializable]
public class EnemyEntry
{
    public EnemyKind kind;
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "SpawnData", menuName = "Game/Spawn Data")]
public class SpawnData : ScriptableObject
{
    [Header("敵プレハブリスト")]
    [SerializeField] private List<EnemyEntry> _enemies = new List<EnemyEntry>();

    public GameObject GetPrefabByKind(EnemyKind kind)
    {
        var entry = _enemies.Find(e => e.kind == kind);
        return entry != null ? entry.prefab : null;
    }
}
