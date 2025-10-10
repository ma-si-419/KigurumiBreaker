using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    [SerializeField] private GameObject _player; // プレイヤーオブジェクトの参照

    [SerializeField] private BoxCollider _moveArea; // カメラの移動範囲を指定するBoxCollider

    [SerializeField] private CameraShakeData _shakeData; // カメラの揺れデータ

    [SerializeField] private CameraData _cameraData; // カメラの位置、回転データ

    [SerializeField] private SpecialAttackCameraMoveData _specialAttackCameraMoveData; // 必殺技中のカメラ移動データ

    bool _isSpecialAttack = false; // プレイヤーが必殺技を使っているかどうか

    public enum ShakeKind
    {
        NONE,
        SMALL,
        MIDDLE,
        LARGE,
        TYPENUM
    }

    int _shakeTime = 0;                                     // 揺れの時間を保存する変数
    float _shakePower = 0.0f;                               // 揺れの大きさを保存する変数

    int _specialAttackFrame = 0;                            // 必殺技中のフレーム数を保存する変数
    int _returnFrame = 0;                                   // 必殺技終了後の元の位置に戻るまでのフレーム数を保存する変数

    SpecialAttackCameraMoveData.MoveData _currentMoveData;  // 現在の必殺技中のカメラ移動データを保存する変数

    Vector3 _frameMoveVec = Vector3.zero;                   // 一フレームに移動するベクトルを保存する変数
    Vector3 _specialAttackShiftVec = Vector3.zero;          // 必殺技中に移動したベクトルの合計を保存する変数

    private Vector3 _initialRotation;                       // カメラの初期回転を保存する変数

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(_cameraData.cameraRotation); // カメラの初期回転を設定
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // プレイヤーの位置にオフセットを加えた位置にカメラを移動
        transform.position = _player.transform.position + _cameraData.cameraPosition;

        // カメラの位置が移動範囲を超えないように制限
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, _moveArea.bounds.min.x, _moveArea.bounds.max.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, _moveArea.bounds.min.y, _moveArea.bounds.max.y);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, _moveArea.bounds.min.z, _moveArea.bounds.max.z);
        transform.position = clampedPosition;

        transform.rotation = Quaternion.Euler(_cameraData.cameraRotation); // カメラの初期回転を設定

        if (_shakeTime > 0)
        {
            _shakeTime--;

            // カメラの向きをランダムに揺らす
            float shakeX = Random.Range(-_shakePower, _shakePower);
            float shakeY = Random.Range(-_shakePower, _shakePower);
            float shakeZ = Random.Range(-_shakePower, _shakePower);

            Vector3 rota = new Vector3(shakeX,shakeY,shakeZ);

            transform.rotation = Quaternion.Euler(_cameraData.cameraRotation + rota);
        }


        if (_isSpecialAttack)
        {
            // 必殺技を行っているフレーム数
            _specialAttackFrame++;

            // データを参照して、次に移動するフレームと距離を取得
            foreach (var data in _specialAttackCameraMoveData.specialMoveDatas)
            {
                // 必殺技を行っているフレームより前のフレームのデータを参照
                if (_specialAttackFrame < data.MoveFrame)
                {
                    // 前回と同じデータなら何もしない
                    if (_currentMoveData == data) break;

                    _currentMoveData = data;

                    // 一フレームに移動する距離を計算
                    Vector3 playerToCameraDir = (transform.position - _player.transform.position).normalized;
                    Vector3 targetPos = _player.transform.position + playerToCameraDir * data.PlayerDistance;
                    _frameMoveVec = (targetPos - (transform.position + _specialAttackShiftVec)) / (data.MoveFrame - _specialAttackFrame);

                    break;
                }
            }
            if (_currentMoveData.MoveFrame != 0)
            {
                _specialAttackShiftVec += _frameMoveVec;
                transform.position += _specialAttackShiftVec;
            }
        }
        else
        {
            // 必殺技終了後、数フレームかけて元の位置に戻る
            _returnFrame++;

            if (_returnFrame <= _specialAttackCameraMoveData.returnFrame)
            {
                Vector3 moveVec = _specialAttackShiftVec;

                moveVec /= _specialAttackCameraMoveData.returnFrame;

                moveVec *= (_specialAttackCameraMoveData.returnFrame - _returnFrame);

                transform.position += moveVec;
            }
            else
            {
                _specialAttackShiftVec = Vector3.zero;
            }
        }

    }

    public void SetShakeData(int time, float power)
    {
        _shakeTime = time;
        _shakePower = power;
    }

    public void SetShakeData(ShakeKind type)
    {
        switch (type)
        {
            case ShakeKind.NONE:
                SetShakeData(0, 0.0f);
                break;
            case ShakeKind.SMALL:
                SetShakeData(_shakeData.lowTime, _shakeData.lowPower);
                break;
            case ShakeKind.MIDDLE:
                SetShakeData(_shakeData.middleTime, _shakeData.middlePower);
                break;
            case ShakeKind.LARGE:
                SetShakeData(_shakeData.highTime, _shakeData.highPower);
                break;
        }
    }

    public void SetSpecialAttack(bool flag)
    {
        _isSpecialAttack = flag;
        // セットするたびにフレーム数をリセット
        _specialAttackFrame = 0;
        _returnFrame = 0;
    }
}
