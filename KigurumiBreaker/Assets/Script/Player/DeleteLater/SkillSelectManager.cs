using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class SkillSelectManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerSkillManager;
    [SerializeField] private SkillData _skillData;
    [SerializeField] private Canvas _skillSelectCanvas;

    private PlayerSkillManager _skillManager;

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


    }
}
