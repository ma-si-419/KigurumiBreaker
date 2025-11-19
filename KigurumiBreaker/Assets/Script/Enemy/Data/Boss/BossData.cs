using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/BossData")]
public class BossData : ScriptableObject
{
    [Header("まぁなんか色々入れる")]

    [Header("攻撃タイプ3プレハブ")]
    [SerializeField] private GameObject AttackType3Prefab;
    [Header("攻撃タイプ4プレハブ")]
    [SerializeField] private GameObject AttackType4Prefab;


    // 読み取り専用
    public GameObject attackType3Prefab => AttackType3Prefab;
    public GameObject attackType4Prefab => AttackType4Prefab;

}
