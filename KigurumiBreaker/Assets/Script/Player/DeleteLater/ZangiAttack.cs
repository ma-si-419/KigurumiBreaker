using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZangiAttack : MonoBehaviour
{

    public int damage = 10;

    [Header("0~2")]
    [SerializeField]private int damageKind = 0;
    [SerializeField]private GameObject hitEffectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ‘OŒã‚ÉˆÚ“®(sin)
        transform.position += new Vector3(Mathf.Sin(Time.time * 5) * 0.1f, 0, 0);
    }

    public int GetDamage()
    {
        return damage;
    }
    public GameObject GetHitEffectPrefab()
    {
        return hitEffectPrefab;
    }

    public PlayerState.DamageKind GetDamageKind()
    {
        return (PlayerState.DamageKind)damageKind;
    }
}
