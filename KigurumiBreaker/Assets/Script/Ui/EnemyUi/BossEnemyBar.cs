using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BossEnemyBar : MonoBehaviour
{
    [Header("HpBar関連")]
    // 現在のHpを表示するImage
    [SerializeField] private Image _currentHpImg;
    // 遅れて減る赤バー
    [SerializeField] private Image _currentHpDelayedImg;

    // ボスの参照
    private BossEnemy _bossEnemy;

    // 硬直時間
    private float _delayRigidityTime;
    // 補間速度
    private float _lerpSpeed;
    // 表示比率
    private float _displayHpRatio;
    // コルーチン管理用
    private Coroutine _hpCoroutine;

    public void SetTarget(BossEnemy bossEnemy)
    {
        // 敵の参照
        _bossEnemy = bossEnemy;

        // 硬直時間を設定
        _delayRigidityTime = _bossEnemy.enemyCommonData.delayRigidityTime;
        // 補間速度を設定
        _lerpSpeed = _bossEnemy.enemyCommonData.lerpSpeed;

        // 初期表示
        _currentHpImg.fillAmount = _bossEnemy.GetCurrentHp() / _bossEnemy.enemyData.maxHp;
        _currentHpDelayedImg.fillAmount = _currentHpImg.fillAmount;
    }

    private void Update()
    {
        // 敵が存在しない場合はUIを破棄
        if (_bossEnemy == null)
        {
            Destroy(gameObject);
            return;
        }

        /* HpBar処理 */
        float targetHpRatio = _bossEnemy.GetCurrentHp() / _bossEnemy.enemyData.maxHp;

        // 現在のHpが目標値と異なる場合
        if (_currentHpImg.fillAmount != targetHpRatio)
        {
            // 即座に減らす
            _currentHpImg.fillAmount = targetHpRatio;

            // 遅延バーの減少をストップ
            if (_hpCoroutine != null) StopCoroutine(_hpCoroutine);

            // 遅延バーの減少を開始
            _hpCoroutine = StartCoroutine(HpDelayDecrease(targetHpRatio));

        }
    }

    // Hp用コルーチン
    private IEnumerator HpDelayDecrease(float target)
    {
        // 1秒硬直
        yield return new WaitForSeconds(_delayRigidityTime);

        // ゆっくり減少
        while (_currentHpDelayedImg.fillAmount > target)
        {
            _currentHpDelayedImg.fillAmount -= Time.deltaTime * _lerpSpeed;
            yield return null;
        }

        // 目標値に合わせる
        _currentHpDelayedImg.fillAmount = target;
    }
}
