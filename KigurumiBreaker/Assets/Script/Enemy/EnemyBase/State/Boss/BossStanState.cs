using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStanState : IState
{
    private BossEnemy _boss;   //ボス敵の参照

    public BossStanState(BossEnemy boss)
    {
        //コンストラクタでEnemyの参照を受け取る
        _boss = boss;
    }

    public void Init()
    {
        //_boss.AttackReset(); //攻撃フラグリセット
        _boss.agent.isStopped = true; // 追跡を停止

        //ダウンアニメーション開始
        _boss.animator.SetBool("Down", true);
    }

    public void Update()
    {
        //_boss.Idle(); //基本待機処理
    }

    public void End()
    {
        //待機アニメーション終了
        _boss.animator.SetBool("Down", false);
    }
}
