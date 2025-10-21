using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : IState
{
    //ボス敵の参照
    private BossEnemy _boss;   
    //ビヘイビアツリー
    private BhaiviorTree _bhaiviorTree;

    //攻撃クールダウンタイマー
    private float _cooldownTimer = 0.0f; 

    public BossAttackState(BossEnemy boss)
    {
        //コンストラクタでEnemyの参照を受け取る
        _boss = boss;
    }

    public void Init()
    {
        //ボスの攻撃アニメーションを開始
        _cooldownTimer = 0.0f;
        // 追跡を停止
        _boss.agent.isStopped = true;
        //攻撃フラグリセット
        _boss.AttackReset(); 
        //攻撃アニメーション開始
        _boss.animator.SetTrigger("Attack");
    }

    public void Update()
    {
        Debug.Log("ふつう");



    }

    public void End()
    {

    }
}
