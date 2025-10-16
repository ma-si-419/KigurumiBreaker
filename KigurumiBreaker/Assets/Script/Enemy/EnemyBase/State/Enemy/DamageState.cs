using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageState : IState
{
    private Enemy _enemy;   //敵の参照

    public DamageState(Enemy enemy)
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy;
    }

    public void Init()
    {
        //ダメージアニメーション開始
        _enemy.animator.SetTrigger("Damage");
    }

    public void Update()
    {
        //ノックバック処理(Playerから攻撃方向を取得してその方向に少しずれる)
        _enemy.StopMovement(); //移動停止

        //死亡アニメーションが終わったらオブジェクト削除
        var stateInfo = _enemy.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Damage") && stateInfo.normalizedTime >= 1.0f)
        {
            //待機状態に遷移
            _enemy.ChangeState(new IdleState(_enemy));
        }
    }

    public void End()
    {
        //ダメージアニメーション終了
        _enemy.animator.ResetTrigger("Damage");
    }
}
