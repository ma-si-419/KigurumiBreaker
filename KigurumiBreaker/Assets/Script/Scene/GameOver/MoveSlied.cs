using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MoveSlied : MonoBehaviour
{
    //[SerializeField] private float _Speed = 1.0f;       //スライド移動
    [SerializeField] private float _MaxY = 0.0f;        //最大Y座標
    [SerializeField] private float _Time = 1.0f;        //移動開始時間
    [SerializeField] private GameObject _Ui;

    //[SerializeField] private float _DropHeight = 100.0f;  //ドロップ高さ
    //[SerializeField] private float _Duration = 0.5f;      //ドロップ時間


    RectTransform Rect;

    /*失敗作

    // Start is called before the first frame update
    void Start()
    {
        //_Ui = GetComponent<Canvas>();

    }


    // Update is called once per frame
    void Update()
    {
        //float delta = Time.deltaTime;

        //Vector3 pos = _Ui.transform.localPosition;

        //_Time -= delta;

        //if (0.0f >= _Time)
        //{
        //    pos.y -= _Speed;

        //    if (pos.y <= _MaxY)
        //    {
        //        pos.y = _MaxY;

        //    }

        //    _Ui.transform.localPosition = pos;

        //    Debug.Log("?");
        //}
    }

    */

    private void OnEnable()
    {
        Rect = GetComponent<RectTransform>();

        Vector3 pos = _Ui.transform.position;

        Rect.DOMove(new Vector3(pos.x, _MaxY, pos.z), _Time).SetEase(Ease.OutBounce);
    }
}
