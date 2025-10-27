using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerState;

public class SkillGetItem : MonoBehaviour
{
    [SerializeField] private GameObject _skillSelectManager;
    private SkillData.SkillElement _skillElement = SkillData.SkillElement.Fire;

    private bool _isButtonDown = false;

    GameInputs inputActions;

    public void Awake()
    {
        inputActions = new GameInputs();
        inputActions.Enable();

        inputActions.Player.GetItem.started += GetItem;

    }

    private void OnTriggerStay(Collider other)
    {

        Debug.Log(other.tag + "とぶつかっています");

        if (other.gameObject.CompareTag("Player"))
        {
            // Yボタンが押されたらスキル選択画面へ
            if (_isButtonDown)
            {
                _skillSelectManager.GetComponent<SkillSelectManager>().StartSkillSelect(_skillElement);
                inputActions.Disable();
                Destroy(gameObject);
            }

        }
    }

    public void SetSkillElement(SkillData.SkillElement element)
    {
        _skillElement = element;
    }

    public void SetSkillSelectManager(GameObject manager)
    {
        _skillSelectManager = manager;
    }

    private void GetItem(InputAction.CallbackContext constext)
    {
        _isButtonDown = true;
    }
}
