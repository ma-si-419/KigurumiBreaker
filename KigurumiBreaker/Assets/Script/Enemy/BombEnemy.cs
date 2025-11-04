using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEnemy : Enemy
{
    public override void Attack()
    {
        
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 0.7f)
        {
            //攻撃判定を一つ生成させる
            if (!_isCreateAttack)
            {
                _isCreateAttack = true;
                CreateAttack();
            }

            _isStateChange = true;
            //攻撃フラグをリセット
            _isCreateAttack = false;
        }

        //状態遷移
        if (_isStateChange)
        {
            _isStateChange = false;
            _currentHp = 0; // 自分の体力を0にする
            _currentTrunk = 0; // 自分の耐久力を0にする
            ChangeState(new DeadState(this));
        }

    }

    // 攻撃オブジェクトを生成する関数
    private void CreateAttack()
    {
        // ゲームオブジェクト生成
        GameObject attackObject = Instantiate(_attackObjectPrefab);

        attackObject.GetComponent<EnemyAttackCol>().SetBattleManager(_battleManager);

        // 攻撃オブジェクトの位置を調整
        attackObject.transform.position = this.transform.position;
    }

}
