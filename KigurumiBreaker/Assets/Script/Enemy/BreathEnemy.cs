using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathEnemy : Enemy
{
    public override void Attack()
    {

        Debug.Log("UŒ‚");

        //‘Ò‹@ó‘Ô‚Ö
        ChangeState(new IdleState(this));
    }
}
