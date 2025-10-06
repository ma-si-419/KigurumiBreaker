using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class ZangiSkillSelect : MonoBehaviour
{
    [SerializeField] string _changeSkillName = "None";
    [SerializeField] bool _isAdd = false;
    [SerializeField] bool _isSub = false;
    [SerializeField] GameObject _skillManager;

    // Update is called once per frame
    void Update()
    {
        if(_isAdd)
        {
            SkillManager skillManager = _skillManager.GetComponent<SkillManager>();
            skillManager.AddPassiveSkill(_changeSkillName);

            _changeSkillName = "None";
            _isAdd = false;
        }
        else if(_isSub)
        {
            SkillManager skillManager = _skillManager.GetComponent<SkillManager>();
            skillManager.SubPassiveSkill(_changeSkillName);
            
            _changeSkillName = "None";
            _isSub = false;
        }
    }
}
