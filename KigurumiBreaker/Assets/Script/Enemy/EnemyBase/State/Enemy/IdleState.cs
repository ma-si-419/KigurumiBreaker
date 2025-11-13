using Unity.VisualScripting;
using UnityEngine;

public class IdleState : IState
{
    private Enemy _enemy;   //敵の参照

    public IdleState(Enemy enemy) 
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy; 
    }

    public void Init()
    {
        _enemy.AttackReset(); //攻撃フラグリセット
        _enemy.agent.isStopped = true; // 追跡を停止

        //待機アニメーション開始
        _enemy.animator.SetBool("Idle", true);
    }

    public void Update()
    {
        //基本待機処理
        _enemy.Idle();
    }

    public void End()
    {
        //待機アニメーション終了
        _enemy.animator.SetBool("Idle", false);
    }
}

