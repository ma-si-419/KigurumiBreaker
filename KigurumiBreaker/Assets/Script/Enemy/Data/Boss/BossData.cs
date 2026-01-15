using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossNameType
{
    None,
    Mohikan,
    TyanTwo,
}

[CreateAssetMenu(menuName = "Boss/BossData")]
public class BossData : ScriptableObject
{
    [Header("ボスの名前")]
    [SerializeField] private BossNameType BossName;         // ボスの名前

    // 読み取り専用
    public BossNameType bossName => BossName;

}
