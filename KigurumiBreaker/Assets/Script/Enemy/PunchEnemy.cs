using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchEnemy : Enemy
{
    //private float _punchTimer = 0.0f;   // タイマー
    //private bool _isAttacking = false;  // 攻撃中かどうかのフラグ
    //private bool _isDash = false;       // ダッシュ中かどうかのフラグ

    private Rigidbody _rigidbody;   // Rigidbody

    public override void Attack()
    {
        //base.Attack();
        

        // タイマーを更新
        //_punchTimer += Time.deltaTime;
        //Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, 1f, LayerMask.GetMask("Player"));

           
        //攻撃判定を行う

        transform.LookAt(player.transform); //プレイヤーの方向を向く





        //攻撃を行い終えたら待機状態へ戻る
        Debug.Log("PunchEnemy: Performing punch attack!");
    }




}
