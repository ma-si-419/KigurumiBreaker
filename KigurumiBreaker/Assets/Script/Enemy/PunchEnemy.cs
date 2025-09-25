using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchEnemy : Enemy
{
    private float _punchTimer = 0.0f;   // タイマー
    private bool _isAttacking = false;  // 攻撃中かどうかのフラグ
    private bool _isDash = false;       // ダッシュ中かどうかのフラグ

    [SerializeField] private float _attackDuration = 1.0f;  // 攻撃全体の長さ
    [SerializeField] private float _hitTime = 0.5f;         // 攻撃がヒットするタイミング
    [SerializeField] private float _attackRange = 1.5f;     // 攻撃の有効範囲
    [SerializeField] private float _dashTime  = 0.25f;       // 攻撃のダメージ量
    [SerializeField] private float _dashDistance = 1.0f;       // ダッシュ距離
    [SerializeField] private float _dashSpeed = 10f;        // ダッシュの速さ

    private Rigidbody _rigidbody;

    public override void Attack()
    {
        //base.Attack();

        // タイマーを更新



        //攻撃判定を行う
        transform.LookAt(player.transform); //プレイヤーの方向を向く





        //攻撃を行い終えたら待機状態へ戻る
        Debug.Log("PunchEnemy: Performing punch attack!");
    }




}
