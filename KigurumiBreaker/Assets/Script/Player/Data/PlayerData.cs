using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("移動入力感知の閾値")]
    [SerializeField] private float MoveInputLength;
    [Header("移動時のサウンドを鳴らす間隔")]
    [SerializeField] private int MoveSoundInterval;
    [Header("1フレームで回転することができる角度")]
    [SerializeField] private float RotateAngle;
    [Header("回避で移動し始めるまでの時間")]
    [SerializeField] private float DodgeStartTime;
    [Header("回避時間")]
    [SerializeField] private int DodgeTime;
    [Header("回避で減速にかける時間")]
    [SerializeField] private int DodgeStopTime;
    [Header("回避クールタイム")]
    [SerializeField] private int DodgeCoolTime;
    [Header("連続回避の入力を受け付けない時間")]
    [SerializeField] private int CancelDodgeCoolTime;
    [Header("ため攻撃の発動までの時間")]
    [SerializeField] private int ChargeAttackTime;
    [Header("強ため攻撃に移行するまでの時間")]
    [SerializeField] private int HighChargeAttackTime;
    [Header("ため攻撃の最大溜め時間")]
    [SerializeField] private int MaxChargeAttackTime;
    [Header("ため攻撃中の回転速度")]
    [SerializeField] private float ChargeTurnSpeed;
    [Header("ため中に揺らす大きさ")]
    [SerializeField] private float ChargeShakeScale;
    [Header("移動ベクトルの回転度")]
    [SerializeField] private float MoveDirAngle;
    [Header("特殊攻撃の最大のチャージ量")]
    [SerializeField] private float MaxSpecialChargeGauge;
    [Header("特殊攻撃の段階数")]
    [SerializeField] private int SpecialAttackMaxLevel;
    [Header("敵のダメージの何割を特殊攻撃のゲージに加算するか")]
    [SerializeField, Range(0.0f, 2.0f)] private float SpecialAttackChargeRate;
    [Header("前方向とする角度")]
    [SerializeField] private float ForwardAngle;
    [Header("前方向の敵を認知する距離")]
    [SerializeField] private float ForwardDistance;
    [Header("チャージエフェクトを出すときどのくらいカメラに近づけるか")]
    [SerializeField] private float ChargeEffectShiftScale;
    [Header("チャージ攻撃で攻撃する部位を拡大するときにかける時間")]
    [SerializeField] private float ChargeAttackPartScaleUpTime;
    [Header("回避や被弾時等に拡大していた部位を1フレームで縮小する大きさ")]
    [SerializeField] private float ChargeAttackPartScaleDownRatePerFrame;
    [Header("死亡時のスロー時間")]
    [SerializeField] private float DeathSlowTime;
    [Header("死亡時のスローのタイムスケール")]
    [SerializeField] private float DeathTimeScale;
    [Header("死亡時のスロー演出が終わって何フレーム後にエフェクトを出すか")]
    [SerializeField] private int DeathEffectTime;

    // 読み取り専用
    public float moveInputLength => MoveInputLength;
    public int moveSoundInterval => MoveSoundInterval;
    public float rotateAngle => RotateAngle;
    public float dodgeStartTime => DodgeStartTime;
    public int dodgeTime => DodgeTime;
    public int dodgeStopTime => DodgeStopTime;
    public int cancelDodgeCooldown => CancelDodgeCoolTime;
    public int dodgeCoolTime => DodgeCoolTime;
    public int chargeAttackTime => ChargeAttackTime;
    public int highChargeAttackTime => HighChargeAttackTime;
    public int maxChargeAttackTime => MaxChargeAttackTime;
    public int specialAttackMaxLevel => SpecialAttackMaxLevel;
    public float specialAttackChargeRate => SpecialAttackChargeRate;
    public float chargeTurnSpeed => ChargeTurnSpeed;
    public float chargeShakeScale => ChargeShakeScale;
    public float moveDirAngle => MoveDirAngle;
    public float maxSpecialChargeNum => MaxSpecialChargeGauge;
    public float forwardAngle => ForwardAngle;
    public float forwardDistance => ForwardDistance;
    public float chargeEffectShiftScale => ChargeEffectShiftScale;
    public float chargeAttackPartScaleUpTime => ChargeAttackPartScaleUpTime;
    public float chargeAttackPartScaleDownRatePerFrame => ChargeAttackPartScaleDownRatePerFrame;
    public float deathSlowTime => DeathSlowTime;
    public float deathTimeScale => DeathTimeScale;
    public int deathEffectTime => DeathEffectTime;
}
