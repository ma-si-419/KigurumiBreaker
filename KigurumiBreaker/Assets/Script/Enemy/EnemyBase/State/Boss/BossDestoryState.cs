using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDestoryState : IState
{
    private BossEnemy _boss;   //ボス敵の参照

    private float _stateTimer = 0.0f; //状態遷移用タイマー

    //private float _idleToAttackDelay = 1.5f; //待機から攻撃への遅延時間

    public BossDestoryState(BossEnemy boss)
    {
        //コンストラクタでEnemyの参照を受け取る
        _boss = boss;
    }

    public void Init()
    {
        _boss.AttackReset(); //攻撃フラグリセット
        _boss.agent.isStopped = true; // 追跡を停止

        //待機アニメーション開始
        _boss.animator.SetBool("Idle", true);
    }

    public void Update()
    {
        // 移動を停止
        _boss.StopMovement();

        //オブジェクトを破棄
        Object.Destroy(_boss.gameObject);
    }

    public void End()
    {
        //待機アニメーション終了
        _boss.animator.SetBool("Idle", false);
    }
}