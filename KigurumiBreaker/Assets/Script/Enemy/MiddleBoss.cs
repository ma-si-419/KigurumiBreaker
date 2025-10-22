using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleBoss : BossEnemy
{
    //攻撃オブジェクトを一度だけ生成するフラグ
    private bool _isCreateAttack = false;
    //攻撃クールダウンタイマー
    //private float _cooldownTimer = 0.0f;

    private float CHARGE_SPEED = 10f; // 突進速度
    private float CHARGE_TIME = 1.0f; // 突進時間

    private bool _isCharge = false; // 突進中かどうかのフラグ
    private float _tackleTime = 0.0f; // タイマー
    private GameObject _attackObject; // 攻撃オブジェクト

    /* 定数 */
    private const float TACKLE_COUNTDOWN = 1.0f; // タックル攻撃のクールダウン時間
    private const float ATTACK_DISTANCE = 1.0f; // 攻撃判定の距離

    public override void MeleeAttack()
    {
        // 中ボス専用の近接攻撃処理をここに追加
        Debug.Log("中ボスの通常攻撃！");

        _stateTimer += Time.deltaTime;
        //一旦
        if (_stateTimer >= 1)
        {
            _stateTimer = 0.0f;
            ChangeState(new BossIdleState(this));
        }

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("MeleeAttack") && stateInfo.normalizedTime >= 0.6f)
        {
            //攻撃判定を一つ生成させる
            if (!_isCreateAttack)
            {
                Debug.Log("攻撃!");
                _isCreateAttack = true;
                CreateMeleeAttack();
            }
        }

        //敵のアニメーション状態を取得
        if (stateInfo.IsName("MeleeAttack") && stateInfo.normalizedTime >= 0.8f)
        {
            //攻撃フラグをリセット
            _isCreateAttack = false;
            isAttack = false;
            //攻撃アニメーションが終了したらIdleStateに遷移
            ChangeState(new BossIdleState(this));
        }
    }

    public override void Attack()
    {
        Debug.Log("中ボスのタックル攻撃！");

        // ここにタックル攻撃の具体的な処理を追加
        _tackleTime += Time.deltaTime;

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //攻撃開始
        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 0.5f)
        {
            if(!_isCreateAttack)
            {
                _isCreateAttack = true;
                CreateAttack();
            }

            //攻撃判定を一つ生成させる
            if (!_isCreateAttack)
            {
                Debug.Log("攻撃!");
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

        //敵のアニメーションが終わったらIdleStateに遷移
        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 0.8f)
        {
            ChangeState(new BossIdleState(this));
        }




    }

    private IEnumerator DoCharge()
    {
        _isCharge = true;
        float timer = 0f;
        // 前進方向を計算
        Vector3 dir = (transform.forward).normalized;

        Debug.Log("うおおおぉぉぉ");

        while (timer < CHARGE_TIME && _isCharge)
        {

            _rigidbody.velocity = dir * CHARGE_SPEED;
            // 前進方向を計算
            //transform.position += dir * CHARGE_SPEED * Time.deltaTime;

            timer += Time.deltaTime;
            yield return null;
        }

        //if (_rigidbody != null)
        //{
        //    _rigidbody.velocity = Vector3.zero;
        //}

        _isCharge = false;
        _tackleTime = 0.0f;
        //攻撃フラグをリセット
        _isCreateAttack = false;
        isAttack = false;
    }

    // 攻撃オブジェクトを生成する関数
    private void CreateMeleeAttack()
    {
        //ゲームオブジェクト生成
       //GameObject attackObject = Instantiate(meleeAttackPrefab);

       // //攻撃オブジェクトの位置を調整
       // attackObject.transform.position = this.transform.position;
    }

    private void CreateAttack()
    {
        // ゲームオブジェクト生成
        //_attackObject = Instantiate(attackObjectPrefab);
    }


}
