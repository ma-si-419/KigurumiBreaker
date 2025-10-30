using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugIdleState : IState
{
    private Enemy _enemy;   //敵の参照

    public DebugIdleState(Enemy enemy)
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy;
    }

    public void Init()
    {
        _enemy.AttackReset(); //攻撃フラグリセット
        _enemy.agent.isStopped = true; // 追跡を停止

        //待機アニメーション開始
        _enemy.animator.SetBool("Idle", false);
    }

    public void Update()
    {
        _enemy.StopMovement(); //移動停止
    }

    public void End()
    {
        //待機アニメーション終了
        _enemy.animator.SetBool("Idle", false);
    }
}