using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class TestLaser : MonoBehaviour
{

    //[SerializeField] private float _speed = 10f;
    //[SerializeField] private float _maxLength = 10f;

    //[SerializeField] private float _laserY = 0.5f;
    //[SerializeField] private float _laserX = 0.5f;

    //private float _currentLength;

    //void FixedUpdate()
    //{
    //    if (_currentLength >= _maxLength) return;

    //    float delta = _speed * Time.deltaTime;
    //    _currentLength += delta;

    //    transform.localScale += new Vector3(_laserX, _laserY, delta);
    //    transform.localPosition += Vector3.forward * (delta * 0.5f);
    //}

    [SerializeField] private float _laserRange = 10.0f;
    [SerializeField] private int _scallingTime = 10;

    [Header("レーザーの太さを変える数値")]
    [SerializeField] private float _laserScaleX = 0.5f;
    [SerializeField] private float _laserScaleZ = 0.5f;

    private int _time = 0;
    private float _scalePerFrame;

    private void Start()
    {
        _scalePerFrame = _laserRange / _scallingTime;
        transform.localScale = new Vector3(_laserScaleX, 0, _laserScaleZ);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_time > _scallingTime) return;

        _time++;

        if (_time <= _scallingTime)
        {
            // Y軸方向にスケールを増やす
            transform.localScale += new Vector3(0, _scalePerFrame, 0);

            // 伸びた分の半分だけ前にずらす
            transform.position += new Vector3(0, 0, _scalePerFrame);
        }
    }

    public Vector3 ScaleProcessor()
    {
        return transform.position += new Vector3(0, 0, _scalePerFrame);
    }
}
