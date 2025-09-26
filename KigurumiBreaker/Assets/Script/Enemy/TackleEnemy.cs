using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TackleEnemy : Enemy
{
    [Header("タックル敵変数")]
    [SerializeField] private float _chargeSpeed = 5f; // 突進速度
    [SerializeField] private float _chargeTime = 0.3f; // 突進時間

    private bool _isCharge = false; // 突進中かどうかのフラグ
    private float _tackleTime = 0.0f; // タイマー

    public override void Attack()
    {
        // ここにタックル攻撃の具体的な処理を追加
        Debug.Log("タックルアタック！");
        _tackleTime += Time.deltaTime;

        if(_tackleTime > 2.0f)
        {
            attackHitBox.SetActive(true); // 攻撃判定を有効化
            // 突進中でなければ突進を開始
            if (!_isCharge) StartCoroutine(DoCharge());
        }
        //base.Attack();
    }

    private IEnumerator DoCharge()
    {
        _isCharge = true;

        // プレイヤーの方向を計算
        Vector3 dir = (playerTrans.position - transform.position).normalized;
        float timer = 0f;

        while (timer < _chargeTime && _isCharge)
        {
            // Rigidbodyを使って突進
            transform.position += dir * _chargeSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        _isCharge = false;
        _tackleTime = 0.0f;

        // 突進終了後、待機状態へ戻る
        attackHitBox.SetActive(false); // 攻撃判定を無効化
        ChangeState(new IdleState(this));

    }

}
