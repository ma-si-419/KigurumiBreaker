using UnityEngine;
using System.Collections.Generic;

public enum EnemyKind
{
    Circle,
    Punch,
    Suicide,
    Tackle,
    Long
}

[System.Serializable]
public class EnemyEntry
{
    public EnemyKind kind;
    public GameObject prefab;
    public float moveSpeed = 3f;
}

[CreateAssetMenu(menuName = "SpawnData")]
public class SpawnData : ScriptableObject
{
    public List<EnemyEntry> enemies = new List<EnemyEntry>();

    public GameObject GetPrefabByKind(EnemyKind kind)
    {
        var entry = enemies.Find(e => e.kind == kind);
        return entry != null ? entry.prefab : null;
    }

    public float GetSpeedByKind(EnemyKind kind)
    {
        var entry = enemies.Find(e => e.kind == kind);
        return entry != null ? entry.moveSpeed : 0f;
    }
}
