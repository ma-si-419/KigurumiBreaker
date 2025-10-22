using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IState
{
    private Enemy _enemy;   //敵の参照

    public DeadState(Enemy enemy)
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy; 
    }   

    public void Init()
    {
        //死亡アニメーション開始
        _enemy.animator.SetTrigger("Dead");
    }

    public void Update()
    {
        _enemy.StopMovement(); //移動停止
        BattleManager manager = _enemy.battleManager.GetComponent<BattleManager>();

        //死亡アニメーションが終わったらオブジェクト削除
        var stateInfo = _enemy.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Dead") && stateInfo.normalizedTime >= 1.0f)
        {
            manager.RemoveEnemy(_enemy.gameObject);
            Object.Destroy(_enemy.gameObject);
        }
    }

    public void End()
    {
        //死亡アニメーション終了
        _enemy.animator.ResetTrigger("Dead");
    }
}
