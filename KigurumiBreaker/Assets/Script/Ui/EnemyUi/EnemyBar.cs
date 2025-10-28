using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBar : MonoBehaviour
{
    private RectTransform foreground = null;
    private Enemy _enemy = null;

    private void Start()
    {
        _enemy = transform.parent.GetComponent<Enemy>();

        foreach (RectTransform child in GetComponentsInChildren<RectTransform>())
        {
            if (child.gameObject.name == "HpBar")
            {
                foreground = child;
            }
        }
    }

    private void Update()
    {
        float hpPercentage = _enemy.GetCurrentHp() / _enemy.enemyData.maxHp;

        Image fgImage = foreground.GetComponent<Image>();
        fgImage.fillAmount = hpPercentage;
        foreground.localPosition = new Vector3(-100 + 100 * hpPercentage, 0, 0);

        // カメラに向ける
        transform.forward = Camera.main.transform.forward;
    }

    public void SetTarget(Enemy enemy)
    {
        _enemy = enemy;
    }

    //[Header("UI要素")]
    //[SerializeField] private Image _hpBarImage; // HPバー画像
    //[SerializeField] private Image _hpDelayImage; // HPバー遅れてくる画像
    //[SerializeField] private Image _trunkBarImage; // 耐久バー画像
    //[SerializeField] private Image _trunkDelayImage; // 耐久バー遅れてくる画像
    //[SerializeField] private Vector3 _offset = new Vector3(0, 2.5f, 0); // オフセット

    //// ターゲットのTransform
    //private Transform _target; 
    //// 敵の参照
    //private Enemy _enemy;
    //// ゲームカメラ
    //private Camera _gameCamera;
    //// 現在のHP
    //private float _currentHp;
    //// Hpの減少量
    //private float _hpDecrease;
    //// 現在の耐久力
    //private float _currentTrunk;
    //// trunkの減少量
    //private float _trunkDecrease;
    //// 敵のレンダラー
    //private Renderer _renderer; 

    //private void Start()
    //{
    //    // ゲーム内カメラを取得する
    //    _gameCamera = Camera.main;

    //    // 敵のポジションを取得する
    //    _enemy = _target.GetComponent<Enemy>();

    //    // 敵のレンダラーを取得する
    //    _renderer = _target.GetComponentInChildren<Renderer>();

    //    // 敵のHpと耐久力を初期化する
    //    _currentHp = _hpDecrease = _enemy.GetCurrentHp() / _enemy.enemyData.maxHp;
    //    _currentTrunk = _trunkDecrease = _enemy.GetCurrentTrunk() / _enemy.enemyData.maxTrunk;
    //}

    //private void LateUpdate()
    //{
    //    // カメラ外では非表示にする
    //    bool isVisible = _renderer != null && _renderer.isVisible;
    //    gameObject.SetActive(isVisible);
    //    if(!isVisible) return;

    //    // HPと耐久力の現在値を更新する
    //    _currentHp = Mathf.Lerp(_currentHp, _hpDecrease, Time.deltaTime * 10f);
    //    _hpBarImage.fillAmount = _currentHp;

    //    _currentTrunk = Mathf.Lerp(_currentTrunk, _trunkDecrease, Time.deltaTime * 10f);
    //    _trunkBarImage.fillAmount = _currentTrunk;

    //    // HPと耐久力の現在値から少し遅れてくるバーを更新する
    //    _hpDelayImage.fillAmount = Mathf.Lerp(_hpDelayImage.fillAmount, _hpDecrease, Time.deltaTime * 2f);
    //    _trunkDelayImage.fillAmount = Mathf.Lerp(_trunkDelayImage.fillAmount, _trunkDecrease, Time.deltaTime * 2f);

    //    // 頭上に追従
    //    transform.position = _target.position + _offset;
    //    transform.LookAt(_gameCamera.transform);
    //    transform.Rotate(0, 180, 0); // 反転
    //}

    //// Hpバーと耐久力バーの値を更新するメソッド
    //public void SetHp(float current, float max)
    //{
    //    _hpDecrease = Mathf.Clamp01(current / max);
    //}

    //public void SetTrunk(float current, float max)
    //{
    //    _trunkDecrease = Mathf.Clamp01(current / max);
    //}
}
