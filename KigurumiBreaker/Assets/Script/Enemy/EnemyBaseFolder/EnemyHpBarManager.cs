//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class EnemyHpBarManager : MonoBehaviour
//{
//    [Header("リファレンス")]
//    [SerializeField] private Camera _mainCmaera;        // メインカメラ
//    [SerializeField] private RectTransform _canvasRect; // キャンバスのRectTransform
//    [SerializeField] private GameObject _hpBarPrefab;   // HPバーのプレハブ

//    [Header("プール/レイアウト")]
//    [SerializeField] private int _initialPoolSize = 100; // 初期プールサイズ
//    [SerializeField] private float _y0ffset = 1.8f;      // 敵頭上オフセット
//    [SerializeField] private float _cullDistance = 30f;  // これ以上遠ければ非表示にする(調整)

//    //内部データ構造
//    private Queue<GameObject> _pool = new Queue<GameObject>(); // オブジェクトプール
//    private Dictionary<Enemy, GameObject> _activeBars = new Dictionary<Enemy, GameObject>(); // アクティブなHPバー
//    private List<KeyValuePair<Enemy, GameObject>> _activeListCache = new List<KeyValuePair<Enemy, GameObject>>(); // 更新用キャッシュ

//    // 初期化
//    private void Awake()
//    {
//        // メインカメラの取得
//        if (_mainCmaera == null) _mainCmaera = Camera.main;
//        // キャンバスのRectTransformの確認
//        if (_canvasRect == null) Debug.LogError("_canvasRectはEnemyHpBarManagerに割り当てられていません。");
//        // HPバーのプレハブの確認
//        if (_hpBarPrefab == null) Debug.LogError("hpBarPrefabはEnemyHpBarManagerに割り当てられていません。");


//        // 初期プールの生成
//        for (int i = 0; i < _initialPoolSize; i++)
//        {
//            // プールに新しいHPバーを生成
//            var g = Instantiate(_hpBarPrefab, _canvasRect);
//            // 非アクティブ化してプールに追加
//            g.SetActive(false);
//            // プールに追加
//            _pool.Enqueue(g);
//        }
//    }

//    private void LateUpdate()
//    {
//        // アクティブなHPバーの更新
//        _activeListCache.Clear();
//        // すべてのアクティブなHPバーを更新
//        foreach (var kv in _activeBars) _activeListCache.Add(kv);

//        // 位置と表示の更新
//        foreach (var kv in _activeListCache)
//        {
//            // キーと値の取得
//            var enemy = kv.Key;
//            // 敵が無効または非表示ならHPバーを非表示にしてプールに戻す
//            var go = kv.Value;

//            // 敵が無効または非表示ならHPバーを非表示にしてプールに戻す
//            if(enemy == null || go == null)
//            {
//                if (enemy != null) ;
//            }


            

//        }
//    }

//    // 登録 : 敵が生成されたときに呼び出す
//    public void RegisterEnemy(Enemy enemy)
//    {
//        // nullチェック
//        if (enemy == null) return;
//        // すでに登録されている場合は無視
//        if (_activeBars.ContainsKey(enemy)) return;

//        // プールからHPバーを取得、なければ新規生成
//        GameObject barObj = (_pool.Count > 0) ? _pool.Dequeue() : Instantiate(_hpBarPrefab, _canvasRect);
//        // 敵のHPバーを辞書に登録
//        barObj.transform.SetParent(_canvasRect, false);
//        // 参照を設定
//        barObj.SetActive(true);

//        // EnemyHpBarコンポーネントを取得して初期化
//        var hb = barObj.GetComponent<EnemyHpBar>();
//        // 初期化
//        if (hb != null) hb.SetFillRatio(enemy.GetCurrentHp() / enemy.GetMaxHp());

//        // 辞書に登録
//        _activeBars.Add(enemy, barObj);

//    }

//    // 登録解除 : 敵が破壊されたときに呼び出す
//    public void UnregisterEnemy(Enemy enemy)
//    {
//        // nullチェック
//        if (enemy == null) return;
//        // 登録されていなければ無視
//        if (!_activeBars.TryGetValue(enemy, out var go)) return;

//        // HPバーを非表示にしてプールに戻す
//        _activeBars.Remove(enemy);
//        //ReturnToPool(go);
//    }

//    // 敵のHp更新通知(ダメージを受けた時に呼ぶ想定)
//    public void UpdateHpValue(Enemy enemy)
//    {
//        // nullチェック
//        if(enemy == null) return;
//        // 登録されていなければ無視
//        if (!_activeBars.TryGetValue(enemy, out var go)) return;

//        var hb = go.GetComponent<EnemyHpBar>();
//        //if (hb != null) hb.SetFillRatio(enemy.GetCurrentHp() / enemy.GetMaxHp);
//    }

//    private void UpdateHpBarPosition(Enemy enemy, GameObject barObj)
//    {

//    }

//    void Start()
//    {
        
//    }

//    void Update()
//    {
        
//    }
//}
