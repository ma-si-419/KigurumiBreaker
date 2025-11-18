using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleBoss : BossEnemy
{
    //攻撃オブジェクトを一度だけ生成するフラグ
    private bool _isCreateAttack = false;

    private float CHARGE_SPEED = 0.6f; // 突進速度
    private float CHARGE_TIME = 0.2f; // 突進時間

    private bool _isCharge = false;    // 突進中かどうかのフラグ
    private float _chargeTimer = 0.0f; // タイマー
    private GameObject _attackObject;  // 攻撃オブジェクト

    /* 定数 */
    private const float TACKLE_COUNTDOWN = 1.0f; // タックル攻撃のクールダウン時間
    private const float ATTACK_DISTANCE = 3.0f; // 攻撃判定の距離

    public override void AttackType1()
    {
        // ここにタックル攻撃の具体的な処理を追加
        _chargeTimer += Time.deltaTime;

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //攻撃開始
        if (stateInfo.IsName("AttackType1") && stateInfo.normalizedTime >= 0.5f)
        {
            if (!_isCreateAttack)
            {
                _isCreateAttack = true;
                CreateAttack();
            }

            if (!_isCharge)
            {
                StartCoroutine(DoCharge());
            }
        }
        else
        {
            LookAtPlayer();
        }

        if (_attackObject != null)
        {
            // 破棄されていない場合のみ位置を更新
            _attackObject.transform.position = this.transform.position + this.transform.forward * ATTACK_DISTANCE;
        }

        //敵のアニメーションが終わったらIdleStateに遷移
        if (stateInfo.IsName("AttackType1") && stateInfo.normalizedTime >= 0.6f)
        {
            StopMovement();
            _isCreateAttack = false;
            ChangeState(new BossIdleState(this));
        }
    }

    public override void AttackType2()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("AttackType2") && stateInfo.normalizedTime >= 0.6f)
        {
            //攻撃判定を一つ生成させる
            if (!_isCreateAttack)
            {
                _isCreateAttack = true;
                CreateMeleeAttack();
            }
        }

        //敵のアニメーション状態を取得
        if (stateInfo.IsName("AttackType2") && stateInfo.normalizedTime >= 0.8f)
        {
            //攻撃フラグをリセット
            _isCreateAttack = false;
            //攻撃アニメーションが終了したらIdleStateに遷移
            ChangeState(new BossIdleState(this));
        }
    }

    public override void AttackType3()
    {
    }

    public override void AttackType4()
    {
    }

    private IEnumerator DoCharge()
    {
        _isCharge = true;
        float timer = 0f;
        Vector3 dir = transform.forward.normalized;

        while (timer < CHARGE_TIME && _isCharge)
        {
            _rigidbody.velocity = dir * CHARGE_SPEED;

            this.transform.position += _rigidbody.velocity;

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        //_rigidbody.velocity = Vector3.zero; // 終了時に停止を保証
        _isCharge = false;
        _chargeTimer = 0.0f;
    }

    // 攻撃オブジェクトを生成する関数
    private void CreateMeleeAttack()
    {
        //ゲームオブジェクト生成
        GameObject attackObject = Instantiate(_attackType2ObjectPrefab);
        //攻撃オブジェクトの位置を調整
        attackObject.transform.position = this.transform.position;
        attackObject.GetComponent<EnemyAttackCol>().SetBattleManager(_battleManager);
    }

    private void CreateAttack()
    {
        // ゲームオブジェクト生成
        _attackObject = Instantiate(_attackType1ObjectPrefab);
        _attackObject.GetComponent<EnemyAttackCol>().SetBattleManager(_battleManager);

    }


}
