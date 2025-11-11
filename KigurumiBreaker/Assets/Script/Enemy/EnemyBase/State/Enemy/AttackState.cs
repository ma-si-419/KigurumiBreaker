using UnityEngine;

public class AttackState : IState
{
    private Enemy _enemy;   //敵の参照

    public AttackState(Enemy enemy) 
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy;
    }  

    public void Init()
    {
        //攻撃フラグリセット
        _enemy.AttackReset(); 

        //攻撃アニメーション開始
        _enemy.animator.SetTrigger("Attack");
    }

    public void Update()
    {
        //攻撃処理
        _enemy.Attack(); 
    }

    public void End()
    {
    }

}
