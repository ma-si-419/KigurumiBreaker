using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DamageData")]
public class DamageData : ScriptableObject
{
    [Header("��U�����󂯂����̍d������")]
    [SerializeField] private int LowStanTime;
    [Header("���U�����󂯂����̍d������")]
    [SerializeField] private int MiddleStanTime;
    [Header("���U�����󂯂����̃m�b�N�o�b�N�̑傫��")]
    [SerializeField] private int MiddleKnockBackScale;
    [Header("���U�����󂯂����Ƀm�b�N�o�b�N���鎞��")]
    [SerializeField] private int MiddleKnockBackTime;
    [Header("���U�����󂯂����̍d������")]
    [SerializeField] private int HighStanTime;
    [Header("���U�����󂯂����̃m�b�N�o�b�N�̑傫��")]
    [SerializeField] private int HighKnockBackScale;
    [Header("���U�����󂯂����Ƀm�b�N�o�b�N���鎞��")]
    [SerializeField] private int HighKnockBackTime;

    // �ǂݎ���p
    public int lowStanTime => LowStanTime;
    public int middleStanTime => MiddleStanTime;
    public int middleKnockBackScale => MiddleKnockBackScale;
    public int middleKnockBackTime => MiddleKnockBackTime;
    public int highStanTime => HighStanTime;
    public int highKnockBackScale => HighKnockBackScale;
    public int highKnockBackTime => HighKnockBackTime;
}
