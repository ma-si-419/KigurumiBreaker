using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestLaser : MonoBehaviour
{
    [SerializeField] private float _laserRange = 10.0f;
    [SerializeField] private int _scallingTime = 10;
    private int _time = 0;
    private float _scalePerFrame;

    private void Start()
    {
        _scalePerFrame = _laserRange / _scallingTime;
        transform.localScale = new Vector3(1, 0, 1);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        _time++;

        if (_time <= _scallingTime)
        {
            transform.localScale += new Vector3(0, _scalePerFrame, 0);
            transform.position += new Vector3(0, 0, _scalePerFrame);
        }
    }
}
