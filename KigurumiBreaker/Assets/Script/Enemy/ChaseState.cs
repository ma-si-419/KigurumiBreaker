using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IState
{
    private Enemy _enemy;        //敵の参照
    private float _timer;        //タイマー


    public ChaseState(Enemy enemy) 
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy;
        _enemy.agent.isStopped = false; //追跡を停止

    }

    public void Init()
    {
        _timer = 0.0f;
        Debug.Log("ChaseState: Init");
    }

    public void Update()
    {
        //タイマーを進める
        _timer += Time.deltaTime;
        Debug.Log("ChaseState: Update");

        _enemy.agent.SetDestination(_enemy.player.transform.position); //プレイヤーの位置を目的地に設定

        //攻撃圏内に入ると攻撃状態へ
        if (_timer > 18.0f)
        {
            //状態を変更する
            Debug.Log("ChaseState: Change to AttackState");
            _enemy.agent.isStopped = true; //追跡を停止
            _enemy.ChangeState(new AttackState(_enemy));
        }
    }

    public void End()
    {
        Debug.Log("ChaseState: End");
    }

}


