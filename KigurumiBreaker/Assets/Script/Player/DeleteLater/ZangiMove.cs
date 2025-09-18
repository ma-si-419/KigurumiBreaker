using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZangiMove : MonoBehaviour
{

    public int maxHp = 30;
    public int nowHp = 0;

    private bool isDamage = false;

    int time = 0;


    void Start()
    {
        nowHp = maxHp;
    }

    void FixedUpdate()
    {
        if (isDamage)
        {
            time++;

            if (time > 15)
            {
                isDamage = false;
            }
        }
        else
        {
            time = 0;
            
            this.GetComponent<Renderer>().material.color = Color.white;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerAttack")
        {

            nowHp -= other.gameObject.GetComponent<PlayerAttack>().GetDamage();

            Debug.Log(other.gameObject.GetComponent<PlayerAttack>().GetDamage() + "ÇÃÉ_ÉÅÅ[ÉW");

            isDamage = true;

            time = 0;

            this.GetComponent<Renderer>().material.color = Color.red;

            if (nowHp <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
