using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZangiMove : MonoBehaviour
{

    public int maxHp = 30;
    public int nowHp = 0;

    [SerializeField] private BattleManager _battleManager;

    private bool isDamage = false;

    int time = 0;


    void Start()
    {
        nowHp = maxHp;
        _battleManager.AddEnemy(this.gameObject);
    }

    void FixedUpdate()
    {
        if (isDamage)
        {
            time++;

            if (time > 15)
            {
                isDamage = false;
            }
        }
        else
        {
            time = 0;
            
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

            isDamage = true;

            time = 0;

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
         
            isDamage = true;
            
            time = 0;
            
            this.GetComponent<Renderer>().material.color = Color.red;
            
            if (nowHp <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
