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

    // 読み取り専用
    public GameObject specialAttackChargeEffectPrefab => SpecialAttackChargeEffectPrefab;
    public GameObject specialAttackEffectPrefab => SpecialAttackEffectPrefab;
}
