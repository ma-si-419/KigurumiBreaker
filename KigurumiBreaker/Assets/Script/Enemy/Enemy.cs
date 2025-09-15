using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private IState _currentState;

    //public Transform Player;
    public float Speed = 3f;

    private void Init()
    {
        ChangeState(new IdleState(this));
    }

    private void Update()
    {
        _currentState?.Update();
    }

    public void ChangeState(IState newState)
    {
        _currentState?.End();   // 現在のステートを抜ける
        _currentState = newState;
        _currentState.Init();   // 新しいステートに入る
    }

    public void Attack()
    {
        Debug.Log("敵が攻撃した！");
        // ここに攻撃処理を書く
    }
}


//using UnityEngine;

//public class Enemy : MonoBehaviour
//{
//    private EnemyState currentState;

//    [Header("基本情報")]
//    public Transform player;
//    public float chaseRange = 5f;
//    public float attackRange = 2f;
//    public int hp = 10;

//    [Header("攻撃タイプ")]
//    public EnemyAttackBase attackBehavior;

//    void Start()
//    {
//        // 初期ステートはIdle
//        ChangeState(new IdleState(this));
//    }

//    void Update()
//    {
//        currentState?.Update();
//    }

//    public void ChangeState(EnemyState newState)
//    {
//        currentState?.Exit();
//        currentState = newState;
//        currentState?.Enter();
//    }

//    public void TakeDamage(int damage)
//    {
//        hp -= damage;
//        if (hp <= 0)
//        {
//            ChangeState(new DeadState(this));
//        }
//    }
//}
