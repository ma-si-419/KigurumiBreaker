using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttackCol : MonoBehaviour
{
    enum AttackType //攻撃の強さ
    {
        Low,        //弱攻撃
        Middle,     //中攻撃
        High,       //強攻撃
    }

    enum AttackKind //攻撃の種類
    {
        Normal,     //通常の攻撃用
        Laser,      //レーザー攻撃用
        Breath,     //ブレス攻撃用
    }

    [Header("ショット敵変数")]
    [SerializeField] private float _shootSpeed;   // 弾の速度
    [SerializeField] private float _shotLifeTime; // 弾の寿命

    [Header("ヒットダメージ設定")]
    [SerializeField] private float _damage;            // ヒットダメージ
    [SerializeField] private AttackType _damageKind;   // ダメージの種類（弱、中、強）
    [SerializeField] private GameObject _hitEffectPrefab;   // ヒットエフェクトのプレハブ

    [SerializeField] private int _lifeTime;         // 攻撃判定の寿命（フレーム数）

    private bool _setBattleManager = false;

    private GameObject _attackEnemy;    // 攻撃を行った敵

    private BattleManager _battleManager;

    private Vector3 _moveDir = new Vector3(0.0f, 0.0f, 1.0f); // 移動方向ベクトル(仮で前方向を入れておく)

    private bool _isStop = false;   // ヒットストップ中に動かないようにするフラグ

    private void Start()
    {
        // 弾の寿命をフレーム数に変換してセット
        if (CompareTag("EnemyRangedAttack"))
        {
            _lifeTime = (int)_shotLifeTime;
        }
    }

    private void FixedUpdate()
    {
        if(!_setBattleManager)
        {
            if(_battleManager != null)
            {
                Debug.Log("AddEnemyAttack");
                _setBattleManager = true;
            }
            else
            {
                Debug.Log("バトルマネージャーが設定されていません");
            }
        }

        if (_isStop) return;

        _lifeTime--;
        if (_lifeTime < 0)
        {
            _battleManager.GetComponent<BattleManager>().RemoveEnemyAttack(this.gameObject);
            //攻撃判定の寿命が来たら消す
            Destroy(this.gameObject);
        }

        if (CompareTag("EnemyRangedAttack"))
        {
            // 弾を前方に移動
            transform.Translate(_moveDir * _shootSpeed * Time.deltaTime);
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

    public void SetMoveDir(Vector3 dir)
    {
        // 一応正規化しておく
        dir = dir.normalized;
        _moveDir = dir;
    }

    public void SetAttackEnemy(GameObject enemy)
    {
        _attackEnemy = enemy;
    }

    public void SetBattleManager(BattleManager manager)
    {
        _battleManager = manager;
    }

    public Vector3 GetEnemyPos()
    {
        if (_attackEnemy != null)
        {
            return _attackEnemy.transform.position;
        }
        return Vector3.zero;
    }

    public void SetStop(bool isStop)
    {
        _isStop = isStop;
        if (isStop)
        {
            // エフェクトの再生を止める
            ParticleSystem ps = GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Pause();
            }
        }
        else
        {
            // エフェクトの再生を再開する
            ParticleSystem ps = GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("EnemyRangedAttack"))
        {
            if (other.CompareTag("Wall"))
            {
                Debug.Log("EnemyAttackCol(OnTriggerEnter):弾が壁に当たった");

                _battleManager.GetComponent<BattleManager>().RemoveEnemyAttack(this.gameObject);
                // 壁や障害物に当たった場合弾を削除
                Destroy(gameObject);

            }
        }
    }
}
