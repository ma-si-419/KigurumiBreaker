using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZangiMove : MonoBehaviour
{

    public int hp = 30;

    private bool isDamage = false;

    int time = 0;

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

            hp -= other.gameObject.GetComponent<PlayerAttack>().GetDamage();

            Debug.Log(other.gameObject.GetComponent<PlayerAttack>().GetDamage() + "�̃_���[�W");

            isDamage = true;

            time = 0;

            this.GetComponent<Renderer>().material.color = Color.red;

            if (hp <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
