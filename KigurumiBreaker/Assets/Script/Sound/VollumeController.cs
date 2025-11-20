using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VolumeController : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    [Header("Initial Select")]
    [SerializeField] private GameObject firstSelected;  // ← 最初の選択UI

    [Header("Long Press Settings")]
    [SerializeField] private float repeatDelay = 0.4f;  // 最初の長押しまでの時間
    [SerializeField] private float repeatRate = 0.12f;  // 連続で動き続ける間隔（小さいほど速い）

    private readonly float[] steps = { 0f, 0.25f, 0.5f, 0.75f, 1f };

    private float holdTimer = 0f;
    private bool isHolding = false;
    private float lastInput = 0f;

    private void Start()
    {
        // 最初の選択UIを指定
        if (firstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }

    private void Update()
    {
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        Slider activeSlider = selected.GetComponent<Slider>();
        if (activeSlider == null) return;

        // 入力取得（十字キー or スティック対応）
        float input = Input.GetAxisRaw("Horizontal");

        // 左右入力が変わったら即1段階動かす
        if (Mathf.Sign(input) != Mathf.Sign(lastInput) || Mathf.Abs(input) < 0.5f)
        {
            isHolding = false;
            holdTimer = 0f;
        }

        if (Mathf.Abs(input) > 0.5f)
        {
            if (!isHolding)
            {
                // 最初の1回は即反応
                MoveSlider(activeSlider, input);
                isHolding = true;
                holdTimer = Time.time + repeatDelay;
            }
            else
            {
                // 長押し時の連続動作
                if (Time.time >= holdTimer)
                {
                    MoveSlider(activeSlider, input);
                    holdTimer = Time.time + repeatRate;
                }
            }
        }

        lastInput = input;
    }

    private void MoveSlider(Slider slider, float input)
    {
        float current = GetNearestStep(slider.value);
        int index = System.Array.IndexOf(steps, current);

        if (input > 0) index++;
        if (input < 0) index--;

        index = Mathf.Clamp(index, 0, steps.Length - 1);

        slider.value = steps[index];
    }

    private float GetNearestStep(float value)
    {
        float best = steps[0];
        float diff = Mathf.Abs(value - best);

        foreach (var s in steps)
        {
            float d = Mathf.Abs(value - s);
            if (d < diff)
            {
                best = s;
                diff = d;
            }
        }
        return best;
    }
}
