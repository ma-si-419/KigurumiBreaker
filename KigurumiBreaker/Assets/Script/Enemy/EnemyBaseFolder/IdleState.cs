using Unity.VisualScripting;
using UnityEngine;

public class IdleState : IState
{
    private Enemy _enemy;   //敵の参照

    public IdleState(Enemy enemy) 
    {
        //コンストラクタでEnemyの参照を受け取る
        _enemy = enemy; 
    }

    public void Init()
    {
        Debug.Log("IdleState: Init");
    }

    public void Update()
    {
        _enemy.Idle(); //基本待機処理
    }

    public void End()
    {
        Debug.Log("IdleState: End");
    }
}

