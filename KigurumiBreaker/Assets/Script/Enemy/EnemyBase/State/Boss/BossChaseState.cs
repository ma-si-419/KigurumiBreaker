using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChaseState : IState
{
    private BossEnemy _enemy;   //ボス敵の参照

    public BossChaseState(BossEnemy enemy)
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy;
    }

    public void Init()
    {
        _enemy.agent.isStopped = false; // 追跡を停止
        //_enemy.AttackReset(); //攻撃フラグリセット

        ////待機アニメーション開始
        //_enemy.animator.SetBool("Idle", false);
    }

    public void Update()
    {
        //基本待機処理
        _enemy.Chase();
    }

    public void End()
    {
        //待機アニメーション終了
        //_enemy.animator.SetBool("Idle", false);
    }
}
