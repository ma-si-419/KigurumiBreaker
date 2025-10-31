using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyBar : MonoBehaviour
{
    [Header("HpBar関連")]
    // 現在のHpを表示するImage
    [SerializeField] private Image _currentHpImg;
    // 遅れて減る赤バー
    [SerializeField] private Image _currentHpDelayedImg;

    [Header("TrunkBar関連")]
    // 現在の耐久力を表示するImage
    [SerializeField] private Image _currentTrunkImg;
    // 遅れて減る赤バー
    [SerializeField] private Image _currentTrunkDelayedImg;

    // 敵の参照
    private Enemy _enemy;

    // オフセット位置
    private Vector3 _offset = new Vector3(0, 2.5f, 0);
    // 補間速度
    private float _lerpSpeed = 2.0f;
    // 表示比率
    private float _displayHpRatio;
    private float _displayTrunkRatio;

    private RectTransform foreground;

    public void SetTarget(Enemy enemy)
    {
        _enemy = enemy;
        // 初期表示比率を設定
        _displayHpRatio = _enemy.GetCurrentHp() / _enemy.enemyData.maxHp;
        _displayTrunkRatio = _enemy.GetCurrentTrunk() / _enemy.enemyData.maxTrunk;
    }

    private void Update()
    {
        // 敵が存在しない場合は自身を破棄
        if (_enemy == null)
        {
            Destroy(gameObject);
            return;
        }

        if (_enemy != null)
        {
            Debug.Log($"HP: {_enemy.GetCurrentHp()} / {_enemy.enemyData.maxHp}");
            Debug.Log($"TRUNK: {_enemy.GetCurrentTrunk()} / {_enemy.enemyData.maxTrunk}");
            Debug.Log($"fill: {_currentHpImg.fillAmount}");
            Debug.Log($"fill: {_currentTrunkImg.fillAmount}");
        }

        // HpBar処理
        float targetHpRatio = _enemy.GetCurrentHp() / _enemy.enemyData.maxHp;
        _currentHpImg.fillAmount = targetHpRatio;
        _displayHpRatio = Mathf.Lerp(_displayHpRatio, targetHpRatio, Time.deltaTime * _lerpSpeed);
        _currentHpDelayedImg.fillAmount = _displayHpRatio;

        // TrunkBar処理
        float targetTrunkRatio = _enemy.GetCurrentTrunk() / _enemy.enemyData.maxTrunk;
        _currentTrunkImg.fillAmount = targetTrunkRatio;
        _displayTrunkRatio = Mathf.Lerp(_displayHpRatio, targetHpRatio, Time.deltaTime * _lerpSpeed);
        _currentTrunkDelayedImg.fillAmount = _displayTrunkRatio;

        // 位置・向きを更新
        transform.position = _enemy.transform.position + _offset;
        transform.forward = Camera.main.transform.forward;
    }
}
