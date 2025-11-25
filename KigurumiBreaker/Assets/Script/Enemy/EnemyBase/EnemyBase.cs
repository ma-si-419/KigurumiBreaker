using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    // 敵のステータスデータ
    [Header("敵のステータスデータ")]
    [SerializeField] protected EnemyData _enemyData;

    // 敵の定数データ
    [Header("敵の定数データ")]
    [SerializeField] protected EnemyCommonData _enemyCommonData;

    // EnemyUiManagerの参照
    protected EnemyBarManager _enemyUiManager;

    // 攻撃タイプ1オブジェクトのプレハブ
    protected GameObject _attackType1ObjectPrefab;

    // 攻撃タイプ2オブジェクトのプレハブ
    protected GameObject _attackType2ObjectPrefab;

    // 攻撃タイプ3オブジェクトのプレハブ
    protected GameObject _attackType3ObjectPrefab;

    // 攻撃タイプ4オブジェクトのプレハブ
    protected GameObject _attackType4ObjectPrefab;

    // ドロップする弾のプレハブ
    protected GameObject _dropBullet;

    // 現在のHP
    protected float _currentHp;

    // 現在の耐久力
    protected float _currentTrunk;

    // 現在のステート状態
    protected IState _currentState;

    // 前のステート状態を保存する変数
    protected IState _previousState;

    //索敵範囲の二乗
    protected float _detectRangeSqr;

    //攻撃範囲の二乗
    protected float _attackRangeSqr;

    //攻撃切り替え範囲の二乗
    protected float _attackSwitchRangeSqr;

    // NavMeshAgentの参照
    protected NavMeshAgent _agent;

    // プレイヤーの参照
    protected GameObject _player;

    // アニメーターの参照
    protected Animator _animator;

    // Rigidbodyの参照
    protected Rigidbody _rigidbody;

    //バトルマネージャーの参照
    protected BattleManager _battleManager;

    // 死んだかどうかのフラグ
    protected bool _isDead = false;

    // 向きたい方向
    protected Vector3 _direction;

    // ヒットストップ用のフラグ
    protected bool _isStop = false;

    // ダメージを受けているかのフラグ
    protected bool _isDamage = false;

    // 震動ベクトル
    protected Vector3 _shakeVec;

    // 停止位置
    protected Vector3 _stopPos;

    // アニメーション速度保存用
    protected float _animationSpeed = 1.0f;

    // アーマーかどうかのフラグ
    protected bool _isArmor;

    // デバッグ用停止フラグ
    protected bool _isDebugStop;

    // ドロップする弾のリスト
    protected List<int> _dropBullets;

    // 弱攻撃を受けている時のフラグ
    protected bool _isWeakAttack = false;

    // 敵の攻撃オブジェクト変数
    protected GameObject _attackObj;

    // 状態遷移フラグ
    protected bool _isStateChange = false;

    //敵のデバフ状態
    public enum EnemyDebuff
    {
        AtkDown,
        DefDown,
        SpeedDown,
        Poison,
        None
    }

    // NavMeshAgentのゲッター
    public NavMeshAgent agent => _agent;
    // Playerのゲッター
    public GameObject player => _player;
    // Animatorのゲッター
    public Animator animator => _animator;
    // 敵のステータスデータのゲッター
    public EnemyData enemyData => _enemyData;
    public EnemyCommonData enemyCommonData => _enemyCommonData;
    public BattleManager battleManager => _battleManager;
    public IState previousState => _previousState;
    public float attackRangeSqr => _attackRangeSqr;

    protected virtual void Start()
    {
        //索敵範囲と攻撃範囲の二乗を計算して保存
        _detectRangeSqr = _enemyData.detectionRange * _enemyData.detectionRange;
        _attackRangeSqr = _enemyData.attackRange * _enemyData.attackRange;
        _attackSwitchRangeSqr = _enemyData.attackSwitchRange * _enemyData.attackSwitchRange;

        // 体力と耐久力の初期化
        _currentHp = _enemyData.maxHp;
        _currentTrunk = _enemyData.maxTrunk;
        // 攻撃オブジェクトのプレハブを設定
        _attackType1ObjectPrefab = _enemyData.attackType1Prefab;
        _attackType2ObjectPrefab = _enemyData.attackType2Prefab;
        _attackType3ObjectPrefab = _enemyData.attackType3Prefab;
        _attackType4ObjectPrefab = _enemyData.attackType4Prefab;
        // ドロップする弾のプレハブを設定
        _dropBullet = _enemyCommonData.dropBullet;
        // 敵データでアーマーかどうかを設定
        _isArmor = _enemyData.isArmor;
        // NavMeshAgentコンポーネントを取得
        _agent = GetComponent<NavMeshAgent>();
        // Animatorコンポーネントを取得
        _animator = GetComponent<Animator>();
        // Rigidbodyコンポーネントを取得
        _rigidbody = GetComponent<Rigidbody>();

        // ドロップする弾のリストを初期化する
        _dropBullets = new List<int>();
        // EnemyUiManagerの参照を取得
        _enemyUiManager = FindObjectOfType<EnemyBarManager>();

        //ステータスからNavMeshAgentの速度を設定
        if (_agent != null)
        {
            _agent.speed = _enemyData.moveSpeed;  // 移動速度を設定
            _agent.stoppingDistance = _enemyData.attackRange; // 攻撃範囲を設定
        }

        // プレイヤー参照が空なら自動で探す
        if (_player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj;
            }
            else
            {
                Debug.LogWarning($"{name}: Playerがシーンに見つかりませんでした！");
            }
        }
    }

    protected virtual void Update()
    {
        // 現在のステートのUpdateメソッドを呼び出す
        _currentState?.Update();

        // 弾をドロップする
        for (int i = 0; i < _dropBullets.Count; i++)
        {
            if (_dropBullets[i] <= 0)
            {
                // ドロップする座標を上側にずらす
                Vector3 dropPos = this.transform.position;
                dropPos.y += _enemyCommonData.dropPosShiftY;

                // 弾をドロップする
                GameObject bullet = Instantiate(_dropBullet, dropPos, Quaternion.identity);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();

                // ランダムな方向に飛ばす
                float xforce = Random.Range(-0.8f, 0.8f);
                float zforce = Random.Range(-0.8f, 0.8f);

                Vector3 forceDir = new Vector3(xforce, 1.0f, zforce).normalized;

                rb.AddForce(forceDir * _enemyCommonData.dropBulletForce, ForceMode.Impulse);

                // ドロップした弾をリストから削除する
                _dropBullets.RemoveAt(i);
                i--;
            }
            else
            {
                _dropBullets[i]--;
            }
        }
    }

    // ステートを変更するメソッド
    public void ChangeState(IState newState)
    {
        // 現在のステートの終了処理を呼び出す
        _currentState?.End();
        // 新しいステートに変更
        _currentState = newState;
        // 新しいステートの開始処理を呼び出す
        _currentState?.Init();
    }

    // 敵がプレイヤーを向く方向を計算して回転
    public void LookAtPlayer()
    {
        // 向きたい方向を計算
        Vector3 direction = (_player.transform.position - transform.position).normalized;
        _direction = direction;
        // 水平方向のみ回転させる
        direction.y = 0;

        if (direction.sqrMagnitude > 0f)
        {
            // 目標の回転を取得
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // 現在の回転から目標の回転へ補完
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _enemyData.rotateSpeed * Time.deltaTime);

        }
    }

    //オブジェクトの移動力をゼロにする
    public void StopMovement()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    // Getterメソッド
    public float GetMaxHp()
    {
        return _enemyData.maxHp;
    }
    public float GetCurrentHp()
    {
        return _currentHp;
    }
    public float GetMaxTrunk()
    {
        return _enemyData.maxTrunk;
    }
    public float GetCurrentTrunk()
    {
        return _currentTrunk;
    }

    //バトルマネージャーの参照をセットするメソッド
    public void SetBattleManager(BattleManager battleManager)
    {
        _battleManager = battleManager;
    }

    public void SetStop(bool flag)
    {
        _isStop = flag;
    }

    public void StopAnimation()
    {
        if (_animator.speed > 0)
        {
            _animationSpeed = _animator.speed;
        }
        _animator.speed = 0;
    }

    public void StartAnimation()
    {
        _animator.speed = _animationSpeed;
    }

    // Gizmosを使って検知範囲と攻撃範囲を表示
    private void OnDrawGizmosSelected()
    {
        // 検知範囲（シアン色のワイヤーフレーム球）
        Gizmos.color = Color.yellow;
        float detectRadius = _enemyData != null ? _enemyData.detectionRange : 0f;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        // 攻撃範囲（赤色のワイヤーフレーム球、必要なら）
        Gizmos.color = Color.red;
        float attackRadius = _enemyData != null ? _enemyData.attackRange : 0f;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    // 敵の攻撃オブジェクトを生成する関数
    public void EnemyAttackCreate(float distance, float up, GameObject attackPrefab)
    {
        // ゲームオブジェクト生成
        _attackObj = Instantiate(attackPrefab);

        // 攻撃オブジェクトにバトルマネージャーをセット
        _attackObj.GetComponent<EnemyAttackCol>().SetBattleManager(_battleManager);

        // 攻撃オブジェクトの位置を調整
        _attackObj.transform.position = this.transform.position + this.transform.forward * distance + this.transform.up * up;
        _attackObj.transform.rotation = this.transform.rotation * Quaternion.Euler(90, 0, 0); ; ;
    }
}
