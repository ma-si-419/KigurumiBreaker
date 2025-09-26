using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCol : MonoBehaviour
{
    //private bool _isAttackHit = false;

    private void OnTriggerEnter(Collider other)
    {
        //if(_isAttackHit) return; // 既に攻撃がヒットしている場合は処理を行わない

        if (other.CompareTag("Player"))
        {
            // プレイヤーにダメージを与える処理
            Debug.Log("プレイヤーに攻撃した");
            //_isAttackHit = true; // 攻撃がヒットしたことを記録
            //Destroy(gameObject);    // 攻撃判定オブジェクトを削除
        }
    }
}
