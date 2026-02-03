using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeadState : IState
{
    private BossEnemy _boss;   //ボス敵の参照

    public BossDeadState(BossEnemy boss)
    {
        //コンストラクタでEnemyの参照を受け取る
        _boss = boss;
    }

    public void Init()
    {
        //_boss.AttackReset(); //攻撃フラグリセット
        _boss.agent.isStopped = true; // 追跡を停止

        //死亡アニメーション開始
        _boss.animator.SetTrigger("Dead");
    }

    public void Update()
    {
        _boss.StopMovement(); //移動停止
        BattleManager manager = _boss.battleManager.GetComponent<BattleManager>();

        // 死んだら攻撃判定オブジェクトを削除する
        Object.Destroy(_boss.attackObj);
        Object.Destroy(_boss.phaseEffectObj);

        // 死んだら敵が持っているエフェクトを全て削除する
        for (int i = 0; i < _boss.effectObj.Length; i++)
        {
            Object.Destroy(_boss.effectObj[i]);
        }

        //死亡アニメーションが終わったらオブジェクト削除
        var stateInfo = _boss.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Dead") && stateInfo.normalizedTime >= 1.0f)
        {
            manager.RemoveEnemy(_boss.gameObject);
            Object.Destroy(_boss.gameObject);
        }

    }

    public void End()
    {
        //死亡アニメーション終了
        _boss.animator.ResetTrigger("Dead");
    }
}
