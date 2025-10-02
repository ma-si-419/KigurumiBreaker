using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerState;


public class ShotEnemyAttackCol : MonoBehaviour
{
    [Header("ショット敵変数")]
    [SerializeField] public float speed;   // 弾の速度
    [SerializeField] public float lifeTime; // 弾の寿命

    [Header("ヒットダメージ設定")]
    [SerializeField]private int _damage;            // ヒットダメージ
    [SerializeField] private int _damageKind;        // 0:通常 1:火炎 2:氷結
    [SerializeField] private int _lifeTime;         // 攻撃判定の寿命（フレーム数）

    // 弾を撃ったオブジェクト
    public Enemy owner { get; private set; }

    public void SetOwner(Enemy enemy)
    {
        owner = enemy;
    }

    //弾を反射するために必要になる誰が撃ったかの情報

    void Start()
    {
        Destroy(gameObject, lifeTime); // 一定時間後に弾を破壊
    }

    // Update is called once per frame
    void Update()
    {
        // 弾を前方に移動
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
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

    public int GetDamage()
    {
        return _damage;
    }

    public PlayerState.DamageKind GetDamageKind()
    {
        return (PlayerState.DamageKind)_damageKind;
    }

}
