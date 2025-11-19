using System.Collections;
using System.Collections.Generic;
using FullOpaqueVFX;
using UnityEngine;

public class TitleCameraMove : MonoBehaviour
{
    [SerializeField] private Transform _playerTr;    //プレイヤーのTransform情報
    [SerializeField] private float _heightAt = 50.0f;    //注視するカメラの高さ
    [SerializeField] private float _cameraDistance = 50.0f;   //カメラと注視点の距離
    [SerializeField] private float _cameraRotateSpeed = 0.5f;    //カメラの回転速度
    [SerializeField] private float _cameraMoveSpeed = 0.4f;      //カメラの移動速度
    [SerializeField] private int _stopRotation = 2;                 //プレイヤーの周りを回る回数
    [SerializeField] private TitleShakeData _cameraShakeData;    //カメラシェイクデータ
    private int _stopCount = 0;
    private bool _isCountFrag = false;                     //回転数カウントフラグ
    private bool _isStopEvent = false;                     //カメラ移動の停止イベントフラグ
    public bool isStop = false;

    private int _shakeTime = 0;            //揺れの時間を保存する変数
    private float _shakePower = 0.0f;      //揺れの大きさを保存する変数


    // Start is called before the first frame update
    void Start()
    {
        //回転停止フラグを初期化
        isStop = false;

        //初期のカメラ位置を設定
        transform.position = new Vector3(_playerTr.position.x, _playerTr.position.y + 150, _cameraDistance);

        _shakeTime = _cameraShakeData.time;
    }

    private void FixedUpdate()
    {
        //注視点の位置を設定
        var lookAt = _playerTr.position + Vector3.up * _heightAt;
        //プレイヤーを注視する
        transform.LookAt(lookAt);

        //プレイヤーが画面を殴るまでカメラを移動させる
        if(!_isStopEvent)
        {
            //カメラの位置を更新
            transform.RotateAround(lookAt, new Vector3(0, 1, 0), _cameraRotateSpeed);


            //回転数が満たない時は移動させる
            if (_stopCount != 1)
            {
                //プレイヤーに近づくまで移動させる
                transform.position = Vector3.MoveTowards(transform.position, lookAt + transform.forward * _cameraDistance, _cameraMoveSpeed);
            }

        }

        
        //プレイヤーの前で止まるようにする
        if(_stopRotation == _stopCount)
        {
            //カメラ移動を止める
            _isStopEvent = true;

            if (Camera.main.fieldOfView >= 20)
            {
                //プレイヤーに近づくまで移動させる
                Camera.main.fieldOfView -= _cameraMoveSpeed;
            }
            else
            {
                isStop = true;
            }

            //カメラシェイクを実行
            if(_cameraShakeData.time > 0.0f)
            {
                _shakeTime--;

                //カメラの向きをランダムに揺らす
                float shakeX = Random.Range(-_shakePower, _shakePower);
                float shakeY = Random.Range(-_shakePower, _shakePower);
                float shakeZ = Random.Range(-_shakePower, _shakePower);

                Vector3 rota = new Vector3(shakeX, shakeY, shakeZ);

                transform.rotation = Quaternion.Euler(transform.position + rota);
            }

            

        }
        //回転数が満たない時は回転カウント
        else if(transform.rotation.y >= 0.0f && transform.rotation.y <= 5.0f)
        {
            if(_isCountFrag)
            {
                _stopCount++;
                _isCountFrag = false;
            }
            //回転数が1回の時のカメラ速度調整
            if(_stopCount == 1)
            {
                _cameraRotateSpeed = 1.5f;
                _cameraMoveSpeed = 1.0f;
            }
        }
        else
        {
            _isCountFrag = true;
        }

        //Debug.Log(_stopCount);
    }
}
