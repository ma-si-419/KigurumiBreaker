using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IState
{
    private Enemy _enemy;   //敵の参照
    private float _timer;   //タイマー

    public DeadState(Enemy enemy)
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy; 
    }   

    public void Init()
    {
        _timer = 0.0f;
        Debug.Log("AttackState: Init");
    }

    public void Update()
    {
        //タイマーを進める
        _timer += Time.deltaTime;
        Debug.Log("AttackState: Update");

        //死んだ処理書いとけ

    }

    public void End()
    {
        Debug.Log("AttackState: End");
    }
}
