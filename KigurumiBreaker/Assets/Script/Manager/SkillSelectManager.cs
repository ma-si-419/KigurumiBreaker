using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class SkillSelectManager : MonoBehaviour
{
    private const int SKILL_SELECT_NUM = 3; // 選択できるスキルの数

    private const int SKILL_PANEL_DISTANCE = 350; // スキルパネルの間隔


    [SerializeField] private GameObject _playerSkillManager;
    [SerializeField] private SkillData _skillData;
    [SerializeField] private Canvas _skillSelectCanvas;
    [SerializeField] private GameObject _skillSelectPanel;

    private PlayerSkillManager _skillManager;

    private struct SelectSkill
    {
        public string skillName;
        public SkillData.SkillCategory skillCategory;
    }

    private string _changeSkillName = "None";
    private bool _isAdd = false;
    private bool _isSub = false;
    private bool _isSkillSelect = false;


    private void Start()
    {
        _skillManager = _playerSkillManager.GetComponent<PlayerSkillManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // デバッグ
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartSkillSelect();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            _isSkillSelect = false;
            Time.timeScale = 1f;
            foreach (Transform child in _skillSelectCanvas.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // スキル選択中以外は処理しない
        if (!_isSkillSelect) return;


        if (_isAdd)
        {
            _skillManager.AddPassiveSkill(_changeSkillName);

            _changeSkillName = "None";
            _isAdd = false;
        }
        else if (_isSub)
        {
            _skillManager.SubPassiveSkill(_changeSkillName);

            _changeSkillName = "None";
            _isSub = false;
        }
    }

    public void StartSkillSelect()
    {
        _isSkillSelect = true;

        Time.timeScale = 0f;

        List<SelectSkill> selectSkills = GetSelectSkill();

        // スキル選択パネルを生成する
        for (int i = 0; i < SKILL_SELECT_NUM; i++)
        {
            GameObject panel = Instantiate(_skillSelectPanel, _skillSelectCanvas.transform);
            SkillPanelInfo panelInfo = panel.GetComponent<SkillPanelInfo>();

            string skillName = selectSkills[i].skillName;

            switch (selectSkills[i].skillCategory)
            {
                case SkillData.SkillCategory.LowAttack:

                    foreach (var skill in _skillData.lowAttackSkill.lowAttackSkillDataList)
                    {
                        if (skill.skillName == skillName)
                        {
                            panelInfo.SetInfo(skillName, skill.skillContents);
                            break;
                        }
                    }

                    break;
                case SkillData.SkillCategory.ChargeAttack:

                    foreach (var skill in _skillData.chargeAttackSkill.chargeAttackSkillDataList)
                    {
                        if (skill.skillName == skillName)
                        {
                            panelInfo.SetInfo(skillName, skill.skillContents);
                            break;
                        }
                    }

                    break;
                case SkillData.SkillCategory.SpecialCharge:
                    foreach (var skill in _skillData.specialChargeSkill.specialChargeSkillDataList)
                    {
                        if (skill.skillName == skillName)
                        {
                            panelInfo.SetInfo(skillName, skill.skillContents);
                            break;
                        }
                    }
                    break;
                case SkillData.SkillCategory.RangedAttack:
                    foreach (var skill in _skillData.rangedAttackSkill.rangedAttackSkillDataList)
                    {
                        if (skill.skillName == skillName)
                        {
                            panelInfo.SetInfo(skillName, skill.skillContents);
                            break;
                        }
                    }
                    break;
                case SkillData.SkillCategory.Dash:
                    foreach (var skill in _skillData.dashSkill.dashSkillDataList)
                    {
                        if (skill.skillName == skillName)
                        {
                            panelInfo.SetInfo(skillName, skill.skillContents);
                            break;
                        }
                    }
                    break;
                case SkillData.SkillCategory.Passive:
                    foreach (var skill in _skillData.passiveSkill.passiveSkillDataList)
                    {
                        if (skill.skillName == skillName)
                        {
                            panelInfo.SetInfo(skillName, skill.skillContents);
                            break;
                        }
                    }
                    break;
            }

            // パネルの位置を調整する
            panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,(i - 1) * SKILL_PANEL_DISTANCE);
        }
    }

    private List<SelectSkill> GetSelectSkill()
    {

        List<SelectSkill> selectSkills = new List<SelectSkill>();

        // まずスキルの属性を選択する
        int skillElement = Random.Range(0, (int)SkillData.SkillElement.TypeNum);

        // 三つのスキルを選択する
        for (int i = 0; i < SKILL_SELECT_NUM; i++)
        {
            // 選ぶことができるスキルを選択するまでループ
            bool isSelectSkill = false;

            SelectSkill selectSkill = new SelectSkill();

            while (!isSelectSkill)
            {
                // スキルのカテゴリを選択する
                int skillCategory = Random.Range(0, (int)SkillData.SkillCategory.CategoryNum);

                switch (skillCategory)
                {
                    case (int)SkillData.SkillCategory.LowAttack:

                        // スキルの属性が一致するスキルを取得する
                        foreach (var skill in _skillData.lowAttackSkill.lowAttackSkillDataList)
                        {
                            if (skill.skillElement == (SkillData.SkillElement)skillElement)
                            {
                                selectSkill.skillName = skill.skillName;
                                selectSkill.skillCategory = SkillData.SkillCategory.LowAttack;
                                isSelectSkill = true;
                                break;
                            }
                        }

                        break;
                    case (int)SkillData.SkillCategory.ChargeAttack:

                        // スキルの属性が一致するスキルを取得する
                        foreach (var skill in _skillData.chargeAttackSkill.chargeAttackSkillDataList)
                        {
                            if (skill.skillElement == (SkillData.SkillElement)skillElement)
                            {
                                selectSkill.skillName = skill.skillName;
                                selectSkill.skillCategory = SkillData.SkillCategory.ChargeAttack;
                                isSelectSkill = true;
                                break;
                            }
                        }

                        break;
                    case (int)SkillData.SkillCategory.SpecialCharge:
                        // スキルの属性が一致するスキルを取得する
                        foreach (var skill in _skillData.specialChargeSkill.specialChargeSkillDataList)
                        {
                            if (skill.skillElement == (SkillData.SkillElement)skillElement)
                            {
                                selectSkill.skillName = skill.skillName;
                                selectSkill.skillCategory = SkillData.SkillCategory.SpecialCharge;
                                isSelectSkill = true;
                                break;
                            }
                        }
                        break;
                    case (int)SkillData.SkillCategory.RangedAttack:
                        // スキルの属性が一致するスキルを取得する
                        foreach (var skill in _skillData.rangedAttackSkill.rangedAttackSkillDataList)
                        {
                            if (skill.skillElement == (SkillData.SkillElement)skillElement)
                            {
                                selectSkill.skillName = skill.skillName;
                                selectSkill.skillCategory = SkillData.SkillCategory.RangedAttack;
                                isSelectSkill = true;
                                break;
                            }
                        }
                        break;
                    case (int)SkillData.SkillCategory.Dash:
                        // スキルの属性が一致するスキルを取得する
                        foreach (var skill in _skillData.dashSkill.dashSkillDataList)
                        {
                            if (skill.skillElement == (SkillData.SkillElement)skillElement)
                            {
                                selectSkill.skillName = skill.skillName;
                                selectSkill.skillCategory = SkillData.SkillCategory.Dash;
                                isSelectSkill = true;
                                break;
                            }
                        }
                        break;
                    case (int)SkillData.SkillCategory.Passive:

                        // パッシブスキルは同じ属性がいくつかあるので選択した属性のパッシブスキルをリストアップする
                        List<int> index = new List<int>();
                        for (int j = 0; j < _skillData.passiveSkill.passiveSkillDataList.Count; j++)
                        {
                            if (_skillData.passiveSkill.passiveSkillDataList[j].skillElement == (SkillData.SkillElement)skillElement)
                            {
                                index.Add(j);
                            }
                        }

                        // もし一つもなければもう一度スキルカテゴリを選択し直す
                        if (index.Count == 0) break;

                        // 選択した属性のパッシブスキルの中からランダムに選択する
                        int passiveIndex = index[Random.Range(0, index.Count)];
                        selectSkill.skillName = _skillData.passiveSkill.passiveSkillDataList[passiveIndex].skillName;
                        selectSkill.skillCategory = SkillData.SkillCategory.Passive;
                        isSelectSkill = true;

                        break;
                }

                // スキルが選択されていなければもう一度スキルカテゴリを選択し直す
                if (!isSelectSkill) continue;

                // 既に選択されているスキルと被っていないか確認する
                bool isCheck = true;
                foreach (var skill in selectSkills)
                {
                    if (skill.skillName == selectSkill.skillName &&
                        skill.skillCategory == selectSkill.skillCategory)
                    {
                        isCheck = false;
                        break;
                    }
                }

                if (isCheck)
                {
                    // 被っていなければ選択したスキルをリストに追加する
                    selectSkills.Add(selectSkill);
                    isSelectSkill = true;
                }
                else
                {
                    // 被っていれば再度スキルを選択する
                    isSelectSkill = false;
                }
            }
        }

        return selectSkills;
    }

}
