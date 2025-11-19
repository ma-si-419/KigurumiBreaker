using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackType4State : IState
{
    //ボス敵の参照
    private BossEnemy _boss;

    public BossAttackType4State(BossEnemy boss)
    {
        //コンストラクタでEnemyの参照を受け取る
        _boss = boss;
    }

    public void Init()
    {
        //NavMeshAgent停止
        _boss.agent.enabled = false;
        //攻撃フラグリセット
        _boss.AttackReset();
        //攻撃アニメーション開始
        _boss.animator.SetTrigger("AttackType2");
    }

    public void Update()
    {

        _boss.AttackType4();
    }

    public void End()
    {
        //NavMeshAgent再開
        _boss.agent.enabled = true;
        //攻撃アニメーション終了
        _boss.animator.ResetTrigger("AttackType2");
    }
}
