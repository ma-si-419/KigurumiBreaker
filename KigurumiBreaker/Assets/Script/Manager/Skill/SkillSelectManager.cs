using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class SkillSelectManager : MonoBehaviour
{
    private const int SKILL_SELECT_NUM = 3;                                     // 選択できるスキルの数

    private const int SKILL_PANEL_DISTANCE = 350;                               // スキルパネルの間隔

    private const int SKill_CURSOR_PANEL_DISTANCE = 830;                        //(後々消す定数) 

    [SerializeField] private GameObject _playerSkillManagerObject;              // PlayerSkillManagerのオブジェクト
    [SerializeField] private SkillData _skillData;                              // スキルデータ
    [SerializeField] private Canvas _skillSelectCanvas;                         // スキル選択のUIを表示するキャンバス
    [SerializeField] private GameObject _skillSelectPanel;                      // スキル選択用のパネル
    [SerializeField] private GameObject _skillGetObject;                        // 取得したらスキル選択を開始するオブジェクト
    [SerializeField] private GameObject _skillSelectCursor;                     // スキル選択カーソル(後々消す予定)
    [SerializeField] private GameObject _player;                                // プレイヤーのオブジェクト

    private PlayerSkillManager _playerSkillManager;                             // PlayerSkillManagerのスクリプト

    // 生成したカーソルのゲームオブジェクト
    GameObject _cursorObj;

    private struct SelectSkill
    {
        public string skillName;
        public SkillData.SkillCategory skillCategory;
    }

    private List<SelectSkill> _selectSkills;

    private bool _isSkillSelect = false;

    private bool _isMoveCursor = false;
    private int _cursorIndex = 0; // 現在のカーソル位置

    private WaveSpawner _waveSpawner; // WaveSpawnerのスクリプト

    /*

    // 削除が出てきたら使用する可能性あり
    private bool _isAdd = false;
    private bool _isSub = false;
    private string _changeSkillName = "None";

    */

    private void Start()
    {
        _playerSkillManager = _playerSkillManagerObject.GetComponent<PlayerSkillManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // スキル選択中以外は処理しない
        if (!_isSkillSelect) return;

        int lastCursorIndex = _cursorIndex;

        // デバッグ用　
        // 上下キーで選択、Aボタンで決定

        float inputV = Input.GetAxis("PadV");

        // 上キーが押されたら選択を一つ上に移動
        if (inputV > 0.5f)
        {
            if (!_isMoveCursor)
            {
                // カーソル移動フラグを立てる
                _isMoveCursor = true;
                // カーソル位置を更新
                _cursorIndex++;

                // もしカーソル位置が範囲外なら一番上に戻す
                if (_cursorIndex > SKILL_SELECT_NUM - 1)
                {
                    _cursorIndex = 0;
                }
            }
        }
        // 下キーが押されたら選択を一つ下に移動
        else if (inputV < -0.5f)
        {
            if (!_isMoveCursor)
            {
                // カーソル位置を更新
                _cursorIndex--;

                // もしカーソル位置が範囲外なら一番下に戻す
                if (_cursorIndex < 0)
                {
                    _cursorIndex = SKILL_SELECT_NUM - 1;
                }
                // カーソル移動フラグを立てる
                _isMoveCursor = true;
            }
        }
        else
        {
            _isMoveCursor = false;
        }

        // カーソル位置が変わっていたらカーソルの位置を更新する
        if (lastCursorIndex != _cursorIndex)
        {
            Vector2 cursorPos = new Vector2(-SKill_CURSOR_PANEL_DISTANCE, (_cursorIndex * SKILL_PANEL_DISTANCE) - SKILL_PANEL_DISTANCE);
            _cursorObj.GetComponent<RectTransform>().anchoredPosition = cursorPos;
        }

        // 決定が押されたら選択したスキルをプレイヤーにセットする
        if (Input.GetButtonDown("OK"))
        {
            SelectSkill setSkill = _selectSkills[_cursorIndex];

            _playerSkillManager.SetSkillName(setSkill.skillCategory, setSkill.skillName);

            // スキル選択パネルとカーソルを削除する
            foreach (Transform child in _skillSelectCanvas.transform)
            {
                Destroy(child.gameObject);
            }

            // スキル選択を終了する
            _isSkillSelect = false;

            // プレイヤーのStateの更新を再開する
            _player.GetComponent<PlayerState>().SetStateUpdateFlag(false);

            // WaveSpawnerがセットされていれば終了通知を送る
            if (_waveSpawner != null)
            {
                _waveSpawner.OnSkillSelectFinished();
                _waveSpawner = null;
            }

            Time.timeScale = 1f;
        }

        /*

        // 削除が出てきたら使用する可能性あり

        if (_isAdd)
        {
            _playerSkillManager.AddPassiveSkill(_changeSkillName);

            _changeSkillName = "None";
            _isAdd = false;
        }
        else if (_isSub)
        {
            _playerSkillManager.SubPassiveSkill(_changeSkillName);

            _changeSkillName = "None";
            _isSub = false;
        }

        */
    }

    public void StartSkillSelect(SkillData.SkillElement element)
    {

        _isSkillSelect = true;

        // 時間を止める
        Time.timeScale = 0f;

        // カーソルの位置を初期化する
        _cursorIndex = SKILL_SELECT_NUM - 1;


        // 選択することができるスキルを取得する
        _selectSkills = GetSelectSkill(element);

        // スキル選択パネルを生成する
        for (int i = 0; i < SKILL_SELECT_NUM; i++)
        {
            GameObject panel = Instantiate(_skillSelectPanel, _skillSelectCanvas.transform);
            SkillPanelInfo panelInfo = panel.GetComponent<SkillPanelInfo>();

            string skillName = _selectSkills[i].skillName;

            switch (_selectSkills[i].skillCategory)
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
            panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (i - 1) * SKILL_PANEL_DISTANCE);
        }



        // カーソルを生成する
        Vector2 cursorPos = new Vector2(-SKill_CURSOR_PANEL_DISTANCE, (_cursorIndex * SKILL_PANEL_DISTANCE) - SKILL_PANEL_DISTANCE);

        _cursorObj = Instantiate(_skillSelectCursor);

        _cursorObj.transform.SetParent(_skillSelectCanvas.transform, false);
        _cursorObj.GetComponent<RectTransform>().anchoredPosition = cursorPos;
    }

    private List<SelectSkill> GetSelectSkill(SkillData.SkillElement element)
    {

        List<SelectSkill> selectSkills = new List<SelectSkill>();

        // 三つのスキルを選択する
        for (int i = 0; i < SKILL_SELECT_NUM; i++)
        {
            // 選ぶことができるスキルを選択するまでループ
            bool isSelectSkill = false;

            SelectSkill selectSkill = new SelectSkill();

            int loopCount = 0;

            while (!isSelectSkill)
            {

                loopCount++;

                // 無限ループ防止
                if (loopCount > 3000)
                {
                    Debug.LogError("スキルの選択に失敗しました。");
                    Debug.Break();
                    break;
                }

                // スキルのカテゴリを選択する
                // int skillCategory = Random.Range(0, (int)SkillData.SkillCategory.CategoryNum);

                // パッシブスキルは同じ属性がいくつかあるので選択した属性のパッシブスキルをリストアップする
                List<int> index = new List<int>();
                for (int j = 0; j < _skillData.passiveSkill.passiveSkillDataList.Count; j++)
                {
                    if (_skillData.passiveSkill.passiveSkillDataList[j].skillElement == SkillData.SkillElement.Fire)
                    {
                        index.Add(j);
                    }
                }

                // 既に持っているスキルカテゴリは選択しない
                // if (_playerSkillManager.IsHaveSkillCategory((SkillData.SkillCategory)skillCategory)) continue;

                //switch (skillCategory)
                //{
                //    case (int)SkillData.SkillCategory.LowAttack:

                //        // スキルの属性が一致するスキルを取得する
                //        foreach (var skill in _skillData.lowAttackSkill.lowAttackSkillDataList)
                //        {
                //            if (skill.skillElement == element)
                //            {
                //                selectSkill.skillName = skill.skillName;
                //                selectSkill.skillCategory = SkillData.SkillCategory.LowAttack;
                //                isSelectSkill = true;
                //                break;
                //            }
                //        }

                //        break;
                //    case (int)SkillData.SkillCategory.ChargeAttack:

                //        // スキルの属性が一致するスキルを取得する
                //        foreach (var skill in _skillData.chargeAttackSkill.chargeAttackSkillDataList)
                //        {
                //            if (skill.skillElement == element)
                //            {
                //                selectSkill.skillName = skill.skillName;
                //                selectSkill.skillCategory = SkillData.SkillCategory.ChargeAttack;
                //                isSelectSkill = true;
                //                break;
                //            }
                //        }

                //        break;
                //    case (int)SkillData.SkillCategory.SpecialCharge:
                //        // スキルの属性が一致するスキルを取得する
                //        foreach (var skill in _skillData.specialChargeSkill.specialChargeSkillDataList)
                //        {
                //            if (skill.skillElement == element)
                //            {
                //                selectSkill.skillName = skill.skillName;
                //                selectSkill.skillCategory = SkillData.SkillCategory.SpecialCharge;
                //                isSelectSkill = true;
                //                break;
                //            }
                //        }
                //        break;
                //    case (int)SkillData.SkillCategory.RangedAttack:
                //        // スキルの属性が一致するスキルを取得する
                //        foreach (var skill in _skillData.rangedAttackSkill.rangedAttackSkillDataList)
                //        {
                //            if (skill.skillElement == element)
                //            {
                //                selectSkill.skillName = skill.skillName;
                //                selectSkill.skillCategory = SkillData.SkillCategory.RangedAttack;
                //                isSelectSkill = true;
                //                break;
                //            }
                //        }
                //        break;
                //    case (int)SkillData.SkillCategory.Dash:
                //        // スキルの属性が一致するスキルを取得する
                //        foreach (var skill in _skillData.dashSkill.dashSkillDataList)
                //        {
                //            if (skill.skillElement == element)
                //            {
                //                selectSkill.skillName = skill.skillName;
                //                selectSkill.skillCategory = SkillData.SkillCategory.Dash;
                //                isSelectSkill = true;
                //                break;
                //            }
                //        }
                //        break;
                //    case (int)SkillData.SkillCategory.Passive:


                //}

                // もし一つもなければもう一度スキルカテゴリを選択し直す
                if (index.Count == 0) break;

                // 選択した属性のパッシブスキルの中からランダムに選択する
                int passiveIndex = index[Random.Range(0, index.Count)];
                selectSkill.skillName = _skillData.passiveSkill.passiveSkillDataList[passiveIndex].skillName;
                selectSkill.skillCategory = SkillData.SkillCategory.Passive;
                isSelectSkill = true;

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

    public void PopSkillGetObject(Vector3 pos, SkillData.SkillElement element)
    {
        PopSkillGetObject(pos, element, null);
    }

    public void PopSkillGetObject(Vector3 pos, SkillData.SkillElement element, WaveSpawner waveSpawner)
    {
        GameObject obj = Instantiate(_skillGetObject, pos, Quaternion.identity);
        obj.transform.position = pos;
        SkillGetItem script = obj.GetComponent<SkillGetItem>();
        script.SetSkillElement(element);
        script.SetSkillSelectManager(this.gameObject);

        if (waveSpawner != null)
        {
            _waveSpawner = waveSpawner;
        }

    }

}