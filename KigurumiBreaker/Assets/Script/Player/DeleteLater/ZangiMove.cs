using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZangiMove : MonoBehaviour
{

    public int maxHp = 30;
    public int nowHp = 0;

    [SerializeField] private BattleManager _battleManager;

    [SerializeField] private int _dropTime = 100;

    [SerializeField] private GameObject _dropBullet;

    [SerializeField] private float _dropBulletForce = 5.0f;

    [SerializeField] private float _dropPosShiftY = 1.0f;

    private bool _isDamage = false;

    private int _time = 0;

    private List<int> _dropBullets;
    void Start()
    {
        nowHp = maxHp;
        _battleManager.AddEnemy(this.gameObject);

        // ドロップする弾のリストを初期化する
        _dropBullets = new List<int>();
    }

    void FixedUpdate()
    {
        // 弾をドロップする
        for(int i = 0;i < _dropBullets.Count; i++)
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
        float z = Mathf.Sin(Time.time * 5.0f) * 0.1f;
        this.transform.position = new Vector3(this.transform.position.x + z, this.transform.position.y, this.transform.position.z);

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerAttack"))
        {
            nowHp -= other.gameObject.GetComponent<PlayerAttack>().GetDamage();

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
