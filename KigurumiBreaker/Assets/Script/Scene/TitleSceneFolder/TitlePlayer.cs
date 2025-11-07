using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlePlayer : MonoBehaviour
{
    //TitleCameraMove cameramove;
    [SerializeField] private TitleCameraMove _cameraMove;
    [SerializeField] private float _moveSpped = 0.02f;
    private Animator _anim = null;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        //Idle状態を解除
        //_anim.SetBool("Idle", false);

        //cameramove = GetComponent<TitleCameraMove>();

        //アニメーションスピード設定
        _anim.SetFloat("Speed", _moveSpped);
    }

    // Update is called once per frame
    void Update()
    {
       if(_cameraMove.isStop)
        {
            //アニメーションを戻す
            _anim.SetFloat("Speed", 1.0f);

            Debug.Log("アニメーションが戻る");
        }
    }
}
