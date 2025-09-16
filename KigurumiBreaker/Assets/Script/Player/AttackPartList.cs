using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackPartList")]
public class AttackPartList : ScriptableObject
{
    public List<AttackPart> attackDataList;
}
[System.Serializable]
public class AttackPart
{
    [Header("���O")]
    [SerializeField] private string AttackPartName;
    [Header("�U�����ʂ̃��O�̖��O")]
    [SerializeField] private string ObjectRigName;

    // �ǂݎ���p�v���p�e�B

    public string attackPartName => AttackPartName;
    public string objectRigName => ObjectRigName;

}
