using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BossIdleState : IState
{
    private BossEnemy _boss;   //ボス敵の参照

    private float _stateTimer = 0.0f; //状態遷移用タイマー

    public BossIdleState(BossEnemy boss)
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
        // ボス専用の待機処理をここに追加
        _boss.agent.isStopped = true; // 追跡を停止
        //プレイヤーの位置を目的地に設定
        _boss.agent.SetDestination(_boss.player.transform.position);

        Debug.Log("待機");

        // 移動を停止
        _boss.StopMovement();

        Vector3 diff = _boss.player.transform.position - _boss.transform.position; //プレイヤーとの位置差を計算

        //タイマーで追跡状態へ移行
        _stateTimer += Time.deltaTime;

        if (_stateTimer > _boss.enemyData.idleToChaseTime)
        {
            //追跡状態へ
            _stateTimer = 0.0f;
            _boss.ChangeState(new BossChaseState(_boss));
        }
    }

    public void End()
    {
        //待機アニメーション終了
        _boss.animator.SetBool("Idle", false);
    }
}
