using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaitoTest : MonoBehaviour
{
    private bool _isLaser = false;
    private float _laserTime = 0.0f;
    private float _damage = 10.0f;
    private float _laserInterval = 1.0f;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("EnemyAttack"))
    //    {
    //        Debug.Log("攻撃された");
    //    }
    //    if (other.CompareTag("EnemyContinuousAttack"))
    //    {
    //        // 最初のレーザー攻撃だけ
    //        _isLaser = true;
    //        _laserTime = 0.0f;
    //        // ダメージ量設定
    //        // レーザーのインターバル設定
    //        _laserTime = _laserInterval;

    //        Debug.Log("EnemyContinuousAttack攻撃");
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            Debug.Log("攻撃されてる");
        }   
        if (other.CompareTag("EnemyContinuousAttack"))
        {
            if (!_isLaser)
            {
                //ここでダメージ処理
                _isLaser = true;
            }

            if(_isLaser)
            {
                _laserTime += Time.deltaTime;

                if (_laserTime >= _laserInterval)
                {
                    Debug.Log("EnemyContinuousAttack攻撃");
                    //ここでダメージ処理
                    _isLaser = false;
                    _laserTime = 0.0f;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("EnemyContinuousAttack"))
        {
            _isLaser = false;
            _laserTime = 0.0f;
        }
    }
}
