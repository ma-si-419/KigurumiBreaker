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
        //ボスの攻撃アニメーションを開始

        //攻撃回数をカウント
        _boss._attackCount++;
    }

    public void Update()
    {
        Debug.Log("攻撃");



    }

    public void End()
    {
    }
}
