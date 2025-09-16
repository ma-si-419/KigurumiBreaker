using UnityEngine;

public class AttackState : IState
{
    private Enemy _enemy;   //敵の参照
    private float _timer;   //タイマー

    public AttackState(Enemy enemy) 
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy;
    }  

    public void Init()
    {
        _timer = 0.0f;
        Debug.Log("AttackState: Init");
    }

    public void Update()
    {
        //タイマーを進める
        _timer += Time.deltaTime;
        Debug.Log("AttackState: Update");

        //プレイヤーを攻撃したら待機状態へ
        if (_timer > 10.0f)
        {
            //状態を変更する
            Debug.Log("AttackState: Change to IdleState");
            _enemy.ChangeState(new IdleState(_enemy));
        }
    }

    public void End()
    {
        Debug.Log("AttackState: End");
    }

}
