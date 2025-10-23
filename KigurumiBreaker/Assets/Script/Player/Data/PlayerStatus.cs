using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerStatus")]
public class PlayerStatus : ScriptableObject
{
    [Header("Å‘å‘Ì—Í")]
    [SerializeField] private int MaxHp;
    [Header("ˆÚ“®‘¬“x")]
    [SerializeField] private float MoveSpeed;
    [Header("‰ñ”ð‘¬“x")]
    [SerializeField] private float DodgeSpeed;
    [Header("UŒ‚—Í")]
    [SerializeField] private float AttackPower;
    [Header("‰“‹——£‚Ì’e‚ÌÅ‘å’l")]
    [SerializeField] private int MaxBulletNum;

    // “Ç‚ÝŽæ‚èê—p
    public int maxHp => MaxHp;
    public float moveSpeed => MoveSpeed;
    public float dodgeSpeed => DodgeSpeed;
    public float attackPower => AttackPower;
    public int maxBulletNum => MaxBulletNum;

}
