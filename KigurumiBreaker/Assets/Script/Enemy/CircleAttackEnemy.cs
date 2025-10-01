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

        StartCoroutine(DoAttack());
    }

    private IEnumerator DoAttack()
    {
        //攻撃を行い終えたら待機状態へ戻る
        Debug.Log("パンチ");

        //アニメーションイベントで攻撃判定オブジェクトを生成したい(将来的に)
        _attackHitBox.SetActive(true);          // 攻撃判定を有効化
        yield return new WaitForSeconds(0.3f); // 攻撃のタイミングを調整
        _attackHitBox.SetActive(false);        // 攻撃判定を無効化
        _circleAttackTimer = 0.0f;
        ChangeState(new IdleState(this));
    }

}
