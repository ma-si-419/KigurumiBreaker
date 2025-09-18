using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyZangiHp : MonoBehaviour
{
    public Canvas canvas;
    public GameObject enemy; //敵キャラのオブジェクト

    public Slider slider; //Sliderコンポーネント

    void Update()
    {
        int nowHp = enemy.GetComponent<ZangiMove>().nowHp;
        int maxHp = enemy.GetComponent<ZangiMove>().maxHp;

        //Sliderの値を更新
        slider.value = (float)nowHp / (float)maxHp;

        //EnemyCanvasをMain Cameraに向かせる
        canvas.transform.rotation =
            Camera.main.transform.rotation;
    }
}
