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
        //NavMeshAgent停止
        _boss.agent.enabled = false;
        //攻撃アニメーション開始
        _boss.animator.SetTrigger("Phase");
    }

    public void Update()
    {
        _boss.PhaseChange(); //基本フェーズ変更
    }

    public void End()
    {
        //NavMeshAgent再開
        _boss.agent.enabled = true;
        //フェーズアニメーション終了
        _boss.animator.ResetTrigger("Phase");
    }
}
