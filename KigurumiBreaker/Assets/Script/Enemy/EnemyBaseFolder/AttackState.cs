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
        //Debug.Log("AttackState: Init");
    }

    public void Update()
    {
        Debug.DrawLine(_enemy.transform.position, _enemy.player.transform.position, Color.green);
        //Debug.Log("AttackState: Update");
        _enemy.Attack(); //攻撃処理
    }

    public void End()
    {
        //Debug.Log("AttackState: End");
    }

}
