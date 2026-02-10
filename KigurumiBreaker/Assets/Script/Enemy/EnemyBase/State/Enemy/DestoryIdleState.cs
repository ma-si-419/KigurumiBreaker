using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIdleState : IState
{
    private Enemy _enemy;   //敵の参照

    public DestroyIdleState(Enemy enemy)
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy;
    }

    public void Init()
    {
        _enemy.agent.isStopped = true; // 追跡を停止

        Debug.Log("敵を破壊する状態");

        //待機アニメーション開始
        _enemy.animator.SetBool("Idle", false);
    }

    public void Update()
    {
        _enemy.StopMovement(); //移動停止

        //オブジェクトを破棄
        GameObject.Destroy(_enemy.gameObject);
    }

    public void End()
    {
        //待機アニメーション終了
        _enemy.animator.SetBool("Idle", false);
    }
}
