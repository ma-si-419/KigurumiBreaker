using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "Camera/SpecialAttackCameraMoveData")]

[System.Serializable]
public class SpecialAttackCameraMoveData : ScriptableObject
{
    [Header("何フレームまでに距離を離すか")]
    [SerializeField] public int MoveFrame;
    [Header("プレイヤーとの距離リスト")]
    [SerializeField] public List<float> PlayerDistance;

    [Header("必殺技終了後の元の位置に戻るまでのフレーム数")]
    [SerializeField] private int ReturnFrame; // 必殺技終了後の元の位置に戻るまでのフレーム数

    public int moveFrame => MoveFrame;
    public List<float> playerDistance => PlayerDistance;
    public int returnFrame => ReturnFrame;

}
