using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEngine.Rendering.VirtualTexturing;

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

    // ボスの攻撃データリスト
    protected BossAttackDataList _bossAttackData;

    //// ボスの攻撃パターン
    //protected AttackPatterns _attackPatterns; 

    // Getter
    public BossAttackDataList bossAttackData => _bossAttackData;
    //public AttackPatterns attackPatterns => _attackPatterns;

    

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
