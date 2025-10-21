using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMeleeAttackState : IState
{
    //ボス敵の参照
    private BossEnemy _boss;   

    //状態遷移用タイマー
    private float _stateTimer = 0.0f;

    //攻撃オブジェクトを一度だけ生成するフラグ
    private bool _isCreateAttack = false;


    public BossMeleeAttackState(BossEnemy boss)
    {
        //コンストラクタでEnemyの参照を受け取る
        _boss = boss;
    }

    public void Init()
    {
        //通常攻撃アニメーション開始
        _boss.animator.SetTrigger("MeleeAttack");
    }

    public void Update()
    {
        Debug.Log("とりあえず");

        var stateInfo = _boss.animator.GetCurrentAnimatorStateInfo(0);

        //アニメーションの特定のタイミングで攻撃オブジェクトを生成
        if (stateInfo.IsName("MeleeAttack") && stateInfo.normalizedTime >= 0.6f)
        {
            //攻撃判定を一つ生成させる
            if (!_isCreateAttack)
            {
                _isCreateAttack = true;
                CreateAttack();
            }
        }

    }

    public void End()
    {
        //通常攻撃アニメーション終了
        _boss.animator.ResetTrigger("MeleeAttack");
    }

    // 攻撃オブジェクトを生成する関数
    private void CreateAttack()
    {
        Debug.Log("攻撃オブジェクト生成");

        //ゲームオブジェクト生成
        //GameObject attackObject = Instantiate(_boss._attackObjectPrefab);

        // ゲームオブジェクト生成
        //_attackObject = Instantiate(_attackObjectPrefab);

        //float yOffset = 1.0f; // Y軸のオフセット値（必要に応じて調整）

        // 攻撃オブジェクトの位置を調整(Y軸を調整したい)
        //attackObject.transform.position = _boss.transform.position + _boss.transform.forward * _boss._attackDistance + Vector3.up * yOffset;

    }
}
