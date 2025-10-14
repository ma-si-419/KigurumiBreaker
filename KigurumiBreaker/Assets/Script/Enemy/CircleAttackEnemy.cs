using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAttackEnemy : Enemy
{

    // 攻撃に関する変数
    private float _circleAttackTimer = 0.0f;   // タイマー

    public override void Attack()
    {
        _circleAttackTimer += Time.deltaTime; //フレーム換算
        //アニメーションイベントで攻撃判定オブジェクトを生成したい

        Debug.Log(_circleAttackTimer);
        Debug.Log(_isCreateAttack);

        //攻撃判定を一つ生成させる
        if (_circleAttackTimer > ATTACK_CREATE_TIME)
        {
            if (!_isCreateAttack)
            {
                _isCreateAttack = true;
                CreateAttack();
            }
        }
    }

    // 攻撃オブジェクトを生成する関数
    private void CreateAttack()
    {
        // ゲームオブジェクト生成
        GameObject attackObject = Instantiate(_attackObjectPrefab);

        // 攻撃オブジェクトの位置を調整
        attackObject.transform.position = this.transform.position;

        //攻撃フラグをリセット
        _isCreateAttack = false;
        _circleAttackTimer = 0.0f;

        // 状態をIdleに変更
        ChangeState(new IdleState(this));
    }

}
