using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackType2State : IState
{
    private Enemy _enemy;   //敵の参照

    public AttackType2State(Enemy enemy)
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy;
    }

    public void Init()
    {
        //攻撃フラグリセット
        _enemy.AttackReset();

        //攻撃アニメーション開始
        _enemy.animator.SetTrigger("AttackType2");
    }

    public void Update()
    {
        //攻撃処理
        _enemy.AttackType1();
    }

    public void End()
    {
    }

}
