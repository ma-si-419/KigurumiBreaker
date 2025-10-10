using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialAttackCameraMoveData")]

[System.Serializable]
public class SpecialAttackCameraMoveData : ScriptableObject
{
    [System.Serializable]
    public class MoveData
    {
        [Header("どのフレームまで動くか")]
        [SerializeField] public int MoveFrame;
        [Header("プレイヤーとの距離")]
        [SerializeField] public float PlayerDistance;
    }

    [SerializeField] private List<MoveData> SpecialMoveDatas;

    public List<MoveData> specialMoveDatas => SpecialMoveDatas;

}
