using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchEnemy : Enemy
{
    // 攻撃に関する変数
    private float _punchTimer = 0.0f;   // タイマー

    [SerializeField] private float _dashSpeed; // 前進速度
    [SerializeField] private float _dashTime; // 前進時間
    [SerializeField] private float _attackDistance; // 攻撃判定の距離

    private bool _isDash = false; // 前進したかどうかのフラグ

    public override void Attack()
    {
        _punchTimer += Time.deltaTime;
        //アニメーションイベントで攻撃判定オブジェクトを生成したい

        base.Attack();

        // 前進動作
        if (!_isDash) StartCoroutine(DoDash());

        //アニメーションイベントで攻撃判定オブジェクトを生成したい(将来的に)


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

        //攻撃判定を一つ生成させる
        if (!_isCreateAttack)
        {
            _isCreateAttack = true;
            CreateAttack();
        }
    }

    // 攻撃オブジェクトを生成する関数
    private void CreateAttack()
    {
        // ゲームオブジェクト生成
        GameObject attackObject = Instantiate(_attackObjectPrefab);

        // 攻撃オブジェクトの位置を調整
        attackObject.transform.position = this.transform.position + this.transform.forward * _attackDistance;

        //攻撃フラグをリセット
        _isCreateAttack = false;
        ChangeState(new IdleState(this));
    }

}


