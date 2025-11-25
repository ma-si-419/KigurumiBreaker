using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAttackType2State : IState
{
    //ボス敵の参照
    private BossEnemy _boss;

    public BossAttackType2State(BossEnemy boss)
    {
        //コンストラクタでEnemyの参照を受け取る
        _boss = boss;
    }

    public void Init()
    {
        _boss.agent.enabled = false;
        _boss.AttackReset();
        //通常攻撃アニメーション開始
        _boss.animator.SetTrigger("AttackType2");
    }

    public void Update()
    {
        _boss.AttackType2();
    }

    public void End()
    {
        _boss.agent.enabled = true;
        //通常攻撃アニメーション終了
        _boss.animator.ResetTrigger("AttackType2");
    }

}
