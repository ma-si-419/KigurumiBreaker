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
        _enemy._animator.SetTrigger("Damage");
    }

    public void Update()
    {
        //タイマーを進める
        //Debug.Log("DamageState: Update");
        //Debug.Log("ノックバック！！");


        //ダメージアニメーション終了を検知したらIdleStateに遷移
        _enemy.ChangeState(new IdleState(_enemy));

        //プレイヤーの攻撃方向によってノックバックを挟む


    }

    public void End()
    {
        //Debug.Log("DamageState: End");
        //ダメージアニメーション終了

    }
}
