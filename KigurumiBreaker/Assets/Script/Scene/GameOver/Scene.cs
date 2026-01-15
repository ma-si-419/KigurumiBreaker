using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene : MonoBehaviour
{
    [SerializeField] private float _Time = 0.0f;        //Scene移動時間
    private bool _One = false;

    // Start is called before the first frame update
    void Start()
    {
        _One = false;
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;
        _Time -= delta;

        if(0.0f >= _Time && !_One)
        {
            //安田オリジナル移動でシーン移動
            BaseSceneController.instance.ChangeSceneWithFade(SceneType.GameScene);

            //一回だけ反応させる
            _One = true;
        }
        
    }
}
