//using UnityEngine;
//using UnityEngine.XR;

//public class IdleState : IState
//{
//    private EnemyBase _enemy;   //敵の参照
//    private float _timer;   //タイマー

//    public IdleState(EnemyBase enemy) { _enemy = enemy; }   //コンストラクタでEnemyの参照を受け取る

//    public void Init()
//    {
//        _timer = 0.0f;
//        Debug.Log("IdleState: Enter");
//    }

//    public void Update()
//    {
//        //タイマーを進める
//        _timer += Time.deltaTime;

//        //プレイヤーを発見したら追跡状態へ
//        if (_timer > 20.0f)
//        {
//            //状態を変更する
//            _enemy.ChangeState(new ChaseState(_enemy));
//            Debug.Log("IdleState: Change to ChaseState");
//        }


//        Debug.Log("IdleState: Update");
//    }

//    public void End()
//    {
//        Debug.Log("IdleState: Exit");
//    }

//}
