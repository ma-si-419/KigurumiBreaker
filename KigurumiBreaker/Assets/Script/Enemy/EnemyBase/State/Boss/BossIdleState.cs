using System.Collections;
using System.Collections.Generic;
using UnityEngine;

# if UNITY_EDITOR
using static UnityEditor.Experimental.GraphView.GraphView;
#endif

public class BossIdleState : IState
{
    private BossEnemy _boss;   //ボス敵の参照

    private float _stateTimer = 0.0f; //状態遷移用タイマー

    //private float _idleToAttackDelay = 1.5f; //待機から攻撃への遅延時間

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
        // 追跡を停止
        _boss.agent.isStopped = true; 

        // 移動を停止
        _boss.StopMovement();

        //プレイヤーとの位置差を計算
        Vector3 diff = _boss.player.transform.position - _boss.transform.position;

        //タイマーで追跡状態へ移行
        _stateTimer += Time.deltaTime;

        // 攻撃を選択する処理
        //_boss.AttackSelect();

        /* 追跡に遷移する処理 */
        if (diff.sqrMagnitude < _boss.enemyData.detectionRange || _stateTimer > _boss.enemyData.idleToChaseTime)
        {
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
