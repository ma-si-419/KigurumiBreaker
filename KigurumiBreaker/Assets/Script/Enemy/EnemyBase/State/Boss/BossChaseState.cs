using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BossChaseState : IState
{
    //ボス敵の参照
    private BossEnemy _boss;
    //状態遷移用タイマー
    private float _stateTimer;


    public BossChaseState(BossEnemy boss)
    {
        //コンストラクタでEnemyの参照を受け取る
        _boss = boss;
    }

    public void Init()
    {
        // 追跡を再開
        _boss.agent.isStopped = false; 
        //待機アニメーション開始
        _boss.animator.SetBool("Chase", true);
    }

    public void Update()
    {
        Debug.Log("追跡中");

        //プレイヤーの位置を目的地に設定
        _boss.agent.SetDestination(_boss.player.transform.position);
        // Rigidbodyの移動を停止(プレイヤーと衝突した際に吹っ飛ばされないため)
        _boss.StopMovement();
        //プレイヤーの方向を向き続ける
        _boss.LookAtPlayer();

        //プレイヤーとの位置差を計算
        Vector3 diff = _boss.player.transform.position - _boss.transform.position;

        //追跡を停止
        //タイマーを進める(スピード感を出すため一旦除外)
        _stateTimer += Time.deltaTime;

        //if (_stateTimer > _boss.enemyData.chaseToAttack)
        {
            //近い距離なら近接攻撃へ
            if (diff.sqrMagnitude < _boss.meleeAttackRangeSqr)
            {
                //通常攻撃へ移行
                _boss.ChangeState(new BossMeleeAttackState(_boss));
            }
            //中距離なら突進攻撃へ
            else if (diff.sqrMagnitude < _boss.specialAttackRangeSqr)
            {
                //突進攻撃へ移行
                _boss.ChangeState(new BossAttackState(_boss));
            }
        }

    }

    public void End()
    {
        //待機アニメーション終了
        _boss.animator.SetBool("Chase", false);
    }
}
