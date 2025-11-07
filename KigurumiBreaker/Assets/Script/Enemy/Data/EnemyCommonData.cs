using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyCommonData")]
public class EnemyCommonData : ScriptableObject
{
    [Header("ヒットストップの揺れる大きさ")]
    [SerializeField] private float ShakeMagnitude;
    [Header("バーが減少する硬直時間")]
    [SerializeField] private float DelayRigidityTime;
    [Header("遅れてくるバーの速度")]
    [SerializeField] private float LerpSpeed;
    [Header("発見マークのプレハブ")]
    [SerializeField] private GameObject DetectionMarkPrefab;
    [Header("敵全員が行動しなくなるフラグ")]
    [SerializeField] private bool IsStopAllAction;
    [Header("ドロップする弾のプレハブ")]
    [SerializeField] private GameObject _dropBullet;
    [Header("ドロップする弾をY方向にずらす大きさ")]
    [SerializeField] private float _dropPosShiftY;
    [Header("ドロップする弾を飛ばす力")]
    [SerializeField] private float _dropBulletForce;
    [Header("ドロップする弾の時間")]
    [SerializeField] private int _dropBulletTime;

    // 読み取り専用
    public float shakeMagnitude => ShakeMagnitude;
    public float delayRigidityTime => DelayRigidityTime;
    public float lerpSpeed => LerpSpeed;
    public GameObject detectionMarkPrefab => DetectionMarkPrefab;
    public bool isStopAllAction => IsStopAllAction;
    public GameObject dropBullet => _dropBullet;
    public float dropPosShiftY => _dropPosShiftY;
    public float dropBulletForce => _dropBulletForce;
    public int dropBulletTime => _dropBulletTime;
}
