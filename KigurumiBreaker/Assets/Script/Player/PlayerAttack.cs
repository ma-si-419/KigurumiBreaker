using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    const float REFLECT_DAMAGE_RATE = 0.5f;

    const float ATTACK_MOVE_SPEED = 1.5f;

    const int ATTACK_LIFE_TIME = 180;

    public struct PlayerAttackData
    {
        public float damage;
        public float knockBackPower;
        public int attackLifeTime;
        public GameObject hitEffect;
        public GameObject chaseAttack;
        public Enemy.EnemyDebuff debuffType;
        public bool isReflect;
    }

    private PlayerAttackData _attackData;

    private Vector3 _playerPos;

    private Vector3 _moveVec = Vector3.zero;

    int _lifeTIme = 0;

    [SerializeField] private float effectShiftScale= 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        _lifeTIme = _attackData.attackLifeTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 移動ベクトルがあったらそっちの方向に動く
        if (_moveVec.magnitude > 0.01f)
        {
            transform.position += _moveVec;
        }

        _lifeTIme--;

        if(_lifeTIme <= 0)
        {
            //攻撃判定の寿命が来たら消す
            Destroy(this.gameObject);
        }
    }

    public void SetPlayerPos(Vector3 pos)
    {
        _playerPos = pos;
    }

    public void SetMoveVec(Vector3 vec)
    {
        _moveVec = vec;
    }

    public void SetPlayerAttackData(PlayerAttackData data)
    {
        _attackData = data;
    }

    public float GetDamage()
    {
        return _attackData.damage;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("攻撃あたった:" + other.name);

        if (other.CompareTag("Enemy"))
        {
            //エフェクトを出す
            if(_attackData.hitEffect != null)
            {
                // ヒットする位置を計算
                Vector3 hitPos = other.ClosestPoint(this.transform.position);

                // 少しだけプレイヤー側にずらす
                Vector3 shiftVec = (_playerPos - hitPos).normalized;
                hitPos += shiftVec * effectShiftScale;

                Instantiate(_attackData.hitEffect, hitPos, Quaternion.identity);
            }
        }
        else if(other.CompareTag("EnemyRangedAttack"))
        {
            Debug.Log("Hit EnemyRangedAttack");

            if (_attackData.isReflect)
            {
                Debug.Log("Reflect!");

                // タグをプレイヤーの攻撃に変更
                other.tag = "PlayerAttack";

                // 敵の攻撃スクリプトを取得
                EnemyAttackCol enemyAttack = other.GetComponent<EnemyAttackCol>();
                // プレイヤーの攻撃データを設定
                PlayerAttack playerAttack = other.AddComponent<PlayerAttack>();

                PlayerAttackData data = new PlayerAttackData();

                // 後で調整
                data.damage = enemyAttack.GetDamage() * REFLECT_DAMAGE_RATE;
                data.attackLifeTime = ATTACK_LIFE_TIME;
                data.hitEffect = enemyAttack.GetHitEffectPrefab();
                data.knockBackPower = 0.0f;
                data.chaseAttack = null;
                data.debuffType = Enemy.EnemyDebuff.None;
                data.isReflect = false;
                
                Vector3 reflectVec = (enemyAttack.GetEnemyPos() - this.transform.position).normalized;
                reflectVec *= ATTACK_MOVE_SPEED;

                // Y軸方向の反射は無し
                reflectVec.y = 0.0f;

                // 攻撃情報の設定
                playerAttack.SetPlayerAttackData(data);
                playerAttack.SetPlayerPos(this.transform.position);
                playerAttack.SetMoveVec(reflectVec);

                // 敵の攻撃スクリプトを無効化する
                enemyAttack.enabled = false;

            }
        }
    }
}
