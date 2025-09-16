using UnityEngine;
using UnityEngine.XR;

public class IdleState : IState
{
    private Enemy _enemy;   //敵の参照
    private float _timer;   //タイマー

    public IdleState(Enemy enemy) { _enemy = enemy; }   //コンストラクタでEnemyの参照を受け取る

    public void Init()
    {
        _timer = 0.0f;
        Debug.Log("IdleState: Enter");
    }

    public void Update()
    {
        //タイマーを進める
        _timer += Time.deltaTime;

        //プレイヤーを発見したら追跡状態へ
        if (_timer > 20.0f)
        {
            //状態を変更する
            _enemy.ChangeState(new ChaseState(_enemy));
            Debug.Log("IdleState: Change to ChaseState");
        }


        Debug.Log("IdleState: Update");
    }

    public void End()
    {
        Debug.Log("IdleState: Exit");
    }

}

//using UnityEngine;

//public class IdleState : EnemyState
//{
//    public IdleState(Enemy enemy) : base(enemy) { }

//    public override void Init()
//    {
//        Debug.Log("Idle: 待機開始");
//    }

//    public override void Update()
//    {
//        float distance = Vector3.Distance(_enemy.transform.position, _enemy.player.position);
//        if (distance < _enemy.chaseRange)
//        {
//            _enemy.ChangeState(new ChaseState(_enemy));
//        }
//    }

//    public override void End()
//    {
//        Debug.Log("Idle: 終了");
//    }
//}

