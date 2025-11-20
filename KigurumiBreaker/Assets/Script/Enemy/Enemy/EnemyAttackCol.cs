using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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

    [Header("ドーナツ敵変数")]
    //[SerializeField] private bool _isDonut; // ドーナツ状の処理を書くよう

    //private float _outerRadius;  // ドーナツの外径
    //private float _innerRadius;  // ドーナツの内径(穴の大きさ)

    //private Mesh _mesh; // メッシュ
    //private Vector3[] _originalVects;   // メッシュ一つ一つのベクトル

    //private float _RadiusAdd;   // 広がっていくための足す変数


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

        //if(_isDonut)
        //{
        //    _mesh = GetComponent<MeshFilter>().mesh;
        //    _originalVects = _mesh.vertices.Clone() as Vector3[];
        //    DonutUpdate();
        //}

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

        //if (_isDonut)
        //{
        //    _donutCurrentScale++;
        //    this.transform.localScale = new Vector3(_donutCurrentScale, _donutCurrentScale, 100);

        //    if (_donutCurrentScale > 1000)
        //    {
        //        Destroy(this.gameObject);
        //    }
        //}

        if (CompareTag("EnemyRangedAttack"))
        {
            // 弾を前方に移動
            transform.Translate(_moveDir * _shootSpeed * Time.deltaTime);
        }
    }

    //private void DonutUpdate()
    //{
    //    Vector3[] verts = _mesh.vertices;

    //    for (int i = 0; i < verts.Length; i++)
    //    {
    //        // ローカル座標上でXZ平面の位置を計算
    //        Vector3 v = _originalVects[i];

    //        // 中心からの方向
    //        Vector2 dir = new Vector2(v.x, v.z).normalized;

    //        // ドーナツの外径部分(円の中心レベル)
    //        float ringDist = _outerRadius;

    //        // リングの太さ
    //        float tubeDist = _innerRadius;

    //        // 新しい位置を構築
    //        float angle = Mathf.Atan2(v.z, v.x);

    //        float radiusOnXZ = ringDist + Mathf.Cos(v.y * Mathf.PI) * tubeDist;

    //        float y = Mathf.Sin(v.y * Mathf.PI) * tubeDist;

    //        verts[i] = new Vector3(Mathf.Cos(angle) * radiusOnXZ, y, Mathf.Sin(angle) * radiusOnXZ);
    //    }

    //    _mesh.vertices = verts;
    //    _mesh.RecalculateNormals();
    //    _mesh.RecalculateBounds();

    //    MeshCollider col = GetComponent<MeshCollider>();

    //    if(col)
    //    {
    //        col.sharedMesh = null;
    //        col.sharedMesh = _mesh;
    //    }
    //}

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
