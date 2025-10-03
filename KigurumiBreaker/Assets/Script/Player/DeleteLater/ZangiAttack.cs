using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ZangiAttack : MonoBehaviour
{

    public float damage = 10;

    [Header("0~2")]
    [SerializeField]private int damageKind = 0;
    [SerializeField]private GameObject hitEffectPrefab;

    public int lifeTime = 150;

    private GameObject _attackEnemy;

    private Vector3 _moveVec = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 移動ベクトルがあったらそっちの方向に動く
        if(_moveVec.magnitude > 0.1f)
        {
            transform.position += _moveVec;

        }
        else
        {
            // 前後に移動(sin)
            transform.position += new Vector3(Mathf.Sin(Time.time * 5) * 0.1f, 0, 0);
        }

        if(CompareTag("EnemyRangedAttack"))
        {
            lifeTime--;
            if(lifeTime < 0)
            {
                Destroy(this.gameObject);
                return;
            }
        }

    }

    public float GetDamage()
    {
        return damage;
    }
    public GameObject GetHitEffectPrefab()
    {
        return hitEffectPrefab;
    }

    public PlayerState.DamageKind GetDamageKind()
    {
        return (PlayerState.DamageKind)damageKind;
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
}
