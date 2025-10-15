using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchEnemy : Enemy
{

    private GameObject _attackObject; // 攻撃オブジェクト


    [SerializeField] private float _dashSpeed; // 前進速度
    [SerializeField] private float _dashTime; // 前進時間
    [SerializeField] private float _attackDistance; // 攻撃判定の距離

    private bool _isDash = false; // 前進したかどうかのフラグ

    /* 定数 */
    private const float ATTACK_DISTANCE = 1.0f; // 攻撃判定の距離


    protected override void Start()
    {
        // 初期化処理
        base.Start();
        StopMovement();

    }

    public override void Attack()
    {
        if (_attackObject != null)
        {
            // 破棄されていない場合のみ位置を更新
            _attackObject.transform.position = this.transform.position + this.transform.forward * ATTACK_DISTANCE;
        }

        // 前進動作
        if (!_isDash)
        {
            StartCoroutine(DoDash());
        }
        else
        {
            StopMovement();
        }

        //アニメーションイベントで攻撃判定オブジェクトを生成したい
        base.Attack();
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 0.6f)
        {
            //攻撃判定を一つ生成させる
            if (!_isCreateAttack)
            {
                _isCreateAttack = true;
                CreateAttack();
            }
        }

        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 0.9f)
        {
            //攻撃フラグをリセット
            _isCreateAttack = false;
            _isDash = false;
            _isStateChange = true;
        }

        if (_isStateChange)
        {
            _isStateChange = false;
            ChangeState(new IdleState(this));
        }
        

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

    }

    // 攻撃オブジェクトを生成する関数
    private void CreateAttack()
    {
        //ゲームオブジェクト生成
       GameObject attackObject = Instantiate(_attackObjectPrefab);

       // ゲームオブジェクト生成
       //_attackObject = Instantiate(_attackObjectPrefab);

        float yOffset = 1.0f; // Y軸のオフセット値（必要に応じて調整）

        // 攻撃オブジェクトの位置を調整(Y軸を調整したい)
        attackObject.transform.position = this.transform.position + this.transform.forward * _attackDistance + Vector3.up * yOffset;



    }

}


