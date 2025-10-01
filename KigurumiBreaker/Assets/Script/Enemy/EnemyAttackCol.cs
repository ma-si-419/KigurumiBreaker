using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCol : MonoBehaviour
{
    [Header("ヒットダメージ設定")]
    [SerializeField]private int _damage;
    [SerializeField]private int _damageKind;
    //[SerializeField] private GameObject _hitEffectPrefab;

    public int GetDamage()
    {
        return _damage;
    }

    public PlayerState.DamageKind GetDamageKind()
    {
        return (PlayerState.DamageKind)_damageKind;
    }

    //public GameObject GetHitEffectPrefab()
    //{
    //    return _hitEffectPrefab;
    //}



}
