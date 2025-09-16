using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectUI : MonoBehaviour
{
    [SerializeField] private GameObject _selectUi;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(_selectUi);
    }
}
