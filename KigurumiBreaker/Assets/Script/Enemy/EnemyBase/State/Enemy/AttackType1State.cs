using UnityEngine;

public class AttackType1State : IState
{
    private Enemy _enemy;   //敵の参照

    public AttackType1State(Enemy enemy) 
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy;
    }  

    public void Init()
    {
        //攻撃フラグリセット
        _enemy.AttackReset(); 

        //攻撃アニメーション開始
        _enemy.animator.SetTrigger("AttackType1");
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
