using UnityEngine;

public class AttackState : IState
{
    private Enemy _enemy;   //敵の参照
    private float _timer;   //タイマー

    public AttackState(Enemy enemy) { _enemy = enemy; }   //コンストラクタでEnemyの参照を受け取る

    public void Init()
    {
        _timer = 0.0f;
        Debug.Log("AttackState: Enter");
    }

    public void Update()
    {
        //タイマーを進める
        _timer += Time.deltaTime;

        //プレイヤーを攻撃したら待機状態へ
        if (_timer > 20.0f)
        {
            //状態を変更する
            _enemy.ChangeState(new IdleState(_enemy));
            Debug.Log("AttackState: Change to IdleState");
        }

        Debug.Log("AttackState: Update");
    }

    public void End()
    {
        Debug.Log("AttackState: Exit");
    }

}
