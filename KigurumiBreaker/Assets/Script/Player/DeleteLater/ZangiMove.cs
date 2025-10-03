using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZangiMove : MonoBehaviour
{

    public enum EnemyDebuff
    {
        AtkDown,
        DefDown,
        SpeedDown,
        Poison,
        None
    }

    public int maxHp = 30;
    public int nowHp = 0;

    [SerializeField] private BattleManager _battleManager;

    [SerializeField] private int _dropTime = 100;

    [SerializeField] private GameObject _dropBullet;

    [SerializeField] private float _dropBulletForce = 5.0f;

    [SerializeField] private float _dropPosShiftY = 1.0f;

    [SerializeField] private GameObject _rangedAttack;

    [SerializeField] private int _rangedAttackInterval = 120;

    [SerializeField] private float _rangedAttackDamage = 30;

    [SerializeField] private float _rangedAttackSpeed = 0.2f;

    private bool _isDamage = false;

    private int _time = 0;

    private int _attackTime = 0;

    private List<int> _dropBullets;
    void Start()
    {
        nowHp = maxHp;
        _battleManager.AddEnemy(this.gameObject);

        // ドロップする弾のリストを初期化する
        _dropBullets = new List<int>();

        // 前方向を向く
        Vector3 forward = new Vector3(0,0,-1);

        this.transform.forward = forward;

    }

    void FixedUpdate()
    {
        // 弾をドロップする
        for (int i = 0;i < _dropBullets.Count; i++)
        {
            if(_dropBullets[i] <= 0)
            {
                // ドロップする座標を上側にずらす
                Vector3 dropPos = this.transform.position;
                dropPos.y += _dropPosShiftY;

                // 弾をドロップする
                GameObject bullet = Instantiate(_dropBullet, dropPos, Quaternion.identity);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();

                // ランダムな方向に飛ばす
                float xforce = Random.Range(-0.8f, 0.8f);
                float zforce = Random.Range(-0.8f, 0.8f);

                Vector3 forceDir = new Vector3(xforce, 1.0f, zforce).normalized;

                rb.AddForce(forceDir * _dropBulletForce, ForceMode.Impulse);

                // ドロップした弾をリストから削除する
                _dropBullets.RemoveAt(i);
                i--;
            }
            else
            {
                _dropBullets[i]--;
            }
        }


        // ダメージを受けている間は赤くする
        if (_isDamage)
        {
            _time++;

            if (_time > 15)
            {
                _isDamage = false;
            }
        }
        else
        {
            _time = 0;
            
            this.GetComponent<Renderer>().material.color = Color.white;
        }

        // 前後にsin波を描く
        float z = Mathf.Sin(Time.time * 2.5f) * 0.1f;
        this.transform.position = new Vector3(this.transform.position.x + z, this.transform.position.y, this.transform.position.z);

        _attackTime++;

        // 一定時間ごとに遠距離攻撃を行う
        if (_attackTime > _rangedAttackInterval)
        {
            _attackTime = 0;

            if (_rangedAttack != null)
            {
                // 弾を生成する
                GameObject attack = Instantiate(_rangedAttack, this.transform.position, Quaternion.identity);
                // 攻撃スクリプトに情報を渡す
                ZangiAttack attackScript = attack.GetComponent<ZangiAttack>();
                if(attackScript != null)
                {
                    // 前方向に飛ばす
                    Vector3 forward = this.transform.forward;
                    forward.y = 0;
                    forward.Normalize();
                    forward *= _rangedAttackSpeed; 
                
                    attackScript.SetMoveVec(forward);
                    attackScript.SetAttackEnemy(this.gameObject);
                    attackScript.damage = _rangedAttackDamage;     
                }
            }
        }


    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerAttack"))
        {
            nowHp -= (int)other.gameObject.GetComponent<PlayerAttack>().GetDamage();

            Debug.Log(other.gameObject.GetComponent<PlayerAttack>().GetDamage() + "のダメージ");

            _isDamage = true;

            _time = 0;

            this.GetComponent<Renderer>().material.color = Color.red;

            if (nowHp <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        if(other.gameObject.CompareTag("PlayerRangedAttack"))
        {
            nowHp -= other.gameObject.GetComponent<PlayerRangedAttack>().GetDamage();
            
            Debug.Log(other.gameObject.GetComponent<PlayerRangedAttack>().GetDamage() + "のダメージ");
         
            _isDamage = true;
            
            _time = 0;
            
            this.GetComponent<Renderer>().material.color = Color.red;
            
            if (nowHp <= 0)
            {
                Destroy(this.gameObject);
            }

            // ドロップする弾を一つ増やす
            _dropBullets.Add(_dropTime);

        }
    }
}
