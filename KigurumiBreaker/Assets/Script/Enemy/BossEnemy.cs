using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : EnemyBase
{
    // 状態遷移するまでのタイマー
    protected float _stateTimer = 0.0f;
    // 攻撃するまでのタイマー
    protected float _attackTimer = 0.0f;
    // プレイヤーが攻撃範囲に入ったら攻撃状態に遷移させるためのフラグ
    protected bool _isAttack = false;
    // フェーズチェンジしたかどうかのフラグ
    protected bool _isPhaseChanged = false;

    public int _attackCount = 0; // 攻撃回数カウント用

    private BossAttackDataList _bossAttackData;

    protected override void Start()
    {
        // 親クラスのStart()を呼び出す
        base.Start();
        // ボス専用の初期化処理をここに追加
        ChangeState(new BossIdleState(this));
    }

    protected override void Update()
    {
        // 親クラスのUpdate()を呼び出す
        base.Update();
    }

    // 通常攻撃(ボスによって変えたい場合はオーバライド)
    public virtual void MeleeAttack()
    {
        animator.SetTrigger("MeleeAttack");

        // 移動を停止
        StopMovement();

        Debug.Log("通常攻撃");
        // ボス専用の近接攻撃処理をここに追加

        ChangeState(new BossIdleState(this));
    }

    // 範囲攻撃(ボスによって変えたい場合はオーバライド)
    public virtual void RangeAttack()
    {
        animator.SetTrigger("RangeAttack");

        // 移動を停止
        StopMovement();

        Debug.Log("範囲攻撃");
        // ボス専用の遠距離攻撃処理をここに追加

        ChangeState(new BossIdleState(this));
    }

    // 長距離攻撃(ボスによって変えたい場合はオーバライド)
    public virtual void LongRangeAttack()
    {
        animator.SetTrigger("LongRangeAttack");

        // 移動を停止
        StopMovement();

        Debug.Log("長距離攻撃");
        // ボス専用の長距離攻撃処理をここに追加

        _attackCount = 0; // 攻撃回数リセット

        ChangeState(new BossIdleState(this));
    }

    public virtual void Stan()
    {
        // ボス専用のスタンする処理をここに追加

        // 一定時間動けなくするなど

    }

    public virtual void PhaseChange()
    {
        // ボス専用のフェーズを変える処理をここに追加
        // 攻撃パターンの変更するフラグを立てるなど

    }

    // モデルのリグを取得して攻撃判定を特定のボーンにアタッチする処理
    


    private BossAttackData SerachAttackData(string attackName)
    {
        // 攻撃データを格納する変数
        BossAttackData attackData = null;

        // ボスの攻撃データリストから攻撃名に一致するデータを探す
        if (attackName == null) return attackData;

        // リストをループして攻撃名を比較
        for (int i = 0; i < _bossAttackData.bossAttackDataList.Count; i++)
        {
            if (_bossAttackData.bossAttackDataList[i].attackName == attackName)
            {
                attackData = _bossAttackData.bossAttackDataList[i];
                break;
            }
        }

        return attackData;
    }

    public void AttackReset()
    {
        _isAttack = false;
        _attackTimer = 0.0f;
    }
}
