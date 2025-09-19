using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AttackDataList")]
public class AttackDataList : ScriptableObject
{
    public List<AttackData> attackDataList;
}
[System.Serializable]
public class AttackData
{
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
    [Header("攻撃があたった時に出すエフェクト")]
    [SerializeField] private GameObject HitEffect;

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
    public GameObject hitEffect => HitEffect;
}
