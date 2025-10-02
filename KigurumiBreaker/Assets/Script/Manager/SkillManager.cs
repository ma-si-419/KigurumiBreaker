using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private SkillData skillData;

    [SerializeField] private GameObject _player;

    [Header("通常攻撃のスキルの名前")]
    [SerializeField] private string lowAttackSkillName;

    [Header("チャージ攻撃のスキルの名前")]
    [SerializeField] private string chargeAttackSkillName;

    [Header("特殊チャージ攻撃のスキルの名前")]
    [SerializeField] private string specialChargeSkillName;

    [Header("遠距離攻撃のスキルの名前")]
    [SerializeField] private string rangedAttackSkillName;

    [Header("ダッシュのスキルの名前")]
    [SerializeField] private string dashSkillName;

    [Header("パッシブスキルのリスト")]
    [SerializeField] private List<string> passiveSkillNameList = new List<string>();

    private string _lastLowAttackSkillName = "empty";
    private string _lastChargeAttackSkillName = "empty";
    private string _lastSpecialChargeSkillName = "empty";
    private string _lastRangedAttackSkillName = "empty";
    private string _lastDashSkillName = "empty";


    private void FixedUpdate()
    {
        PlayerState playerState = _player.GetComponent<PlayerState>();

        // 前フレームから名前が変わっていたらプレイヤーにスキルを入れる
        if (lowAttackSkillName != _lastLowAttackSkillName)
        {
            playerState.SetLowAttackSkill(GetLowAttackSkillData(lowAttackSkillName));
        }
        if(chargeAttackSkillName != _lastChargeAttackSkillName)
        {
            playerState.SetChargeAttackSkill(GetChargeAttackSkillData(chargeAttackSkillName));
        }
        if(specialChargeSkillName != _lastSpecialChargeSkillName)
        {
            playerState.SetSpecialChargeSkill(GetSpecialChargeSkillData(specialChargeSkillName));
        }
        if(rangedAttackSkillName != _lastRangedAttackSkillName)
        {
            playerState.SetRangedAttackSkill(GetRangedAttackSkillData(rangedAttackSkillName));
        }
        if(dashSkillName != _lastDashSkillName)
        {
            playerState.SetDashSkill(GetDashSkillData(dashSkillName));
        }



        _lastLowAttackSkillName = lowAttackSkillName;
        _lastChargeAttackSkillName = chargeAttackSkillName;
        _lastSpecialChargeSkillName = specialChargeSkillName;
        _lastRangedAttackSkillName = rangedAttackSkillName;
        _lastDashSkillName = dashSkillName;
    }

    LowAttackSkillData GetLowAttackSkillData(string skillName)
    {
        // skillDataから該当するスキル名のLowAttackSkillDataを探す
        foreach (LowAttackSkillData skill in skillData.lowAttackSkill.lowAttackSkillDataList)
        {
            if (skill.skillName == skillName)
            {
                return skill;
            }
        }

        // 見つからなかったらnullを返す
        return null;
    }

    ChargeAttackSkillData GetChargeAttackSkillData(string skillName)
    {
        // skillDataから該当するスキル名のChargeAttackSkillDataを探す
        foreach (ChargeAttackSkillData skill in skillData.chargeAttackSkill.chargeAttackSkillDataList)
        {
            if (skill.skillName == skillName)
            {
                return skill;
            }
        }
        // 見つからなかったらnullを返す
        return null;
    }

    SpecialChargeSkillData GetSpecialChargeSkillData(string skillName)
    {
        // skillDataから該当するスキル名のSpecialChargeSkillDataを探す
        foreach (SpecialChargeSkillData skill in skillData.specialChargeSkill.specialChargeSkillDataList)
        {
            if (skill.skillName == skillName)
            {
                return skill;
            }
        }
        // 見つからなかったらnullを返す
        return null;
    }

    RangedAttackSkillData GetRangedAttackSkillData(string skillName)
    {
        // skillDataから該当するスキル名のRangedAttackSkillDataを探す
        foreach (RangedAttackSkillData skill in skillData.rangedAttackSkill.rangedAttackSkillDataList)
        {
            if (skill.skillName == skillName)
            {
                return skill;
            }
        }
        // 見つからなかったらnullを返す
        return null;
    }

    DashSkillData GetDashSkillData(string skillName)
    {
        // skillDataから該当するスキル名のDashSkillDataを探す
        foreach (DashSkillData skill in skillData.dashSkill.dashSkillDataList)
        {
            if (skill.skillName == skillName)
            {
                return skill;
            }
        }
        // 見つからなかったらnullを返す
        return null;
    }

    PassiveSkillData GetPassiveSkillData(string skillName)
    {
        // skillDataから該当するスキル名のPassiveSkillDataを探す
        foreach (PassiveSkillData skill in skillData.passiveSkill.passiveSkillDataList)
        {
            if (skill.skillName == skillName)
            {
                return skill;
            }
        }
        // 見つからなかったらnullを返す
        return null;
    }
}
