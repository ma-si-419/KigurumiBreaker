using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AttackData")]
public class AttackData : ScriptableObject
{
    public List<Attack> attackDataList;

    public GameObject meleeAttackGameObject;    // 近接攻撃の攻撃判定プレハブ

    public GameObject chargeAttackAreaGameObject;   // 溜め攻撃の攻撃判定プレハブ

    public GameObject rangedAttackGameObject;   // 遠距離攻撃の攻撃判定プレハブ
}
[System.Serializable]
public class Attack
{
    public enum AttackType
    {
        LowAttack,      // 通常攻撃
        ChargeAttack,   // 溜め攻撃
        RangedAttack,   // 遠距離攻撃
        SpecialAttack   // 特殊攻撃
    }



    [Header("名前")]
    [SerializeField] private string AttackName;
    [Header("ダメージ係数")]
    [SerializeField] private int Damage;
    [Header("発生フレーム")]
    [SerializeField] private int StartFrame;
    [Header("硬直フレーム")]
    [SerializeField] private int StunFrame;
    [Header("キャンセルフレーム")]
    [SerializeField] private int CancelFrame;
    [Header("トータルフレーム")]
    [SerializeField] private int TotalFrame;
    [Header("前に進む速度")]
    [SerializeField] private float MoveSpeed;
    [Header("攻撃判定の大きさ")]
    [SerializeField] private float Scale;
    [Header("攻撃判定を前方向にずらす大きさ")]
    [SerializeField] private float ShiftPosZ;
    [Header("攻撃判定の持続時間")]
    [SerializeField] private int AttackLifeTime;
    [Header("次に出てくる攻撃の名前(コンボ用)")]
    [SerializeField] private string NextAttackName;
    [Header("攻撃を出す部位")]
    [SerializeField] private string AttackPart;
    [Header("攻撃を出す部位に出すエフェクト")]
    [SerializeField] private GameObject AttackEffect;
    [Header("エフェクトをプレイヤーから離す距離")]
    [SerializeField] private float EffectShiftScale;
    [Header("攻撃があたった時に出すエフェクト")]
    [SerializeField] private GameObject HitEffect;
    [Header("攻撃の種類")]
    [SerializeField] private AttackType AttackKind;
    [Header("カメラを揺らす大きさ")]
    [SerializeField] private CameraMove.ShakeKind CameraShakeKind;
    [Header("ヒットストップの長さ")]
    [SerializeField] private int HitStopFrame;

    // 読み取り専用プロパティ

    public string attackName => AttackName;
    public int damage => Damage;
    public int startFrame => StartFrame;
    public int stunFrame => StunFrame;
    public int cancelFrame => CancelFrame;
    public int totalFrame => TotalFrame;
    public float moveSpeed => MoveSpeed;
    public float scale => Scale;
    public float shiftPosZ => ShiftPosZ;
    public int attackLifeTime => AttackLifeTime;
    public string nextAttackName => NextAttackName;
    public string attackPart => AttackPart;
    public GameObject attackEffect => AttackEffect;
    public float effectShiftScale => EffectShiftScale;
    public GameObject hitEffect => HitEffect;
    public AttackType attackKind => AttackKind;
    public CameraMove.ShakeKind cameraShakeKind => CameraShakeKind;
    public int hitStopFrame => HitStopFrame;
}
