using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerEffectData")]
public class PlayerEffectData : ScriptableObject
{
    [Header("特殊攻撃チャージ中のエフェクト")]
    [SerializeField] private GameObject SpecialAttackChargeEffectPrefab;
    [Header("特殊攻撃のエフェクト")]
    [SerializeField] private GameObject SpecialAttackEffectPrefab;
    [Header("ダッシュのエフェクト")]
    [SerializeField] private GameObject DashEffectPrefab;
    [Header("ダッシュの開始エフェクト")]
    [SerializeField] private GameObject StartDashEffectPrefab;
    [Header("死亡時の爆発エフェクト")]
    [SerializeField] private GameObject DeathEffectPrefab;
    [Header("チャージエフェクト")]
    [SerializeField] private GameObject ChargeEffectPrefab;

    // 読み取り専用
    public GameObject specialAttackChargeEffectPrefab => SpecialAttackChargeEffectPrefab;
    public GameObject specialAttackEffectPrefab => SpecialAttackEffectPrefab;
    public GameObject dashEffectPrefab => DashEffectPrefab;
    public GameObject startDashEffectPrefab => StartDashEffectPrefab;
    public GameObject deathEffectPrefab => DeathEffectPrefab;
    public GameObject chargeEffectPrefab => ChargeEffectPrefab;
}
