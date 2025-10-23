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
    [Header("点滅速度")]
    [SerializeField] private float FlashSpeed = 5f;
    [Header("オーラの回転速度")]
    [SerializeField] private float AuraRotateSpeed = 50f;

    // 読み取り専用
    public Color normalColor => NormalColor;
    public Color maxColor => MaxColor;
    public float flashSpeed => FlashSpeed;
    public float auraRotateSpeed => AuraRotateSpeed;
}
