using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

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
    // 耐久力のアイコン
    [SerializeField] private Image _trunkIconImg;

    // 敵の参照
    private Enemy _enemy;

    // オフセット位置
    private Vector3 _offset;
    // 硬直時間
    private float _delayRigidityTime;
    // 補間速度
    private float _lerpSpeed;

    private bool _isArmor; 

    // コルーチン管理用
    private Coroutine _hpCoroutine;
    private Coroutine _trunkCoroutine;

    public void SetTarget(Enemy enemy)
    {
        // 敵の参照
        _enemy = enemy;
        
        // バーの高さをエネミーごとに調整
        _offset.y = _enemy.enemyData.barYPosition;
        // 硬直時間を設定
        _delayRigidityTime = _enemy.enemyCommonData.delayRigidityTime;
        // 補間速度を設定
        _lerpSpeed = _enemy.enemyCommonData.lerpSpeed;

        _isArmor = _enemy.enemyData.isArmor;

        // アーマー装備していない場合の処理
        if (!_enemy.enemyData.isArmor)
        {
            // アーマー装備していない場合はTrunkBarを非表示にする
            _currentTrunkImg.gameObject.SetActive(false);
            _currentTrunkDelayedImg.gameObject.SetActive(false);
            _trunkIconImg.gameObject.SetActive(false);
            return;
        }

        // 初期表示
        _currentHpImg.fillAmount = _enemy.GetCurrentHp() / _enemy.enemyData.maxHp;
        _currentHpDelayedImg.fillAmount = _currentHpImg.fillAmount;
        _currentTrunkImg.fillAmount = _enemy.GetCurrentTrunk() / _enemy.enemyData.maxTrunk;
        _currentTrunkDelayedImg.fillAmount = _currentTrunkImg.fillAmount;
    }

    private void FixedUpdate()
    {
        // 敵が存在しない場合はUIを破棄
        if (_enemy == null)
        {
            Debug.Log("UI破壊！");
            Destroy(gameObject);
            return;
        }
        else
        {
            /* HpBar処理 */
            float targetHpRatio = _enemy.GetCurrentHp() / _enemy.enemyData.maxHp;

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

            /* TrunkBar処理 */
            if (_enemy.GetCurrentTrunk() <= 0f)
            {
                _isArmor = false;
            }

            // アーマー装備時の処理
            if (_enemy.enemyData.isArmor)
            {
                float targetTrunkRatio = _enemy.GetCurrentTrunk() / _enemy.enemyData.maxTrunk;

                // 現在のHpが目標値と異なる場合
                if (_currentTrunkImg.fillAmount != targetTrunkRatio)
                {
                    // 即座に減らす
                    _currentTrunkImg.fillAmount = targetTrunkRatio;

                    // 遅延バーの減少をストップ
                    if (_trunkCoroutine != null) StopCoroutine(_trunkCoroutine);

                    // 遅延バーの減少を開始
                    _trunkCoroutine = StartCoroutine(TrunkDelayDecrease(targetTrunkRatio));
                }
            }

            if (!_isArmor)
            {
                // アーマー装備していない場合はTrunkIconを非表示にする
                _trunkIconImg.gameObject.SetActive(false);
            }

            /* 位置・向きを更新 */
            transform.position = _enemy.transform.position + _offset;
            transform.forward = Camera.main.transform.forward;

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

    // Trunk用コルーチン
    private IEnumerator TrunkDelayDecrease(float target)
    {
        // 1秒硬直
        yield return new WaitForSeconds(_delayRigidityTime);

        // ゆっくり減少
        while (_currentTrunkDelayedImg.fillAmount > target)
        {
            _currentTrunkDelayedImg.fillAmount -= Time.deltaTime * _lerpSpeed;
            yield return null;
        }

        // 目標値に合わせる
        _currentTrunkDelayedImg.fillAmount = target;
    }

}
