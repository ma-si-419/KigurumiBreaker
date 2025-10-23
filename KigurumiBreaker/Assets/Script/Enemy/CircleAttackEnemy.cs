using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAttackEnemy : Enemy
{


    public override void Attack()
    {

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

        //敵のアニメーション状態を取得
        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 0.8f)
        {
            //攻撃フラグをリセット
            _isCreateAttack = false;

            //攻撃アニメーションが終了したらIdleStateに遷移
            ChangeState(new IdleState(this));
        }

    }

    // 攻撃オブジェクトを生成する関数
    private void CreateAttack()
    {
        // ゲームオブジェクト生成
        GameObject attackObject = Instantiate(attackObjectPrefab);

        attackObject.GetComponent<EnemyAttackCol>().SetBattleManager(_battleManager);

        // 攻撃オブジェクトの位置を調整
        attackObject.transform.position = this.transform.position;
    }

}
