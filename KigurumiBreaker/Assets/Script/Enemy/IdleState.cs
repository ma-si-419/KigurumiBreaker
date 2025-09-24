using Unity.VisualScripting;
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

        Vector3 diff = _enemy.playerTrans.position - _enemy.transform.position; //プレイヤーとの位置差を計算
        //Debug.Log(diff);

        //プレイヤーが検知範囲内にいるかチェック
        if (diff.sqrMagnitude < _enemy.detectRangeSqr && _timer > 3.0f)
        {
            Debug.Log("IdleState: Change to ChaseState");
            _enemy.ChangeState(new ChaseState(_enemy));
        }

    }

    public void End()
    {
        Debug.Log("IdleState: End");
    }
}

