using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private ShakeCamera _shakeCamera;
    [SerializeField] private BreakBlock _breakBlock;
    // Update is called once per frame
    void Update()
    {
        //壁に日々が入った瞬間に揺らす
        if (_breakBlock.breakMoment)
        {
            //カメラを揺らす
            StartCoroutine(_shakeCamera.MyShake(0.5f, 0.2f));
           
            Debug.Log("シェイク");
        }
    }
}
