using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUiController : MonoBehaviour
{
    public struct DamageUiData
    {
        public float lifeTime;
        public float startScale;
        public float endScale;
    }

    private DamageUiData _damageUiData;

    private float _frameCount = 0;

    private void Awake()
    {
        // 初期化処理
        this.transform.transform.localScale = Vector3.one * _damageUiData.startScale;
        // 子オブジェクトのアンカー座標を自身の位置に設定
        foreach (Transform child in this.transform)
        {
            RectTransform rectTransform = child.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _frameCount++;

        // サイズをlerpで変更
        float t = _frameCount / _damageUiData.lifeTime;

        float scale = Mathf.Lerp(_damageUiData.startScale, _damageUiData.endScale, t);

        this.transform.localScale = Vector3.one * scale;

        // ライフタイムを超えたら削除
        if (_frameCount > _damageUiData.lifeTime)
        {
            Destroy(this.gameObject);
            return;
        }

    }

    public void SetData(DamageUiData data)
    {
        _damageUiData = data;
    }
}
