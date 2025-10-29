using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyConstantData")]
public class EnemyConstantData : ScriptableObject
{
    [Header("ヒットストップの揺れる大きさ")]
    [SerializeField] private float ShakeMagnitude;

    // 読み取り専用
    public float shakeMagnitude => ShakeMagnitude;

}
