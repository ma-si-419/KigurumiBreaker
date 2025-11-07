using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCameraMove : MonoBehaviour
{
    [SerializeField] private Transform _playerTr;    //プレイヤーのTransform情報
    [SerializeField] private float _heightAt = 50.0f;    //注視するカメラの高さ
    [SerializeField] private float _cameraDistance = 50.0f;   //カメラと注視点の距離
    [SerializeField] private float _cameraRotateSpeed = 0.5f;    //カメラの回転速度
    [SerializeField] private float _cameraMoveSpeed = 0.4f;      //カメラの移動速度
    [SerializeField] private int _stopRotation = 2;                 //プレイヤーの周りを回る回数
    private int _stopCount = 0;
    private bool _isCountFrag = false;                     //回転数カウントフラグ
    public bool isStop = false;


    // Start is called before the first frame update
    void Start()
    {
        //回転停止フラグを初期化
        isStop = false;

        //初期のカメラ位置を設定
        transform.position = new Vector3(_playerTr.position.x, _playerTr.position.y + 150, _cameraDistance);
    }

    private void FixedUpdate()
    {
        //注視点の位置を設定
        var lookAt = _playerTr.position + Vector3.up * _heightAt;
        //プレイヤーを注視する
        transform.LookAt(lookAt);

        //プレイヤーが画面を殴るまでカメラを移動させる
        if(!isStop)
        {
            //カメラの位置を更新
            transform.RotateAround(lookAt, new Vector3(0, 1, 0), _cameraRotateSpeed);
            //プレイヤーに近づくまで移動させる
            transform.position = Vector3.MoveTowards(transform.position, lookAt + transform.forward * _cameraDistance, _cameraMoveSpeed);
        }

        
        //プレイヤーの前で止まるようにする
        if(_stopRotation == _stopCount)
        {
            isStop = true;
        }
        //回転数が満たない時は回転カウント
        else if(transform.rotation.y >= 0.0f && transform.rotation.y <= 5.0f)
        {
            if(_isCountFrag)
            {
                _stopCount++;
                _isCountFrag = false;
            }
        }
        else
        {
            _isCountFrag = true;
        }

        //Debug.Log(_stopCount);
    }
}
