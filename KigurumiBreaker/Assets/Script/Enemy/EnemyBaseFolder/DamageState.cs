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
        Debug.Log("DamageState: Init");
        //ダメージアニメーション開始

    }

    public void Update()
    {
        //タイマーを進める
        Debug.Log("DamageState: Update");

        //プレイヤーの攻撃方向によってノックバックを挟む
        //ノックバックが終わったらIdleStateに遷移

    }

    public void End()
    {
        Debug.Log("DamageState: End");
        //ダメージアニメーション終了

    }
}
