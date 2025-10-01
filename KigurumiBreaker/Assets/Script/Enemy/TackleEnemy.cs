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

    public override void Attack()
    {
        // ここにタックル攻撃の具体的な処理を追加
        Debug.Log("タックルアタック！");
        _tackleTime += Time.deltaTime;

        if(_tackleTime > 1.0f)
        {
            _attackObjectPrefab.SetActive(true); // 攻撃判定を有効化

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

            // Rigidbodyを使って突進
            transform.position += dir * _chargeSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        _isCharge = false;
        _tackleTime = 0.0f;

        // 突進終了後、待機状態へ戻る
        _attackObjectPrefab.SetActive(false); // 攻撃判定を無効化
        ChangeState(new IdleState(this));

    }

    // 攻撃オブジェクトを生成する関数
    private void CreateAttack()
    {
        // ゲームオブジェクト生成
        GameObject attackObject = Instantiate(_attackObjectPrefab);

        // 球の当たり判定設定
        //attackObject.GetComponent<SphereCollider>().radius = _attackRadius;

        // 攻撃オブジェクトの位置を調整
        attackObject.transform.position = this.transform.position + this.transform.forward * _attackDistance;

        //攻撃フラグをリセット
        _isCreateAttack = false;
        ChangeState(new IdleState(this));
    }

}
