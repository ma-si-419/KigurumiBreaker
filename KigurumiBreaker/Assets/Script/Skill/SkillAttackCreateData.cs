using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttackCreateData : MonoBehaviour
{
    [SerializeField] private SkillAttackDataList SkillDataList;
    public List<SkillAttackData> skillDataList => SkillDataList.attackDataList;
}
