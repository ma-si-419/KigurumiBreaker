using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideEnemy : Enemy
{

    private float _suicideTimer = 0.0f; // タイマー
    private const float SUICIDE_COUNTDOWN = 1.5f; // 自爆攻撃のカウントダウン

    public override void Attack()
    {
        _suicideTimer += Time.deltaTime;

        if(_suicideTimer > SUICIDE_COUNTDOWN)
        {

            //攻撃判定を一つ生成させる
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
        _suicideTimer = 0.0f;

        ChangeState(new DeadState(this));
    }

}
