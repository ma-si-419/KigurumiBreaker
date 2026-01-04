using System.Collections;
using System.Collections.Generic;
using FullOpaqueVFX;
using UnityEngine;

public class TitleCameraMove : MonoBehaviour
{
    [SerializeField] private GameObject _player;    //プレイヤーのオブジェクト情報

    [SerializeField] private TitleCameraData _titleCameraData;    //タイトルカメラデータ

    private CameraShake _cameraShake;

    private float _cameraRotateSpeed = 0.0f;    //カメラの回転速度
    private float _cameraMoveSpeed = 0.0f;      //カメラの移動速度
    [SerializeField] private int _stopRotation = 2;                 //プレイヤーの周りを回る回数
    private int _stopCount = 0;
    private bool _isCountFrag = false;                     //回転数カウントフラグ
    private bool _isStopEvent = false;                     //カメラ移動の停止イベントフラグ
    public bool isStop = false;

    public bool GetStopEvent() { return _isStopEvent; }

    [SerializeField] private float _gravityScale = 1.0f;

    private float _lastGravity = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //回転停止フラグを初期化
        isStop = false;

        //初期のカメラ位置を設定
        transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y + 150, _titleCameraData.distance);

        //初期のカメラ回転を設定
        transform.rotation = Quaternion.Euler(_titleCameraData.cameraRotation);

        _cameraRotateSpeed = _titleCameraData.rotationSpeed;
        _cameraMoveSpeed = _titleCameraData.moveSpeed;
    }

    private void FixedUpdate()
    {

        if (_lastGravity != _gravityScale)
        {
            Physics.gravity = new Vector3(0.0f, -_gravityScale, 0.0f);
        }
        _lastGravity = _gravityScale;

        //注視点の位置を設定
        var lookAt = _player.transform.position + Vector3.up * _titleCameraData.lookHeight;

        //プレイヤーを注視する
        transform.LookAt(lookAt);

        //プレイヤーが画面を殴るまでカメラを移動させる
        if (!_isStopEvent)
        {
            //カメラの位置を更新
            transform.RotateAround(lookAt, new Vector3(0, 1, 0), _cameraRotateSpeed);


            //回転数が満たない時は移動させる
            if (_stopCount != 2)
            {
                //プレイヤーに近づくまで移動させる
                transform.position = Vector3.MoveTowards(transform.position, lookAt + transform.forward * _titleCameraData.distance, _cameraMoveSpeed);
            }

        }


        //プレイヤーの前で止まるようにする
        if (_stopRotation == _stopCount)
        {
            //カメラ移動を止める
            _isStopEvent = true;

            //注視点をプレイヤーにする


            if (Camera.main.fieldOfView >= 20)
            {
                //プレイヤーに近づくまで移動させる
                Camera.main.fieldOfView -= _cameraMoveSpeed;
            }
            else
            {
                isStop = true;
            }


        }
        //回転数が満たない時は回転カウント
        else if (transform.rotation.y >= 0.0f && transform.rotation.y <= 5.0f)
        {
            if (_isCountFrag)
            {
                _stopCount++;
                _isCountFrag = false;
            }
            //回転数が1回の時のカメラ速度調整
            //if (_stopCount == 1)
            //{
            //    _cameraRotateSpeed = 1.5f;
            //    _cameraMoveSpeed = 1.0f;
            //}
        }
        else
        {
            _isCountFrag = true;
        }

        Debug.Log(isStop);
    }
}
