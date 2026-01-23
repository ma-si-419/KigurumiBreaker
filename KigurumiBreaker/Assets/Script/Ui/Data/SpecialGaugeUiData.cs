using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/SpecialGaugeUiData")]
public class SpecialGaugeUiData : ScriptableObject
{
    [Header("たまっていないときのゲージの色")]
    [SerializeField] private Color NormalColor = Color.yellow;
    [Header("たまったときのゲージの色")]
    [SerializeField] private Color MaxColor = Color.red;
    [Header("後ろの炎の通常時の色")]
    [SerializeField] private Color AuraNormalColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
    [Header("後ろの炎の最大時の色")]
    [SerializeField] private Color AuraMaxColor = new Color(0.8f, 0f, 0f, 1.0f);


    // 読み取り専用
    public Color normalColor => NormalColor;
    public Color maxColor => MaxColor;
    public Color auraNormalColor => AuraNormalColor;
    public Color auraMaxColor => AuraMaxColor;
}
