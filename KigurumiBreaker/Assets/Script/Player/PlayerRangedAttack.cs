using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRangedAttack : MonoBehaviour
{
    private AttackData _attackData;

    private GameObject _target;

    // 現在の向いている方向
    private Vector3 _currentDir;

    // 1フレームで変化できる角度
    [SerializeField] private float _rotateSpeed = 5.0f;

    // 移動速度
    [SerializeField] private float _moveSpeed = 5.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        // 向きをターゲットの方向に徐々に変える
        if (_target != null)
        {
            Vector3 targetDir = (_target.transform.position - this.transform.position).normalized;
            Vector3 newDir = Vector3.RotateTowards(_currentDir, targetDir, _rotateSpeed * Mathf.Deg2Rad, 0.0f);
            this.transform.rotation = Quaternion.LookRotation(newDir);
            _currentDir = newDir;
        }

        // 向いている方向に進む
        transform.position += _currentDir * _moveSpeed * Time.fixedDeltaTime;
    }

    public void SetAttackData(AttackData data)
    {
        this._attackData = data;
    }
    public void SetTarget(GameObject target)
    {
        _target = target;
    }
    public void SetCurrentDir(Vector3 dir)
    {
        _currentDir = dir;
        // 最初の向きを設定
        this.transform.rotation = Quaternion.LookRotation(dir);
    }

    public int GetDamage()
    {
        return _attackData.damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // エフェクトを出す
            if (_attackData.hitEffect != null)
            {
                // ヒットする位置を計算
                Vector3 hitPos = other.ClosestPoint(this.transform.position);
                // 少しだけプレイヤー側にずらす
                Vector3 shiftVec = (this.transform.position - hitPos).normalized;
                hitPos += shiftVec * _attackData.effectShiftScale;
                Instantiate(_attackData.hitEffect, hitPos, Quaternion.identity);
            }
            // 攻撃が当たったら消す
            Destroy(this.gameObject);
        }
        //else if (other.gameObject.CompareTag("Wall"))
        //{
        //    // 壁に当たったら消す
        //    Destroy(this.gameObject);
        //}
    }

}
