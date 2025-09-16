using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEffectController : MonoBehaviour
{
    [Header("�Ώۂ̃G�t�F�N�g")]
    [SerializeField] private ParticleSystem[] targetEffects;

    [Header("�G�ɕt����^�O��")]
    [SerializeField] private string enemyTag = "Enemy";

    void Update()
    {
        // �G���S�ł�����G�t�F�N�g��~
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        //if (enemies.Length == 0)
        //{
        //    StopAllEffects();
        //}

        // Space�L�[��������G�t�F�N�g��~
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllEffects();
        }
    }

    private void StopAllEffects()
    {
        foreach (var effect in targetEffects)
        {
            if (effect != null && effect.isPlaying)
            {
                effect.loop = false;  // ���[�v��؂�
                effect.Stop();        // �Đ����~
            }
        }
    }
}
