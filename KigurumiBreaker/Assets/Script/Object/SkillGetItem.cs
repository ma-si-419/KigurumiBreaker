using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillGetItem : MonoBehaviour
{
    private GameObject _skillSelectManager;
    private SkillData.SkillElement _skillElement;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Aボタンが押されたらスキル選択画面へ
            if (Input.GetButtonDown("OK"))
            {
                _skillSelectManager.GetComponent<SkillSelectManager>().StartSkillSelect(_skillElement);
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
}
