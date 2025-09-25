using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TackleEnemy : Enemy
{ 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Attack()
    {



        base.Attack();

        // ここにタックル攻撃の具体的な処理を追加
        Debug.Log("PunchEnemy: Performing punch attack!");

        
    }

}
