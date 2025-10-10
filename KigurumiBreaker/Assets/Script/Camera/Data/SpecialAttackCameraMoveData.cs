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

    [Header("必殺技中のカメラ移動データ")]
    [SerializeField] private List<MoveData> SpecialMoveDatas;

    [Header("必殺技終了後の元の位置に戻るまでのフレーム数")]
    [SerializeField] private int ReturnFrame; // 必殺技終了後の元の位置に戻るまでのフレーム数

    public List<MoveData> specialMoveDatas => SpecialMoveDatas;
    public int returnFrame => ReturnFrame;

}
