using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IState
{
    private Enemy _enemy;   //敵の参照

    private int _timer = 0;

    private bool _isAnimEnd;

    public DeadState(Enemy enemy)
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy; 
    }   

    public void Init()
    {
        //死亡アニメーション開始
        _enemy.animator.SetTrigger("Dead");
    }

    public void Update()
    {
        _enemy.StopMovement(); //移動停止
        BattleManager manager = _enemy.battleManager.GetComponent<BattleManager>();


        // 死んだら攻撃判定オブジェクトを削除する
        Object.Destroy(_enemy.attackObj);

        // 死んだら敵が持っているエフェクトを全て削除する
        for (int i = 0; i < _enemy.effectObj.Length; i++)
        {
            Object.Destroy(_enemy.effectObj[i]);
        }

        //死亡アニメーションが終わったらオブジェクト削除
        var stateInfo = _enemy.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Dead") && stateInfo.normalizedTime >= 1.0f)
        {
            _enemy.OnDeadEffect(); // 死亡エフェクト再生

            _isAnimEnd = true;
        }

        if (_isAnimEnd)
        {
            _timer++;
        }

        if (_timer > 40)
        {
            Object.Destroy(_enemy.deadEffectObj);

            manager.RemoveEnemy(_enemy.gameObject);
            // 敵自身を削除
            Object.Destroy(_enemy.gameObject);
        }
    }

    public void End()
    {
        //死亡アニメーション終了
        _enemy.animator.ResetTrigger("Dead");
    }
}
