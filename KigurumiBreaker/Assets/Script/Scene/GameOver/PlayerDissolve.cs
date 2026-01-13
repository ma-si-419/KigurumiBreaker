using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDissolve : MonoBehaviour
{
    [SerializeField] private float _Time = 1.0f;       //再生時間
    [SerializeField] private float _WaitTime = 1.0f;   //再生までの待ち時間

    private Material _Material = null;
    [SerializeField] private Material _OutLineMaterial = null;
    private int _Width = 0;
    private int _Cutoff = 0;
    private int _ColorIntensity = 0;
    private int _OutLineAlpha = 0;

    private float _Duration = 0.0f;   //残り時間
    private float _HalfTime = 0.0f;   //再生時間の半分

    // Start is called before the first frame update
    void Start()
    {
        _Material = GetComponentInChildren<Renderer>().material;
        _Width = Shader.PropertyToID("_Width");
        _Cutoff = Shader.PropertyToID("_CutOff");
        _ColorIntensity = Shader.PropertyToID("_ColorIntensity");
        _OutLineAlpha = Shader.PropertyToID("_Alpha");

        if(_Material != null)
        {
            _Material.SetFloat(_Cutoff, 1.0f);
            _Material.SetFloat(_Width, 1.0f);
            _Material.SetFloat(_ColorIntensity, 0.0f);
            _OutLineMaterial.SetFloat(_OutLineAlpha, 1.0f);
        }
        _HalfTime = _Time / 4.0f * 3.0f;   //見た目調整
        _Duration = _Time;
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;

        //待ち時間
        _WaitTime -= delta;
        if (_WaitTime > 0.0f) return;

        _Duration -= delta;
        if (_Duration < 0.0f) _Duration = 0.0f;

        //しきい値のアニメーション(再生時間の上半分の時間で1～0に推移)
        float cutoff = (_Duration - _HalfTime) / _HalfTime;
        //float cutoff = (_HalfTime - _Duration) / _HalfTime;
        if (cutoff < 0.0f) cutoff = 0.0f;

        //幅のアニメーション(再生時間の下半分の時間で1～0に推移)
        float width = (_HalfTime - _Duration) / _HalfTime;
        //float width = (_Duration - _HalfTime) / _HalfTime;
        if (width < 0.0f) width = 0.0f;
        width = 1.0f - width;

        //シェーダーに値を渡す
        if(_Material != null)
        {
            _Material.SetFloat(_Cutoff, cutoff);
            _Material.SetFloat(_Width, width);
            _Material.SetFloat(_ColorIntensity, width);
            _OutLineMaterial.SetFloat(_OutLineAlpha, cutoff);
        }

    }
}
