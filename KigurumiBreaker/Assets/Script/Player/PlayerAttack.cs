using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    private AttackData _attackData;
    private Vector3 _playerPos;

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

    public void SetAttackData(AttackData data)
    {
        this._attackData = data;
    }

    public int GetDamage()
    {
        return _attackData.damage;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
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
    }
}
