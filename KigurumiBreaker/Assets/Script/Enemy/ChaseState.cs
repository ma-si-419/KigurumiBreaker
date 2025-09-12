//using UnityEngine;

//public class ChaseState : IState
//{
//    private EnemyBase _enemy;   //敵の参照
//    private float _timer;   //タイマー

//    public ChaseState(EnemyBase enemy) { _enemy = enemy; }   //コンストラクタでEnemyの参照を受け取る

//    public void Init()
//    {
//        _timer = 0.0f;
//        Debug.Log("ChaseState: Enter");
//    }

//    public void Update()
//    {
//        //タイマーを進める
//        _timer += Time.deltaTime;

//        //攻撃圏内に入ると攻撃状態へ
//        if (_timer > 20.0f)
//        {
//            //状態を変更する
//            _enemy.ChangeState(new AttackState(_enemy));
//            Debug.Log("ChaseState: Change to AttackState");
//        }


//        Debug.Log("ChaseState: Update");
//    }

//    public void End()
//    {
//        Debug.Log("ChaseState: Exit");
//    }



//}
