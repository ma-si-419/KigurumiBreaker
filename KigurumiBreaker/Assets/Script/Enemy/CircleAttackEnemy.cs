using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAttackEnemy : Enemy
{
    // 攻撃に関する変数
    private float _circleAttackTimer = 0.0f;   // タイマー

    public override void Attack()
    {
        _circleAttackTimer += Time.deltaTime;
        //アニメーションイベントで攻撃判定オブジェクトを生成したい

        //攻撃判定を一つ生成させる
        if (!_isCreateAttack)
        {
            _isCreateAttack = true;
            CreateAttack();
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
        ChangeState(new IdleState(this));
    }

}
