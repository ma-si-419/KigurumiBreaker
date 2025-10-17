using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillGetItem : MonoBehaviour
{
    [SerializeField] private GameObject _skillSelectManager;
    private SkillData.SkillElement _skillElement = SkillData.SkillElement.Fire;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Yボタンが押されたらスキル選択画面へ
            if (Input.GetButtonDown("ItemGet"))
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
