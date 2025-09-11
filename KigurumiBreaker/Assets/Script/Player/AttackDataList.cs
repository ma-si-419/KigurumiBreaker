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
    public string attackName;
    public int damage;
    public int startFrame;
    public int cancelFrame;
    public int totalFrame;
    public float scale;
    public int attackLifeTime;
    public string nextAttackName;
}
