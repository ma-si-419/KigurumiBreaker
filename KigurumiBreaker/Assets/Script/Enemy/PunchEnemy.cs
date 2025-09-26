using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchEnemy : Enemy
{
    // 攻撃に関する変数
    private float _punchTimer = 0.0f;   // タイマー
    //private bool _isAttacking = false;  // 攻撃中かどうかのフラグ
    //private bool _isDash = false;       // ダッシュ中かどうかのフラグ
    //private bool _isAttackHit = false;

    //[SerializeField] private GameObject _attackPrefab; // 攻撃判定のプレハブ
    //private float _attackDistance = 1.5f; // 攻撃判定の生成距離


    public override void Attack()
    {
        _punchTimer += Time.deltaTime;

        //Vector3 spawnPos = transform.position + transform.forward * _attackDistance;
        //攻撃判定を生成
        //GameObject attack = Instantiate(_attackPrefab, _attackPoint.position, _attackPoint.rotation);

        //Destroy(attack, 0.1f);  // 0.3秒後に攻撃判定を削除

        //アニメーションイベントで攻撃判定オブジェクトを生成したい




        // 1秒間攻撃状態を維持
        if (_punchTimer > 2.0f)
        {
            //攻撃の表示非表示
            StartCoroutine(DoAttack());
        }

       
    }

    private IEnumerator DoAttack()
    {
        //攻撃を行い終えたら待機状態へ戻る
        Debug.Log("パンチ");

        attackHitBox.SetActive(true); // 攻撃判定を有効化
        yield return new WaitForSeconds(0.3f); // 攻撃のタイミングを調整
        attackHitBox.SetActive(false); // 攻撃判定を無効化
        _punchTimer = 0.0f;
        ChangeState(new IdleState(this));
    }


}


