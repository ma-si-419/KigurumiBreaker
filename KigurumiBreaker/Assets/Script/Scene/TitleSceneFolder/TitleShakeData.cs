using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TitleSceneFolder/TitleShakeData")]

[System.Serializable]

public class TitleShakeData : ScriptableObject
{
    [Header("—h‚ê‚ÌŽžŠÔ")]
    [SerializeField] private int Time;
    [Header("—h‚ê‚Ì‹­‚³")]
    [SerializeField] private float Power;

    public int time => Time;
    public float power => Power;
}
