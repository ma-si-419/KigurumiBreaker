using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaitoTest : MonoBehaviour
{
    private bool _isLaser = false;
    private float _laserTime = 0.0f;
    //private float _damage = 10.0f;
    private float _laserInterval = 1.0f;

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("EnemyAttack"))
        {
            // 攻撃がLowタイプの場合の処理として書くしかない？
            // if (_enemyAttackType == AttackType.Low)
            // {
                    if (!_isLaser)
                    {
                        //ここでダメージ処理
                        _isLaser = true;
                    }

                    if (_isLaser)
                    {
                        _laserTime += Time.deltaTime;

                        if (_laserTime >= _laserInterval)
                        {
                            //ここでダメージ処理
                            _isLaser = false;
                            _laserTime = 0.0f;
                        }
                    }
            // }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("EnemyAttack"))
        {
            _isLaser = false;
            _laserTime = 0.0f;
        }
    }
}
