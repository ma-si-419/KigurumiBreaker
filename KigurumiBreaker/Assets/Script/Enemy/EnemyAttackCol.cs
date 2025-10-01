using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCol : MonoBehaviour
{
    [Header("ヒットダメージ設定")]
    [SerializeField]private int _damage;            // ヒットダメージ
    [SerializeField]private int _damageKind;        // 0:通常 1:火炎 2:氷結
    [SerializeField]private int _lifeTime;         // 攻撃判定の寿命（フレーム数）
    //[SerializeField] private GameObject _hitEffectPrefab;

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        _lifeTime--;

        if(_lifeTime <= 0)
        {
            //攻撃判定の寿命が来たら消す
            Destroy(this.gameObject);
        }

    }

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
