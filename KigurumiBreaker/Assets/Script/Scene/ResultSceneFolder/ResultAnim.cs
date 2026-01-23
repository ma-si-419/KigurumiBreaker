using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultAnim : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 0.1f;
    private Animator _anim = null;

    private float _stopTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();

        //Lowが攻撃っぽい
        _anim.SetBool("Low1", true);

        //アニメーションスピード設定
        _anim.SetFloat("Speed", _moveSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        _anim.SetBool("Low1", true);

       

        if (_stopTime >= 1.0f)
        {
            //一時停止する
            _anim.speed = 0.0f;
        }
        else
        {
            _stopTime = Time.time;
        }

    }
}
