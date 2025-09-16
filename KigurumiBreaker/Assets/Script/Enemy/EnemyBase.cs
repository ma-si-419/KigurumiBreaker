//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// ステートパターン
///// </summary>
//public interface IState
//{
//    //状態に入るときに呼び出す関数
//    void Init();
//    //毎フレーム呼び出す関数
//    void Update();
//    //終了するときに呼び出す関数
//    void End();
//}

//public class StateMachine
//{
//    private IState _currentState;   //現在の状態

//    //状態を変更する関数
//    public void ChangeState(IState newState)
//    {
//        //新しい状態に変更し、初期化処理を呼び出す
//        _currentState?.End();
//        _currentState = newState;
//        _currentState?.Init();
//    }
//    //毎フレーム呼び出す関数
//    public void Update()
//    {
//        _currentState?.Update();
//    }
//}

///// <summary>
///// ストラテジーパターン
///// </summary>

//public interface IAttack
//{
//    //実行する関数
//    void Execute(EnemyBase enemy);
//}

////近接攻撃関数
//public class MeleeAttack : IAttack
//{
//    //実行する関数
//    public void Execute(EnemyBase enemy)
//    {
//        Debug.Log("近接攻撃!");
//    }
//}

////遠距離攻撃関数
//public class RangedAttack : IAttack
//{
//    //実行する関数
//    public void Execute(EnemyBase enemy)
//    {
//        Debug.Log("遠距離攻撃!");
//    }
//}

////特殊攻撃関数
//public class SpecialAttack : IAttack
//{
//    //実行する関数
//    public void Execute(EnemyBase enemy)
//    {
//        Debug.Log("特殊攻撃!");
//    }
//}

///// <summary>
///// 敵の基底クラス
///// </summary>
//public abstract class EnemyBase : MonoBehaviour
//{
//    protected StateMachine _stateMachine;   //状態マシン
//    public Transform target;                //ターゲット(プレイヤー)
//    public float speed = 2.0f;              //移動速度

//    protected List<IAttack> _attacks = new List<IAttack>();   //攻撃方法のリスト

//    //初期化
//    protected virtual void Init()
//    {
//        _stateMachine = new StateMachine();     //状態マシンの初期化
//        //_stateMachine.ChangeState(new IdleState());  //初期状態を待機状態に設定
//    }

//    //更新処理
//    protected virtual void Update()
//    {
//        _stateMachine.Update();  //状態マシンの更新
//    }

//    //攻撃
//    public virtual void Attack(int index)
//    {
//        if(index >= 0 && index < _attacks.Count)
//        {
//            _attacks[index].Execute(this);  //指定された攻撃方法を実行
//        }
//    }


//}

///// <summary>
///// ステートパターン
///// </summary>

//public class IdleState : IState
//{
//    private EnemyBase _enemy;   //敵の参照
//    public IdleState(EnemyBase enemy) { _enemy = enemy; }   //コンストラクタでEnemyの参照を受け取る

//    //初期関数
//    public void Init()
//    {
//        Debug.Log("IdleState: Init");
//    }

//    //毎フレーム呼び出す関数
//    public void Update()
//    {
//        Debug.Log("IdleState: Update");
//    }

//    //エンド関数
//    public void End()
//    {
//        Debug.Log("IdleState: End");
//    }

//}

//public class ChaseState : IState
//{
//    private EnemyBase _enemy;   //敵の参照

//    //初期関数
//    public void Init()
//    {
//        Debug.Log("ChaseState: Init");
//    }

//    //毎フレーム呼び出す関数
//    public void Update()
//    {
//        Debug.Log("ChaseState: Update");
//    }

//    //エンド関数
//    public void End()
//    {
//        Debug.Log("ChaseState: End");
//    }

//}

//public class AttackState : IState
//{
//    private EnemyBase _enemy;   //敵の参照

//    //初期関数
//    public void Init()
//    {
//        Debug.Log("AttackState: Init");
//    }
//    //毎フレーム呼び出す関数
//    public void Update()
//    {
//        Debug.Log("AttackState: Update");
//    }
//    //エンド関数
//    public void End()
//    {
//        Debug.Log("AttackState: End");
//    }
//}

//public class DeadState : IState
//{
//    private EnemyBase _enemy;   //敵の参照

//    //初期関数
//    public void Init()
//    {
//        Debug.Log("DeadState: Init");
//    }
//    //毎フレーム呼び出す関数
//    public void Update()
//    {
//        Debug.Log("DeadState: Update");
//    }
//    //エンド関数
//    public void End()
//    {
//        Debug.Log("DeadState: End");
//    }
//}

///// <summary>
///// 敵の種類
///// </summary>

//public class  NormalEnemy : EnemyBase
//{
//    protected override void Init()
//    {
//        //初期化
//        base.Init();
//        //攻撃方法の決定
//        _attacks.Add(new MeleeAttack());
//    }
//}