using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyZangiHp : MonoBehaviour
{
    public Canvas canvas;
    public GameObject enemy; //�G�L�����̃I�u�W�F�N�g

    public Slider slider; //Slider�R���|�[�l���g

    void Update()
    {
        int nowHp = enemy.GetComponent<ZangiMove>().nowHp;
        int maxHp = enemy.GetComponent<ZangiMove>().maxHp;

        //Slider�̒l���X�V
        slider.value = (float)nowHp / (float)maxHp;

        //EnemyCanvas��Main Camera�Ɍ�������
        canvas.transform.rotation =
            Camera.main.transform.rotation;
    }
}
