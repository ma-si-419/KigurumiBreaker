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

        Vector3 diff = _enemy.playerTrans.position - _enemy.transform.position; //プレイヤーとの位置差を計算
        //Debug.Log(diff);

        //攻撃圏内に入ると攻撃状態へ
        //プレイヤーが検知範囲内にいるかチェック
        if (diff.sqrMagnitude < _enemy.attackRangeSqr && _timer > 3.0f)
        {
            Debug.Log("IdleState: Change to ChaseState");
            _enemy.agent.isStopped = true; //追跡を停止
            _enemy.ChangeState(new AttackState(_enemy));
        }

    }

    public void End()
    {
        Debug.Log("ChaseState: End");
    }

}


