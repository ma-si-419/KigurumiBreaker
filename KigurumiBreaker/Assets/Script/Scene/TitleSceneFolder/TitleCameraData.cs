using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TitleSceneFolder/TitleCameraData")]

[System.Serializable]

public class TitleCameraData : ScriptableObject
{

    [Header("カメラの座標")]
    [SerializeField] private Vector3 CameraPosition;
    [Header("カメラの回転")]
    [SerializeField] private Vector3 CameraRotation;
    [Header("注視する高さ")]
    [SerializeField] private float LookHeight;
    [Header("カメラと注視点の距離")]
    [SerializeField] private float Distance;
    [Header("カメラの回転速度")]
    [SerializeField] private float RotationSpeed;
    [Header("カメラの移動速度")]
    [SerializeField] private float MoveSpeed;


    public Vector3 cameraPosition => CameraPosition;
    public Vector3 cameraRotation => CameraRotation;
    public float lookHeight => LookHeight;
    public float distance => Distance;
    public float rotationSpeed => RotationSpeed;
    public float moveSpeed => MoveSpeed;

}
