using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerEffectData")]
public class PlayerEffectData : ScriptableObject
{
    [Header("特殊攻撃チャージ中のエフェクト")]
    [SerializeField] private GameObject SpecialAttackChargeEffectPrefab;

    // 読み取り専用
    public GameObject specialAttackChargeEffectPrefab => SpecialAttackChargeEffectPrefab;
}
