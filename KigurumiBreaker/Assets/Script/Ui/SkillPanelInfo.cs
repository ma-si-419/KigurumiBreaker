using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillPanelInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _contents;

    public void SetInfo(string name, string contents)
    {
        _name.text = name;
        _contents.text = contents;
    }
}
