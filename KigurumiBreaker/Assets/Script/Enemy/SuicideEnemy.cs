using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideEnemy : Enemy
{
    // 攻撃に関する変数
    private float _suicideTimer = 0.0f;   // タイマー


    public override void Attack()
    {
        _suicideTimer += Time.deltaTime;

        // 1秒間攻撃状態を維持
        if (_suicideTimer > 5.0f)
        {
            //攻撃の表示非表示
            StartCoroutine(DoExplosion());
        }


    }

    private IEnumerator DoExplosion()
    {
        //攻撃を行い終えたら待機状態へ戻る
        Debug.Log("爆破!!");

        attackHitBox.SetActive(true); // 攻撃判定を有効化
        yield return new WaitForSeconds(0.9f); // 攻撃のタイミングを調整
        attackHitBox.SetActive(false); // 攻撃判定を無効化
        _suicideTimer = 0.0f;
        ChangeState(new IdleState(this));
    }

}
