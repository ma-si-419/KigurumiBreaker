using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player<T> : MonoBehaviour where T : Player<T>
{
    // 移動入力の閾値
    protected const float MOVE_INPUT_LENGTH = 0.1f;
    // 回避の時間
    protected const int DODGE_TIME = 40;
    // 溜め攻撃を行えるようになる時間
    protected const int CHARGE_ATTACK_TIME = 35;


    //ひとつ前の状態
    private StateBase<T> _currentState;

    //次の状態
    private StateBase<T> _nextState;

    /// <summary>
    /// 状態を変更する関数
    /// </summary>
    /// <param name="next"></param>
    /// <returns></returns>
    public bool ChangeState(StateBase<T> next)
    {
        if (next == null) return false;

        _nextState = next;
        return true;
    }

    //更新
    private void FixedUpdate()
    {

        //次の状態が設定いるとき
        if (_nextState != null)
        {
            //現在の状態があれば終了処理
            if (_currentState != null)
            {
                _currentState.OnExitState();
            }

            //現在の状態をnullにする
            _currentState = null;

            //現在の状態を次の状態に設定
            _currentState = _nextState;
            //次の状態をnullにする
            _nextState = null;

            //開始処理を行う
            _currentState.OnEnterState();
        }

        // 現在のStateをDebugLogで表示
        Debug.Log("Current State: " + (_currentState != null ? _currentState.GetType().Name : "None"));


        //現在の状態があれば更新処理
        if (_currentState != null)
        {
            //更新処理を行う
            _currentState.OnUpdate();
        }
    }
}
