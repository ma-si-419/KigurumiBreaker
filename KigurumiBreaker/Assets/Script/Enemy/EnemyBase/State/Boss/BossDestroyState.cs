using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDestroyState : IState
{
    private BossEnemy _boss;   //ボス敵の参照

    public BossDestroyState(BossEnemy boss)
    {
        //コンストラクタでEnemyの参照を受け取る
        _boss = boss;
    }

    public void Init()
    {
        _boss.agent.isStopped = true; // 追跡を停止

        //待機アニメーション開始
        _boss.animator.SetBool("Idle", true);
    }

    public void Update()
    {
        // 移動を停止
        _boss.StopMovement();

        //オブジェクトを破棄
        GameObject.Destroy(_boss.gameObject);
    }

    public void End()
    {
        //待機アニメーション終了
        _boss.animator.SetBool("Idle", false);
    }
}