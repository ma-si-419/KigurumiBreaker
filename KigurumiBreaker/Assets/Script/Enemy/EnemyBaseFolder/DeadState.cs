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
        //死亡アニメーション開始
        _enemy._animator.SetTrigger("Dead");
    }

    public void Update()
    {
        //タイマーを進める
        //Debug.Log("DeadState: Update");
        //Debug.Log("死んだ");

        //死亡アニメーションが終わったらオブジェクト削除
        if (_enemy._animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            Object.Destroy(_enemy.gameObject);
        }
    }

    public void End()
    {
        //Debug.Log("DeadState: End");
        //死亡アニメーション終了

    }
}
