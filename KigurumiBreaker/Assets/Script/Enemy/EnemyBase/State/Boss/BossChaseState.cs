using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BossChaseState : IState
{
    //ボス敵の参照
    private BossEnemy _boss;   

    //private float _stateTimer;

    public BossChaseState(BossEnemy boss)
    {
        //コンストラクタでEnemyの参照を受け取る
        _boss = boss;
    }

    public void Init()
    {
        _boss.agent.isStopped = false; // 追跡を停止
        //_enemy.AttackReset(); //攻撃フラグリセット

        ////待機アニメーション開始
        //_enemy.animator.SetBool("Idle", false);
    }

    public void Update()
    {
        // ボス専用の追跡処理をここに追加(正味ここはザコ敵と同じ処理)
        Debug.Log("追跡");

        //プレイヤーの位置を目的地に設定
        _boss.agent.SetDestination(_boss.player.transform.position);
        // Rigidbodyの移動を停止(プレイヤーと衝突した際に吹っ飛ばされないため)
        _boss.StopMovement();

        //プレイヤーとの位置差を計算
        Vector3 diff = _boss.player.transform.position - _boss.transform.position;

        //攻撃圏内に入ると攻撃状態へ
        //プレイヤーが検知範囲内にいるかチェック
        //if (diff.sqrMagnitude < _attackRangeSqr)
        {
            //プレイヤーの方向を向き続ける
            _boss.LookAtPlayer();

            //追跡を停止
            _boss.agent.isStopped = true;

            //攻撃状態へ
            //_stateTimer = 0.0f;
            _boss.ChangeState(new BossAttackState(_boss));

            //タイマーを進める(スピード感を出すため一旦除外)
            //_stateTimer += Time.deltaTime;
            //if (_stateTimer > _enemyData.chaseToAttack)
            //{
            //}
        }

    }

    public void End()
    {
        //待機アニメーション終了
        //_enemy.animator.SetBool("Idle", false);
    }
}
