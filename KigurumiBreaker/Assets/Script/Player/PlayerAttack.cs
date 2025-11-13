using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    const float REFLECT_DAMAGE_RATE = 0.5f;

    const float ATTACK_MOVE_SPEED = 1.5f;

    [SerializeField] private int _attackLifeTime = 180;
    [SerializeField] private float _damage = 3.0f;

    public struct PlayerAttackData
    {
        public float damage;
        public float knockBackPower;
        public int attackLifeTime;
        public GameObject hitEffect;
        public GameObject chaseAttack;
        public Enemy.EnemyDebuff debuffType;
        public bool isReflect;
        public CameraMove.ShakeKind shakeKind;
        public int hitStopFrame;
        public bool isWeakAttack;
    }

    private GameObject _camera; // カメラオブジェクトの参照

    private PlayerAttackData _attackData;

    private Vector3 _playerPos;

    private Vector3 _moveVec = Vector3.zero;

    private BattleManager _battleManager;

    [SerializeField] private float effectShiftScale = 0.5f;

    // Start is called before the first frame update
    void Awake()
    {
        _attackData.damage = _damage;
        _attackData.attackLifeTime = _attackLifeTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 移動ベクトルがあったらそっちの方向に動く
        if (_moveVec.magnitude > 0.01f)
        {
            transform.position += _moveVec;
        }

        _attackLifeTime--;

        if (_attackLifeTime <= 0)
        {
            _battleManager.GetComponent<BattleManager>().RemovePlayerAttack(this.gameObject);

            //攻撃判定の寿命が来たら消す
            Destroy(this.gameObject);
        }
    }

    public void SetPlayerPos(Vector3 pos)
    {
        _playerPos = pos;
    }

    public void SetCamera(GameObject camera)
    {
        _camera = camera;
    }
    public void SetBattleManager(BattleManager battleManager)
    {
        _battleManager = battleManager;
    }

    public void SetMoveVec(Vector3 vec)
    {
        _moveVec = vec;
    }

    public void SetPlayerAttackData(PlayerAttackData data)
    {
        _attackData = data;
        _attackLifeTime = _attackData.attackLifeTime;
        _damage = _attackData.damage;
    }

    public float GetDamage()
    {
        return _attackData.damage;
    }

    public int GetHitStopTime()
    {
        return _attackData.hitStopFrame;
    }

    public bool GetIsWeakAttack()
    {
        return _attackData.isWeakAttack;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // プレイヤーの特殊ゲージを増加させる
            _battleManager.AddPlayerSpecialGauge(3.0f);// TODO:後で調整(定数にするかどうかも悩み中)

            //エフェクトを出す
            if (_attackData.hitEffect != null)
            {
                // ヒットする位置を計算
                Vector3 hitPos = other.ClosestPoint(this.transform.position);

                // 少しだけプレイヤー側にずらす
                Vector3 shiftVec = (_playerPos - hitPos).normalized;
                hitPos += shiftVec * effectShiftScale;


                Instantiate(_attackData.hitEffect, hitPos, Quaternion.identity);
            }

            // カメラを揺らす
            if (_camera != null)
            {
                CameraMove cameraMove = _camera.GetComponent<CameraMove>();

                cameraMove.SetShakeData(_attackData.shakeKind);
            }
        }
        else if (other.CompareTag("EnemyRangedAttack"))
        {
            if (_attackData.isReflect)
            {
                // タグをプレイヤーの攻撃に変更
                other.tag = "PlayerAttack";

                // 敵の攻撃スクリプトを取得
                EnemyAttackCol enemyAttack = other.GetComponent<EnemyAttackCol>();
                // プレイヤーの攻撃データを設定
                PlayerAttack playerAttack = other.AddComponent<PlayerAttack>();

                PlayerAttackData data = new PlayerAttackData();

                // 後で調整
                data.damage = enemyAttack.GetDamage() * REFLECT_DAMAGE_RATE;
                data.attackLifeTime = _attackLifeTime;
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
