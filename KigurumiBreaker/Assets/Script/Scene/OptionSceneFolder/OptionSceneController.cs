using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //�߂�(�{�^��)
        if (Input.GetButtonDown("Cancel"))
        {
            BaseSceneController.instance.ToggleOption();
            Debug.Log("A�{�^����������܂���");
        }
    }



}
