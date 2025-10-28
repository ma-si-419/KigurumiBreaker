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
        _enemy.AttackReset(); //攻撃フラグリセット

        _enemy.animator.SetBool("Idle", false); //待機アニメーション終了

        //攻撃アニメーション開始
        _enemy.animator.SetTrigger("Attack");
    }

    public void Update()
    {
        _enemy.Attack(); //攻撃処理


    }

    public void End()
    {
    }

}
