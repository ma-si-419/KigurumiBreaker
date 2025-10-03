using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCol : MonoBehaviour
{

    enum AttackType //攻撃の種類
    {
        Low,        //弱攻撃
        Middle,     //中攻撃
        High,       //強攻撃
    }

    [Header("ショット敵変数")]
    [SerializeField] private float _shootSpeed;   // 弾の速度
    [SerializeField] private float _shotLifeTime; // 弾の寿命

    [Header("ヒットダメージ設定")]
    [SerializeField]private float _damage;            // ヒットダメージ
    [SerializeField] private AttackType _damageKind;   // ダメージの種類（弱、中、強）
    [SerializeField] private GameObject _hitEffectPrefab;   // ヒットエフェクトのプレハブ

    [SerializeField]private int _lifeTime;         // 攻撃判定の寿命（フレーム数）

    private GameObject _attackEnemy;    // 攻撃を行った敵

    private Vector3 _moveVec = Vector3.zero; // 移動ベクトル


    private void Start()
    {
        // 一定時間後に弾を破壊
        if (CompareTag("EnemyRangedAttack"))
        {
            Destroy(gameObject, _shotLifeTime); // 一定時間後に弾を破壊
        }
    }

    private void FixedUpdate()
    {
        _lifeTime--;

        if(CompareTag("EnemyAttack"))
        {
            if (_lifeTime <= 0)
            {
                //攻撃判定の寿命が来たら消す
                Destroy(this.gameObject);
            }
        }

        if(CompareTag("EnemyRangedAttack"))
        {
            // 弾を前方に移動
            transform.Translate(Vector3.forward * _shootSpeed * Time.deltaTime);
        }

    }

    public float GetDamage()
    {
        return _damage;
    }

    public PlayerState.DamageKind GetDamageKind()
    {
        return (PlayerState.DamageKind)_damageKind;
    }

    public GameObject GetHitEffectPrefab()
    {
        return _hitEffectPrefab;
    }

    public void SetMoveVec(Vector3 vec)
    {
        _moveVec = vec;
    }

    public void SetAttackEnemy(GameObject enemy)
    {
        _attackEnemy = enemy;
    }

    public Vector3 GetEnemyPos()
    {
        if(_attackEnemy != null)
        {
            return _attackEnemy.transform.position;
        }
        return Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("EnemyRangedAttack"))
        {
            if (other.CompareTag("Player"))
            {
                // プレイヤーにダメージを与える処理
                Debug.Log("プレイヤーに攻撃した");
                Destroy(gameObject);    // 弾を削除
            }
            else if (other.CompareTag("Wall"))
            {
                // 壁や障害物に当たった場合も弾を削除
                Destroy(gameObject);
            }
        }
    }
}
