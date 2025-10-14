using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TackleEnemy : Enemy
{
    [Header("タックル敵変数")]
    [SerializeField] private float CHARGE_SPEED = 5f; // 突進速度
    [SerializeField] private float CHARGE_TIME = 0.0f; // 突進時間

    private bool _isCharge = false; // 突進中かどうかのフラグ
    private float _tackleTime = 0.0f; // タイマー
    private GameObject _attackObject; // 攻撃オブジェクト

    /* 定数 */
    private const float TACKLE_COUNTDOWN = 1.0f; // タックル攻撃のクールダウン時間
    private const float ATTACK_DISTANCE = 1.0f; // 攻撃判定の距離


    public override void Attack()
    {
        // ここにタックル攻撃の具体的な処理を追加
        _tackleTime += Time.deltaTime;

        if (_tackleTime > TACKLE_COUNTDOWN)
        {
            //攻撃判定を一つ生成させる
            if (!_isCreateAttack)
            {
                _isCreateAttack = true;
                CreateAttack();
            }

            if (_attackObject != null)
            {
                // 破棄されていない場合のみ位置を更新
                _attackObject.transform.position = this.transform.position + this.transform.forward * ATTACK_DISTANCE;
            }

            // 突進中でなければ突進を開始
            if (!_isCharge) StartCoroutine(DoCharge());
        }
        else
        {
            LookAtPlayer();
        }

    }

    private IEnumerator DoCharge()
    {
        _isCharge = true;
        float timer = 0f;

        while (timer < CHARGE_TIME && _isCharge)
        {
            // 前進方向を計算
            Vector3 dir = (transform.forward).normalized;

            transform.position += dir * CHARGE_SPEED * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        _isCharge = false;
        _tackleTime = 0.0f;

        //攻撃フラグをリセット
        _isCreateAttack = false;

        ChangeState(new IdleState(this));
    }

    // 攻撃オブジェクトを生成する関数
    private void CreateAttack()
    {
        // ゲームオブジェクト生成
        _attackObject = Instantiate(_attackObjectPrefab);
    }

}
