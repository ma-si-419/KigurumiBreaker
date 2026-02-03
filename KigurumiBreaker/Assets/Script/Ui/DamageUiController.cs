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

    public enum UiKind
    {
        EnemyDamage,
        PlayerDamage,
        Heal,
        KindNum
    }

    private List<RectTransform> _uis = new List<RectTransform>();

    private DamageUiData _damageUiData;

    private float _frameCount = 0;

    private void Awake()
    {
        // 初期化処理
        this.transform.transform.localScale = Vector3.one * _damageUiData.startScale;

        // 子オブジェクトのアンカー座標を自身の位置に設定
        foreach (Transform child in this.transform)
        {
            _uis.Add(child.GetComponent<RectTransform>());
            child.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        // UIの数によってピポッドの座標を設定
        if (_uis.Count == 1)
        {
            _uis[0].pivot = new Vector2(0.5f, 0.5f);
        }
        else if (_uis.Count == 2)
        {
            _uis[0].pivot = new Vector2(1.0f, 0.5f);
            _uis[1].pivot = new Vector2(0.0f, 0.5f);
        }
        else if (_uis.Count == 3)
        {
            _uis[0].pivot = new Vector2(1.0f, 0.5f);
            _uis[1].pivot = new Vector2(0.5f, 0.5f);
            _uis[2].pivot = new Vector2(0.0f, 0.5f);
        }
        else if (_uis.Count == 4)
        {
            _uis[0].pivot = new Vector2(1.0f, 0.5f);
            _uis[1].pivot = new Vector2(0.7f, 0.5f);
            _uis[2].pivot = new Vector2(0.3f, 0.5f);
            _uis[3].pivot = new Vector2(0.0f, 0.5f);
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

        // スケールの設定
        foreach (RectTransform ui in _uis)
        {
            ui.localScale = Vector3.one * scale;
        }

        // ZのScaleは1に固定
        this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, 1f);

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
