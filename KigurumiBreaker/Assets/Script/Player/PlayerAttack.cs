using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    private AttackData attackData;

    int lifeTIme = 0;

    public static int num = 0;

    // Start is called before the first frame update
    void Start()
    {
        lifeTIme = attackData.attackLifeTime;
        Debug.Log(attackData);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lifeTIme--;

        if(lifeTIme <= 0)
        {
            //UŒ‚”»’è‚ÌŽõ–½‚ª—ˆ‚½‚çÁ‚·
            Destroy(this.gameObject);
        }

    }

    public void SetAttackData(AttackData data)
    {
        this.attackData = data;
    }
}
