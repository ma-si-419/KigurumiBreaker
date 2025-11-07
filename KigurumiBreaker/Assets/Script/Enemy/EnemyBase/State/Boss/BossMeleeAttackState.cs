using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMeleeAttackState : IState
{
    //ボス敵の参照
    private BossEnemy _boss;

    public BossMeleeAttackState(BossEnemy boss)
    {
        //コンストラクタでEnemyの参照を受け取る
        _boss = boss;
    }

    public void Init()
    {
        //通常攻撃アニメーション開始
        _boss.animator.SetTrigger("MeleeAttack");
    }

    public void Update()
    {
        _boss.MeleeAttack();
    }

    public void End()
    {
        //通常攻撃アニメーション終了
        _boss.animator.ResetTrigger("MeleeAttack");
    }

}
