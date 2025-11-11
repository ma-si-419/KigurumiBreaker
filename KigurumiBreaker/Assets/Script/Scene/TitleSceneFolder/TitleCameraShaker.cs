using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TitleCameraShaker : MonoBehaviour
{
    public static TitleCameraShaker Instance { get; private set; }

    private CinemachineVirtualCamera cam;
    private float shakerTimer;

    //ƒXƒNƒŠƒvƒg‚ğƒCƒ“ƒXƒ^ƒ“ƒX‰»
    private void Awake()
    {
        Instance = this;
        cam = GetComponent<CinemachineVirtualCamera>();
    }

    //—h‚ê‚Ì‹­‚³‚Æ—h‚ê‚éŠÔ‚Ìˆø”‚ğ“n‚·
    public void MyShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        shakerTimer = time;
    }

    void Update()
    {
        //—h‚êŠÔ‚ªI‚í‚Á‚½‚ç—h‚ê‚ğ~‚ß‚é
        if(shakerTimer > 0)
        {
            shakerTimer -= Time.deltaTime;
            if(shakerTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            }
        }
        
    }
}
