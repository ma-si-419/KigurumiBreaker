//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class DeadState : IState
//{
//    private EnemyBase _enemy;   //敵の参照
//    private float _timer;   //タイマー

//    public DeadState(EnemyBase enemy) { _enemy = enemy; }   //コンストラクタでEnemyの参照を受け取る

//    public void Init()
//    {
//        _timer = 0.0f;
//        Debug.Log("AttackState: Enter");
//    }

//    public void Update()
//    {
//        //タイマーを進める
//        _timer += Time.deltaTime;

//        //死んだ処理書いとけ


//        Debug.Log("AttackState: Update");
//    }

//    public void End()
//    {
//        Debug.Log("AttackState: Exit");
//    }
//}
