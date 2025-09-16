using UnityEngine;

public class IdleState : IState
{
    private Enemy _enemy;   //敵の参照
    private float _timer;   //タイマー
    

    public IdleState(Enemy enemy) 
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy; 
    }   

    public void Init()
    {
        Debug.Log("IdleState: Init");
        _timer = 0.0f;
    }

    public void Update()
    {
        //タイマーを進める
        _timer += Time.deltaTime;
        Debug.Log("IdleState: Update");

        //プレイヤーを発見したら追跡状態へ
        if (_timer > 10.0f)
        {
            //状態を変更する
            Debug.Log("IdleState: Change to ChaseState");
            _enemy.ChangeState(new ChaseState(_enemy));
        }

    }

    public void End()
    {
        Debug.Log("IdleState: End");
    }

}

