using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "Camera/SpecialAttackCameraMoveData")]

[System.Serializable]
public class SpecialAttackCameraMoveData : ScriptableObject
{
    [Header("何フレーム目までにプレイヤーに近づくか")]
    [SerializeField] private int ApproachFrame;
    [Header("どこまでプレイヤーに最初近づくか")]
    [SerializeField] private float InitialPlayerDistance;
    [Header("何フレーム目までに距離を離すか")]
    [SerializeField] private int LeaveFrame;
    [Header("レベルごとにどこまでカメラを離すか")]
    [SerializeField] private List<float> PlayerDistance;

    [Header("必殺技終了後元の位置に戻るまでのフレーム数")]
    [SerializeField] private int ReturnFrame; 

    public int approachFrame => ApproachFrame;
    public float initialPlayerDistance => InitialPlayerDistance;
    public int leaveFrame => LeaveFrame;
    public List<float> playerDistance => PlayerDistance;
    public int returnFrame => ReturnFrame;

}
