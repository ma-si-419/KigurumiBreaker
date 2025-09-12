using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    public float MOVE_SPEED;
    public float DODGE_SPEED;
    public float MOVE_INPUT_LENGTH;
    public int DODGE_TIME;
    public int DOGDE_COOLTIME;
    public int MAX_HEALTH;
    public int CHARGE_ATTACK_MIN_TIME;
}
