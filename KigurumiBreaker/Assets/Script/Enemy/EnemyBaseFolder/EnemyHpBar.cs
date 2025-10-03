using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBar : MonoBehaviour
{
    
    [SerializeField] private Image _fillImage;   // HPバーのImageコンポーネント
    private RectTransform _rectTransform;

    // HPバーの表示更新
    private void Awake()
    {
        // 取得
        _rectTransform = GetComponent<RectTransform>();
        // チェック
        if (_fillImage == null) Debug.LogError("_fillImageはEnemyHpBarに割り当てられていません。");
    }

    public void SetFillRatio(float ratio)
    {
        // HPバーの表示更新
        if (_fillImage != null) _fillImage.fillAmount = Mathf.Clamp01(ratio);
    }

    // RectTransformの公開
    public RectTransform RectTransform => _rectTransform;

    // HPバーの表示/非表示
    public void Show(bool visible)
    {
        gameObject.SetActive(visible);
    }

}
