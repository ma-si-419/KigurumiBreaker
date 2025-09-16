using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZangiAttack : MonoBehaviour
{

    private int damage = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ‘OŒã‚ÉˆÚ“®(sin)
        transform.position += new Vector3(Mathf.Sin(Time.time * 5) * 0.3f, 0, 0);
    }

    public int GetDamage()
    {
        return damage;
    }

    public PlayerState.DamageKind GetDamageKind()
    {
        int rand = Random.Range(0, 3);
        return (PlayerState.DamageKind)rand;
    }
}
