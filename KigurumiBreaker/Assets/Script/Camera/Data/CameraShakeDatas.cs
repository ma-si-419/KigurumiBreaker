using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraShakeData")]

[System.Serializable]
public class CameraShakeData : ScriptableObject
{
    [Header("Žã—h‚ê‚ÌŽžŠÔ")]
    [SerializeField] private int LowTime;
    [Header("Žã—h‚ê‚Ì‘å‚«‚³")]
    [SerializeField] private float LowPower;
    [Header("’†—h‚ê‚ÌŽžŠÔ")]
    [SerializeField] private int MiddleTime;
    [Header("’†—h‚ê‚Ì‘å‚«‚³")]
    [SerializeField] private float MiddlePower;
    [Header("‹­—h‚ê‚ÌŽžŠÔ")]
    [SerializeField] private int HighTime;
    [Header("‹­—h‚ê‚Ì‘å‚«‚³")]
    [SerializeField] private float HighPower;


    public int lowTime => LowTime;
    public float lowPower => LowPower;
    public int middleTime => MiddleTime;
    public float middlePower => MiddlePower;
    public int highTime => HighTime;
    public float highPower => HighPower;
}
