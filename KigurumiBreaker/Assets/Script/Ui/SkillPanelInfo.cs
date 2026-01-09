using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SkillPanelInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _contents;
    [SerializeField] private Color _color;
    [SerializeField] private Sprite _icon;

    public void SetInfo(string name, string contents, Color color, Sprite sprite)
    {
        _name.text = name;
        _contents.text = contents;
        _color = color;
        _icon = sprite;

        transform.GetChild(0).GetComponent<Image>().color = _color;
        transform.GetChild(1).GetComponent<Image>().sprite = _icon;
    }
}
