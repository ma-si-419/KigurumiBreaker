using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BossAttackState : IState
{
    //ボス敵の参照
    private BossEnemy _boss;   

    public BossAttackState(BossEnemy boss)
    {
        //コンストラクタでEnemyの参照を受け取る
        _boss = boss;
    }

    public void Init()
    {
        //ボスの攻撃アニメーションを開始
        //_cooldownTimer = 0.0f;
        // 追跡を停止
        //_boss.agent.isStopped = true;
        //NavMeshAgent停止
        _boss.agent.enabled = false;
        //攻撃フラグリセット
        _boss.AttackReset();
        //攻撃アニメーション開始
        _boss.animator.SetTrigger("AttackType1");
    }

    public void Update()
    {

        _boss.Attack();
    }

    public void End()
    {
        //NavMeshAgent再開
        _boss.agent.enabled = true;
        //攻撃アニメーション終了
        _boss.animator.ResetTrigger("AttackType1");
    }
}
