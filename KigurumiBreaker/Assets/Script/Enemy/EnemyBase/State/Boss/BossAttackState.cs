using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : IState
{
    //ボス敵の参照
    private BossEnemy _boss;   
    //ビヘイビアツリー
    private BhaiviorTree _bhaiviorTree; 

    public BossAttackState(BossEnemy boss)
    {
        //コンストラクタでEnemyの参照を受け取る
        _boss = boss;

       

    }

    public void Init()
    {
        //_boss.AttackReset(); //攻撃フラグリセット
        //_boss.agent.isStopped = true; // 追跡を停止

        //攻撃回数をカウント
        _boss._attackCount++;

        //ビヘイビアツリーの構築
        _bhaiviorTree = new SelectorNode(new SequenceNode
                                        (new ConditionNode(() => _boss._attackCount == 1),
                                         new ActionNode(() => { _boss.MeleeAttack(); return true; }),
                                         new SequenceNode
                                         (new ConditionNode(() => _boss._attackCount == 2), 
                                         new ActionNode(() => { _boss.RangeAttack(); return true; }),
                                         new ActionNode(() => { _boss.LongRangeAttack(); return true; })
                                        )));
    }

    public void Update()
    {
        Debug.Log("攻撃");

        //ビヘイビアツリーの実行
        _bhaiviorTree.Tick();
    }

    public void End()
    {
    }
}
