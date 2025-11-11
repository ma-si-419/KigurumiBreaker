using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IState
{
    private Enemy _enemy;        //敵の参照


    public ChaseState(Enemy enemy) 
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy;
    }

    public void Init()
    {
        //追跡アニメーション開始
        _enemy.animator.SetBool("Chase", true);
        _enemy.agent.isStopped = false; //追跡再開
    }

    public void Update()
    {
        //敵がプレイヤーを向いている方向を取得
        Vector3 directionToPlayer = _enemy.player.transform.position - _enemy.transform.position;

        _enemy.Chase(); //基本移動処理
    }

    public void End()
    {
        // 待機アニメーションを停止
        if(_enemy.isChasetoIdle)
        {
            _enemy.animator.SetBool("Idle", false);
        }
        else
        {
            _enemy.animator.SetBool("Chase", false);
        }
    }

}


