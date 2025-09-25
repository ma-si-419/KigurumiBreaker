using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private SkillData skillData; 

    LowAttackSkillData GetLowAttackSkillData(string skillName)
    {
        // skillData‚©‚çŠY“–‚·‚éƒXƒLƒ‹–¼‚ÌLowAttackSkillData‚ð’T‚·
        foreach (LowAttackSkillData skill in skillData.lowAttackSkill.lowAttackSkillDataList)
        {
            if (skill.skillName == skillName)
            {
                return skill;
            }
        }

        // Œ©‚Â‚©‚ç‚È‚©‚Á‚½‚çnull‚ð•Ô‚·
        return null;
    }

    ChargeAttackSkillData GetChargeAttackSkillData(string skillName)
    {
        // skillData‚©‚çŠY“–‚·‚éƒXƒLƒ‹–¼‚ÌChargeAttackSkillData‚ð’T‚·
        foreach (ChargeAttackSkillData skill in skillData.chargeAttackSkill.chargeAttackSkillDataList)
        {
            if (skill.skillName == skillName)
            {
                return skill;
            }
        }
        // Œ©‚Â‚©‚ç‚È‚©‚Á‚½‚çnull‚ð•Ô‚·
        return null;
    }

    SpecialChargeSkillData GetSpecialChargeSkillData(string skillName)
    {
        // skillData‚©‚çŠY“–‚·‚éƒXƒLƒ‹–¼‚ÌSpecialChargeSkillData‚ð’T‚·
        foreach (SpecialChargeSkillData skill in skillData.specialChargeSkill.specialChargeSkillDataList)
        {
            if (skill.skillName == skillName)
            {
                return skill;
            }
        }
        // Œ©‚Â‚©‚ç‚È‚©‚Á‚½‚çnull‚ð•Ô‚·
        return null;
    }

    RangedAttackSkillData GetRangedAttackSkillData(string skillName)
    {
        // skillData‚©‚çŠY“–‚·‚éƒXƒLƒ‹–¼‚ÌRangedAttackSkillData‚ð’T‚·
        foreach (RangedAttackSkillData skill in skillData.rangedAttackSkill.rangedAttackSkillDataList)
        {
            if (skill.skillName == skillName)
            {
                return skill;
            }
        }
        // Œ©‚Â‚©‚ç‚È‚©‚Á‚½‚çnull‚ð•Ô‚·
        return null;
    }

    DashSkillData GetDashSkillData(string skillName)
    {
        // skillData‚©‚çŠY“–‚·‚éƒXƒLƒ‹–¼‚ÌDashSkillData‚ð’T‚·
        foreach (DashSkillData skill in skillData.dashSkill.dashSkillDataList)
        {
            if (skill.skillName == skillName)
            {
                return skill;
            }
        }
        // Œ©‚Â‚©‚ç‚È‚©‚Á‚½‚çnull‚ð•Ô‚·
        return null;
    }
}
