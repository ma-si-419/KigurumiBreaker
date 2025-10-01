using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IState
{
    private Enemy _enemy;   //敵の参照

    public DeadState(Enemy enemy)
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy; 
    }   

    public void Init()
    {
        Debug.Log("DeadState: Init");
        //死亡アニメーション開始

    }

    public void Update()
    {
        //タイマーを進める
        Debug.Log("DeadState: Update");
        Debug.Log("死んだ");

        //死亡アニメーションが終わったらオブジェクト削除
        //オブジェクト削除
        
    }

    public void End()
    {
        Debug.Log("DeadState: End");
        //死亡アニメーション終了

    }
}
