using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    [SerializeField] private GameObject _player; // プレイヤーオブジェクトの参照

    [SerializeField] private Vector3 _offset; // カメラとプレイヤーの相対位置

    [SerializeField] private BoxCollider _moveArea; // カメラの移動範囲を指定するBoxCollider

    [SerializeField] private CameraShakeData _shakeData; // カメラの揺れデータ


    public enum ShakeKind
    {
        NONE,
        SMALL,
        MIDDLE,
        LARGE,
        TYPENUM
    }

    int _shakeTime = 0; // 揺れの時間を保存する変数
    float _shakePower = 0.0f; // 揺れの大きさを保存する変数

    private Vector3 _initialRotation; // カメラの初期回転を保存する変数

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(45.0f, -29.0f, -4.5f); // カメラの初期回転を設定
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // プレイヤーの位置にオフセットを加えた位置にカメラを移動
        transform.position = _player.transform.position + _offset;

        // カメラの位置が移動範囲を超えないように制限
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, _moveArea.bounds.min.x, _moveArea.bounds.max.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, _moveArea.bounds.min.y, _moveArea.bounds.max.y);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, _moveArea.bounds.min.z, _moveArea.bounds.max.z);
        transform.position = clampedPosition;

        transform.rotation = Quaternion.Euler(45.0f, -29.0f, -4.5f); // カメラの初期回転を設定

        if (_shakeTime > 0)
        {
            _shakeTime--;

            // カメラの向きをランダムに揺らす
            float shakeX = Random.Range(-_shakePower, _shakePower);
            float shakeY = Random.Range(-_shakePower, _shakePower);
            float shakeZ = Random.Range(-_shakePower, _shakePower);
            transform.rotation = Quaternion.Euler(45.0f + shakeX, -29.0f + shakeY, -4.5f + shakeZ);

        }

    }

    public void SetShakeData(int time, float power)
    {
        _shakeTime = time;
        _shakePower = power;
    }

    public void SetShakeData(ShakeKind type)
    {
        switch(type)
        {
            case ShakeKind.NONE:
                SetShakeData(0,0.0f);
                break;
            case ShakeKind.SMALL:
                SetShakeData(_shakeData.lowTime,_shakeData.lowPower);
                break;
            case ShakeKind.MIDDLE:
                SetShakeData(_shakeData.middleTime, _shakeData.middlePower);
                break;
            case ShakeKind.LARGE:
                SetShakeData(_shakeData.highTime, _shakeData.highPower);
                break;
        }
    }
}
