using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZangiAttack : MonoBehaviour
{

    private int damage = 10;

    [Header("0~2")]
    [SerializeField]private int damageKind = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // �O��Ɉړ�(sin)
        transform.position += new Vector3(Mathf.Sin(Time.time * 5) * 0.3f, 0, 0);
    }

    public int GetDamage()
    {
        return damage;
    }

    public PlayerState.DamageKind GetDamageKind()
    {
        return (PlayerState.DamageKind)damageKind;
    }
}
