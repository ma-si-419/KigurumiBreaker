using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Camera/CameraData")]

[System.Serializable]
public class CameraData : ScriptableObject
{
    [Header("カメラの座標(プレイヤーのローカル)")]
    [SerializeField] private Vector3 CameraPosition;
    [Header("カメラの回転")]
    [SerializeField] private Vector3 CameraRotation;

    public Vector3 cameraPosition =>CameraPosition;
    public Vector3 cameraRotation =>CameraRotation;
}

