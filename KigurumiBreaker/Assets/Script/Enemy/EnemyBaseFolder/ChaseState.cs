using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IState
{
    private Enemy _enemy;        //敵の参照


    public ChaseState(Enemy enemy) 
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy;
        _enemy.agent.isStopped = false; //追跡を停止

    }

    public void Init()
    {
        Debug.Log("ChaseState: Init");
    }

    public void Update()
    {
        //敵がプレイヤーを向いている方向を取得
        Vector3 directionToPlayer = _enemy.playerTrans.position - _enemy.transform.position;

        _enemy.Move(); //基本移動処理
    }

    public void End()
    {
        Debug.Log("ChaseState: End");
    }

}


