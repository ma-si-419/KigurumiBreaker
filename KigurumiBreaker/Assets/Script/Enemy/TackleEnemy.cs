using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TackleEnemy : Enemy
{
    [Header("タックル敵変数")]
    [SerializeField] private float _chargeSpeed = 5f; // 突進速度
    [SerializeField] private float _chargeTime = 0.3f; // 突進時間
    [SerializeField] private float _attackDistance; // 攻撃判定の距離

    private bool _isCharge = false; // 突進中かどうかのフラグ
    private float _tackleTime = 0.0f; // タイマー
    private GameObject _attackObject; // 攻撃オブジェクト

    /* 定数 */
    private const float TACKLE_COUNTDOWN = 1.0f; // タックル攻撃のクールダウン時間

    public override void Attack()
    {
        // ここにタックル攻撃の具体的な処理を追加
        _tackleTime += Time.deltaTime;
        base.Attack();

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
                _attackObject.transform.position = this.transform.position + this.transform.forward * _attackDistance;
            }

            // 突進中でなければ突進を開始
            if (!_isCharge) StartCoroutine(DoCharge());
        }

    }

    private IEnumerator DoCharge()
    {
        _isCharge = true;
        float timer = 0f;

        while (timer < _chargeTime && _isCharge)
        {
            // 前進方向を計算
            Vector3 dir = (transform.forward).normalized;

            transform.position += dir * _chargeSpeed * Time.deltaTime;
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
