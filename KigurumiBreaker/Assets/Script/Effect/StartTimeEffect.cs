using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTimeEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _ps;
    [SerializeField] private float _timeSpecification;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _ps.Length; i++)
        {
            _ps[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _ps[i].Simulate(_timeSpecification, true, true); // 2•b•ªi‚ß‚é
            _ps[i].Play();
        }
    }

}
