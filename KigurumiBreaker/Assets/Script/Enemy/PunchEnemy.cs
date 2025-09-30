using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchEnemy : Enemy
{
    // 攻撃に関する変数
    private float _punchTimer = 0.0f;   // タイマー

    [SerializeField] private float _dashSpeed; // 前進速度
    [SerializeField] private float _dashTime; // 前進時間

    private bool _isDash = false; // 前進したかどうかのフラグ


    public override void Attack()
    {
        _punchTimer += Time.deltaTime;
        //アニメーションイベントで攻撃判定オブジェクトを生成したい

        

        StartCoroutine(DoAttack());
    }

    private IEnumerator DoAttack()
    {
        //攻撃を行い終えたら待機状態へ戻る
        Debug.Log("パンチ");

        // 前進動作
        if(!_isDash) StartCoroutine(DoDash());

        //アニメーションイベントで攻撃判定オブジェクトを生成したい(将来的に)

        _attackHitBox.SetActive(true);          // 攻撃判定を有効化
        yield return new WaitForSeconds(0.3f); // 攻撃のタイミングを調整
        _attackHitBox.SetActive(false);        // 攻撃判定を無効化
        _punchTimer = 0.0f;
        ChangeState(new IdleState(this));
    }

    private IEnumerator DoDash()
    {
        _isDash = true;
        float timer = 0f;

        while (timer < _dashTime && _isDash)
        {
            // 前進方向を計算
            Vector3 dir = (transform.forward).normalized;

            // Rigidbodyを使って突進
            transform.position += dir * _dashSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        _isDash = false;
        _punchTimer = 0.0f;
    }

}


