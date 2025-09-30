using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideEnemy : Enemy
{
    // 攻撃に関する変数
    private float _suicideTimer = 0.0f;   // タイマー

    [Header("自爆敵の変数")]
    [SerializeField] private float _dashSpeed; // 突進速度
    [SerializeField] private float _dashTime; // 突進時間

    private bool _isDash = false; // 突進中かどうかのフラグ

    public override void Move()
    {

        //プレイヤーに当たらなかった場合の処理
        //一定時間経過で自爆
        _suicideTimer += Time.deltaTime;

        if (_suicideTimer > 3.0f)
        {
            //敵の速度を0にする
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;

            //自爆
            ChangeState(new AttackState(this));
        }


        //移動処理
        if (!_isDash) StartCoroutine(DoDash());

        //プレイヤーとの位置差を計算
        Vector3 diff = _player.transform.position - transform.position;

        //攻撃圏内に入ると攻撃状態へ
        //プレイヤーが検知範囲内にいるかチェック
        if (diff.sqrMagnitude < _attackRangeSqr)
        {
            //敵の速度を0にする
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;

            //状態を変更する
            ChangeState(new AttackState(this));
        }
    }

    public override void Attack()
    {
        _suicideTimer += Time.deltaTime;

        //攻撃の表示非表示
        StartCoroutine(DoExplosion());
    }

    private IEnumerator DoExplosion()
    {
        //攻撃を行い終えたら待機状態へ戻る
        Debug.Log("爆破!!");

        _attackHitBox.SetActive(true); // 攻撃判定を有効化
        yield return new WaitForSeconds(0.9f); // 攻撃のタイミングを調整
        _attackHitBox.SetActive(false); // 攻撃判定を無効化
        _suicideTimer = 0.0f;
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
    }

}
