using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseState : IState
{
    private BossEnemy _boss;   //ボス敵の参照

    public BossPhaseState(BossEnemy boss)
    {
        //コンストラクタでEnemyの参照を受け取る
        _boss = boss;
    }

    public void Init()
    {
        //_boss.AttackReset(); //攻撃フラグリセット
        _boss.agent.isStopped = true; // 追跡を停止

        //フェーズアニメーション開始
        _boss.animator.SetBool("Down", true);
    }

    public void Update()
    {
        //_boss.Idle(); //基本待機処理
        
        _boss.PhaseChange(); //基本フェーズ変更
    }

    public void End()
    {
        //フェーズアニメーション終了
        _boss.animator.SetBool("Down", false);
    }
}
