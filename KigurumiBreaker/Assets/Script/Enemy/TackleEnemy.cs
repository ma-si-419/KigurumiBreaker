using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TackleEnemy : Enemy
{
    [Header("タックル敵変数")]
    private float CHARGE_SPEED = 0.2f; // 突進速度
    private float CHARGE_TIME = 0.1f; // 突進時間

    private bool _isCharge = false; // 突進中かどうかのフラグ
    private float _tackleTime = 0.0f; // タイマー
    private GameObject _attackObject; // 攻撃オブジェクト

    /* 定数 */
    private const float TACKLE_COUNTDOWN = 1.0f; // タックル攻撃のクールダウン時間
    private const float ATTACK_DISTANCE = 1.0f; // 攻撃判定の距離


    public override void Attack()
    {
        // NavMeshの追跡
        agent.enabled = false;
        //// ここにタックル攻撃の具体的な処理を追加
        //_tackleTime += Time.deltaTime;

        //if (_tackleTime > TACKLE_COUNTDOWN)
        //{
        //    //攻撃判定を一つ生成させる
        //    if (!_isCreateAttack)
        //    {
        //        _isCreateAttack = true;
        //        CreateAttack();
        //    }

        //    if (_attackObject != null)
        //    {
        //        // 破棄されていない場合のみ位置を更新
        //        _attackObject.transform.position = this.transform.position + this.transform.forward * ATTACK_DISTANCE;
        //    }

        //    // 突進中でなければ突進を開始
        //    if (!_isCharge) StartCoroutine(DoCharge());
        //}
        //else
        //{
        //    LookAtPlayer();
        //}

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //攻撃開始
        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 0.3f)
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
        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 0.6f)
        {
            StopMovement();
            agent.enabled = true;
            _isCreateAttack = false;
            ChangeState(new IdleState(this));
        }

    }

    private IEnumerator DoCharge()
    {
        //_isCharge = true;
        //float timer = 0f;

        //while (timer < CHARGE_TIME && _isCharge)
        //{
        //    // 前進方向を計算
        //    Vector3 dir = (transform.forward).normalized;

        //    transform.position += dir * CHARGE_SPEED * Time.deltaTime;
        //    timer += Time.deltaTime;
        //    yield return null;
        //}

        //_isCharge = false;
        //_tackleTime = 0.0f;

        ////攻撃フラグをリセット
        //_isCreateAttack = false;

        //ChangeState(new IdleState(this));

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

        _rigidbody.velocity = Vector3.zero; // 終了時に停止を保証
        _isCharge = false;
        _tackleTime = 0.0f;
    }

    // 攻撃オブジェクトを生成する関数
    private void CreateAttack()
    {
        // ゲームオブジェクト生成
        _attackObject = Instantiate(attackObjectPrefab);
        _attackObject.GetComponent<EnemyAttackCol>().SetBattleManager(_battleManager);
    }

}
