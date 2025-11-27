using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCamera : MonoBehaviour
{
    [SerializeField] private float _cameraMove;          //カメラを回転させる速度

    [SerializeField] private Vector3 _axis = Vector3.forward;   //ワールド座標での回転
    [SerializeField] private GameObject _block;            //オブジェクト非表示

    [SerializeField] private TitleCameraMove _titleCameraMove;
    [SerializeField] private float _cameraMoveTimeRimit; //カメラを回転させるまでの待ち時間(実質壁が破壊されるのを待つ時間)
    private float _cameraMoveTime = 0.0f;


    float x = 0.0f;
    // Update is called once per frame
    void Update()
    {
        //カメラ移動が止まったら回転させる
        if(_cameraMoveTimeRimit <= _cameraMoveTime)
        {
            _block.SetActive(false);

            x = Time.deltaTime * 10;
            transform.rotation = Quaternion.AngleAxis(_cameraMove, _axis) * transform.rotation;
        }
        else if (_titleCameraMove.isStop)
        {
            _cameraMoveTime++;

            Debug.Log("カメ");
        }

    }
}
