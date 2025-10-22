using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DamageData")]
public class DamageData : ScriptableObject
{
    [Header("ダメージを受けた際に変化させるマテリアル")]
    [SerializeField] private Material DamageMaterial;
    [Header("弱攻撃を受けた時の硬直時間")]
    [SerializeField] private int LowStanTime;
    [Header("弱攻撃を受けた時のヒットストップ時間")]
    [SerializeField] private int LowHitStop;
    [Header("中攻撃を受けた時の硬直時間")]
    [SerializeField] private int MiddleStanTime;
    [Header("中攻撃を受けた時のノックバックの大きさ")]
    [SerializeField] private int MiddleKnockBackScale;
    [Header("中攻撃を受けた時にノックバックする時間")]
    [SerializeField] private int MiddleKnockBackTime;
    [Header("中攻撃を受けた時のヒットストップ時間")]
    [SerializeField] private int MiddleHitStop;
    [Header("強攻撃を受けた時の硬直時間")]
    [SerializeField] private int HighStanTime;
    [Header("強攻撃を受けた時のノックバックの大きさ")]
    [SerializeField] private int HighKnockBackScale;
    [Header("強攻撃を受けた時にノックバックする時間")]
    [SerializeField] private int HighKnockBackTime;
    [Header("強攻撃を受けた時のヒットストップ時間")]
    [SerializeField] private int HighHitStop;

    // 読み取り専用
    public Material damageMaterial => DamageMaterial;
    public int lowStanTime => LowStanTime;
    public int lowHitStop => LowHitStop;
    public int middleStanTime => MiddleStanTime;
    public int middleKnockBackScale => MiddleKnockBackScale;
    public int middleKnockBackTime => MiddleKnockBackTime;
    public int middleHitStop => MiddleHitStop;
    public int highStanTime => HighStanTime;
    public int highKnockBackScale => HighKnockBackScale;
    public int highKnockBackTime => HighKnockBackTime;
    public int highHitStop => HighHitStop;
}
