using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRangedAttack : MonoBehaviour
{

    public struct RangedAttackData
    {
        public int damage;
        public bool isHoming;
        public Enemy.EnemyDebuff debuffType;
        public bool isKnockBack;
        public float speedRate;
        public GameObject chaseAttack;
        public GameObject hitEffect;
        public float effectShiftScale;
        public int hitStopTime;
    }

    private RangedAttackData _attackData;

    private GameObject _target;


    // 現在の向いている方向
    private Vector3 _currentDir;

    // ドロップする弾丸のプレハブ
    [SerializeField] private GameObject _dropBullet;

    // 1フレームで変化できる角度
    [SerializeField] private float _rotateSpeed;

    // 移動速度
    [SerializeField] private float _moveSpeed = 5.0f;

    // 弾をドロップするときの運動量の大きさ
    [SerializeField] private float _dropBulletForce = 5.0f;

    // 複数個の壁に同時に当たるのを防ぐためのフラグ
    private bool _isHitWall = false;

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
        transform.position += _currentDir * (_moveSpeed * _attackData.speedRate) * Time.fixedDeltaTime;
    }

    public void SetRangedAttackData(RangedAttackData data)
    {
        _attackData = data;
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

    public int GetHitStopTime()
    {
        return _attackData.hitStopTime;
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
        else if (other.gameObject.CompareTag("Wall"))
        {
            if(_isHitWall) return;

            _isHitWall = true;

            // 壁に当たったら消す
            Destroy(this.gameObject);

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

            // 弾をドロップする
            if (_dropBullet != null)
            {
                // 上にホップする感じで出す
                float xforce = Random.Range(-0.5f, 0.5f);
                float zforce = Random.Range(-0.5f, 0.5f);

                Vector3 dropDir = new Vector3(xforce, 1.0f, zforce).normalized;

                GameObject dropObj = Instantiate(_dropBullet, this.transform.position, Quaternion.identity);
                Rigidbody rb = dropObj.GetComponent<Rigidbody>();
                
                rb.AddForce(dropDir * _dropBulletForce, ForceMode.Impulse);
            }
        }

    }

}
