using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    [SerializeField] private GameObject _player; // プレイヤーオブジェクトの参照

    [SerializeField] private CapsuleCollider _moveArea; // カメラの移動範囲を指定するCapsuleCollider

    [SerializeField] private CameraShakeData _shakeData; // カメラの揺れデータ

    [SerializeField] private CameraData _cameraData; // カメラの位置、回転データ

    [SerializeField] private SpecialAttackCameraMoveData _specialAttackCameraMoveData; // 必殺技中のカメラ移動データ

    private bool _isSpecialAttack = false; // プレイヤーが必殺技を使っているかどうか

    public enum ShakeKind
    {
        NONE,
        SMALL,
        MIDDLE,
        LARGE,
        TYPENUM
    }

    private int _shakeTime = 0;                                     // 揺れの時間を保存する変数
    private float _shakePower = 0.0f;                               // 揺れの大きさを保存する変数

    private int _specialAttackFrame = 0;                            // 必殺技中のフレーム数を保存する変数
    private int _returnFrame = 0;                                   // 必殺技終了後の元の位置に戻るまでのフレーム数を保存する変数

    private bool _isStop = false;                                   // カメラ移動を停止するかどうかを保存する変数

    private bool _isSwingCamera = false;                            // カメラがスイングしているかどうかを保存する変数

    private float _specialAttackDistance;                           // 特殊攻撃中のプレイヤーとカメラの距離を保存する変数

    private GameObject _specialAttackObject;                        // 必殺技を行っているオブジェクトを保存する変数

    private Vector3 _frameMoveVec = Vector3.zero;                   // 一フレームに移動するベクトルを保存する変数
    private Vector3 _specialAttackShiftVec = Vector3.zero;          // 必殺技中に移動したベクトルの合計を保存する変数

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(_cameraData.cameraRotation); // カメラの初期回転を設定
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // カメラシェイクは常に行う
        transform.rotation = Quaternion.Euler(_cameraData.cameraRotation); // カメラの初期回転を設定

        if (_shakeTime > 0)
        {
            _shakeTime--;

            // カメラの向きをランダムに揺らす
            float shakeX = Random.Range(-_shakePower, _shakePower);
            float shakeY = Random.Range(-_shakePower, _shakePower);
            float shakeZ = Random.Range(-_shakePower, _shakePower);

            Vector3 rota = new Vector3(shakeX, shakeY, shakeZ);

            transform.rotation = Quaternion.Euler(_cameraData.cameraRotation + rota);

        }


        if (_isStop) return;

        // スイングカメラ処理
        if (_isSwingCamera)
        {
            // カメラの向きを上下に揺らす
            float swingX = Mathf.Sin(Time.time * 30.0f) * 1.0f; // 周期と振幅を調整
            Vector3 rota = new Vector3(swingX * 0.7f, 0.0f, 0.0f);
            transform.rotation = Quaternion.Euler(_cameraData.cameraRotation + rota);
        }

        // 関数が呼ばれている間だけスイングカメラを有効にする
        _isSwingCamera = false;

        // プレイヤーの位置にオフセットを加えた位置にカメラを移動
        transform.position = _player.transform.position + _cameraData.cameraPosition;

        if (_moveArea != null)
        {
            // カメラの位置が移動範囲外に出ないように制限
            Vector3 closestPoint = _moveArea.ClosestPoint(transform.position);
            transform.position = closestPoint;
        }

        if (_isSpecialAttack)
        {
            // 必殺技を行っているフレーム数
            _specialAttackFrame++;

            // 必殺技中のカメラ移動処理
            Vector3 targetPos = _specialAttackObject.transform.position + _cameraData.cameraPosition;
            Vector3 dir = (targetPos - transform.position).normalized;
            targetPos += dir * _specialAttackDistance;

            // 移動方向
            Vector3 moveDir = (targetPos - transform.position).normalized;

            // 移動距離
            float moveScale = (float)_specialAttackFrame / (float)_specialAttackCameraMoveData.moveFrame;
            if (moveScale > 1.0f) moveScale = 1.0f;

            moveScale *= _specialAttackDistance;

            transform.position = _specialAttackObject.transform.position + _cameraData.cameraPosition + moveDir * moveScale;

            _specialAttackShiftVec = transform.position - (_player.transform.position + _cameraData.cameraPosition);
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

    public void SetMoveArea(CapsuleCollider col)
    {
        _moveArea = col;
    }

    public void SetShakeData(int time, float power)
    {
        _shakeTime = time;
        _shakePower = power;
    }

    public void SetSwing()
    {
        _isSwingCamera = true;
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

    public void StartSpecialAttack(int level, GameObject attackObj)
    {
        _isSpecialAttack = true;
        _specialAttackDistance = _specialAttackCameraMoveData.PlayerDistance[level - 1];
        _specialAttackObject = attackObj;

        // セットするたびにフレーム数をリセット
        _specialAttackFrame = 0;
    }

    public void EndSpecialAttack()
    {
        _isSpecialAttack = false;
        _returnFrame = 0;
    }

    public void SetStop(bool flag)
    {
        _isStop = flag;
    }
}
