using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLoopTime : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _ps;
    [SerializeField] private float _timeSpecification;

    // 
    private bool _isFirstPlay = true;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _ps.Length; i++)
        {
            _ps[i] = GetComponent<ParticleSystem>();
        }
    }

    private void FixedUpdate()
    {
        if(_isFirstPlay)
        {
            bool anyAlive = false;

            for(int i = 0; i < _ps.Length; i++)
            {
                if (_ps[i].IsAlive(true))
                {
                    anyAlive = true;
                    break;
                }
            }

            if (!anyAlive)
            {
                for (int i = 0; i < _ps.Length; i++)
                {
                    _ps[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    _ps[i].Simulate(_timeSpecification, true, true); // 2•b•ªi‚ß‚é
                    _ps[i].Play();
                }

                _isFirstPlay = false;
            }
        }
    }
}
