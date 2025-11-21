using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakBlock : MonoBehaviour
{
    [SerializeField] private bool _useGravity = true;                           //重力を有効にする
    [SerializeField] private Vector3 _explodeVe1 = new Vector3(0, 0, 0.1f);     //爆発の中心地
    [SerializeField] private float _explodeForce = 200f;                        //爆発の威力
    [SerializeField] private float _explodeRange = 10f;                         //爆発の範囲
    [SerializeField] private float _breakTime1 = 0.02f;                         //ひび割れタイム
    [SerializeField] private float _breakTime2 = 0.8f;                          //弾けタイム
    [SerializeField] private TitleCameraMove _titleCameraMove;                  //カメラ動きをとる                        
    private Rigidbody[] rigidbodies;

    public bool breakMoment = false;                                            //壁を破壊する瞬間

    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();                    //子のRididbodyを取得しておく
       

        breakMoment = false;
    }

    void Update()
    {
        //壁を壊す
        if(_titleCameraMove.isStop)
        {
            StartCoroutine("BreakStart");                                          //動作にディレイをするためのコルーチンを使用
        }
    }

    IEnumerator BreakStart()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
            rb.useGravity = _useGravity;
            rb.AddExplosionForce(_explodeForce / 5, transform.position + _explodeVe1, _explodeRange);
            Debug.Log(rigidbodies);

        }

        yield return new WaitForSeconds(_breakTime1);

        foreach (Rigidbody rb in rigidbodies)
        {

            rb.isKinematic = true;

            Debug.Log(rigidbodies);

        }

        yield return new WaitForSeconds(_breakTime2);

        foreach (Rigidbody rb in rigidbodies)
        {
            //画面を揺らすフラグ建て
            breakMoment = true;

            rb.isKinematic = false;
            rb.AddExplosionForce(_explodeForce, transform.position + _explodeVe1, _explodeRange);

            Debug.Log(rigidbodies);


        }
    }
}
