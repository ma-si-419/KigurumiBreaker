using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private SkillData skillData;

    [SerializeField] private GameObject _player;

    [Header("通常攻撃のスキルの名前")]
    [SerializeField] private string _lowAttackSkillName;

    [Header("チャージ攻撃のスキルの名前")]
    [SerializeField] private string _chargeAttackSkillName;

    [Header("特殊チャージ攻撃のスキルの名前")]
    [SerializeField] private string _specialChargeSkillName;

    [Header("遠距離攻撃のスキルの名前")]
    [SerializeField] private string _rangedAttackSkillName;

    [Header("ダッシュのスキルの名前")]
    [SerializeField] private string _dashSkillName;

    [Header("パッシブスキルのリスト")]
    [SerializeField] private List<string> _passiveSkillNameList = new List<string>();

    // パッシブスキルの追加したり削除したりした際にtrueにする
    bool _isChangePassiveSkill = false;

    // 前フレームのスキル名を保存する変数
    private string _lastLowAttackSkillName = "empty";
    private string _lastChargeAttackSkillName = "empty";
    private string _lastSpecialChargeSkillName = "empty";
    private string _lastRangedAttackSkillName = "empty";
    private string _lastDashSkillName = "empty";

    PlayerState _playerState;

    private void Start()
    {
        _playerState = _player.GetComponent<PlayerState>();
    }

    private void FixedUpdate()
    {
        // 前フレームから名前が変わっていたらプレイヤーにスキルを入れる

        // 通常攻撃のスキル
        if (_lowAttackSkillName != _lastLowAttackSkillName)
        {
            // 通常攻撃のスキルをセットする
            _playerState.SetLowAttackSkill(GetLowAttackSkillData(_lowAttackSkillName));
        }

        // チャージ攻撃のスキル
        if (_chargeAttackSkillName != _lastChargeAttackSkillName)
        {
            // チャージ攻撃のスキルをセットする
            _playerState.SetChargeAttackSkill(GetChargeAttackSkillData(_chargeAttackSkillName));
        }

        // 特殊チャージ攻撃のスキル
        if (_specialChargeSkillName != _lastSpecialChargeSkillName)
        {
            // 特殊チャージ攻撃のスキルをセットする
            _playerState.SetSpecialChargeSkill(GetSpecialChargeSkillData(_specialChargeSkillName));
        }

        // 遠距離攻撃のスキル
        if (_rangedAttackSkillName != _lastRangedAttackSkillName)
        {
            // 遠距離攻撃のスキルをセットする
            _playerState.SetRangedAttackSkill(GetRangedAttackSkillData(_rangedAttackSkillName));
        }

        // ダッシュのスキル
        if (_dashSkillName != _lastDashSkillName)
        {
            // ダッシュのスキルをセットする
            _playerState.SetDashSkill(GetDashSkillData(_dashSkillName));
        }

        // パッシブスキルの数が変更されていたらプレイヤーにスキルを入れる
        if (_isChangePassiveSkill)
        {
            // パッシブスキルをセットする
            List<PassiveSkillData> passiveSkillDataList = new List<PassiveSkillData>();

            List<string> deleteNameList = new List<string>();

            // すべてのパッシブスキルを取得してリストに追加する
            foreach (string skillName in _passiveSkillNameList)
            {
                // スキル名からパッシブスキルを取得する
                PassiveSkillData skillData = GetPassiveSkillData(skillName);

                // skillDataがnullでなければリストに追加する
                if (skillData != null)
                {
                    passiveSkillDataList.Add(skillData);
                }
                // nullであればスキルの名前を削除する
                else
                {
                    deleteNameList.Add(skillName);
                }
            }
            _playerState.SetPassiveSkills(passiveSkillDataList);
            // パッシブスキルの数が変更されたことを保存する
            _isChangePassiveSkill = false;


            // スキル名のリストから削除する
            foreach (string skillName in deleteNameList)
            {
                _passiveSkillNameList.Remove(skillName);
            }

        }

        // 名前を更新する
        _lastLowAttackSkillName = _lowAttackSkillName;
        _lastChargeAttackSkillName = _chargeAttackSkillName;
        _lastSpecialChargeSkillName = _specialChargeSkillName;
        _lastRangedAttackSkillName = _rangedAttackSkillName;
        _lastDashSkillName = _dashSkillName;
    }

    public void AddPassiveSkill(string skillName)
    {
        // 同じ名前のパッシブスキルがすでにある場合は追加しない
        if (_passiveSkillNameList.Contains(skillName))
        {
            return;
        }

        // パッシブスキルを追加する
        _passiveSkillNameList.Add(skillName);

        // パッシブスキルの数が変更されたことを保存する
        _isChangePassiveSkill = true;
    }

    public void SubPassiveSkill(string skill)
    {
        // パッシブスキルを削除する
        _passiveSkillNameList.Remove(skill);
        // パッシブスキルの数が変更されたことを保存する
        _isChangePassiveSkill = true;
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
