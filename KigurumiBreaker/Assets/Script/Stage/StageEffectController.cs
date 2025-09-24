//using UnityEngine;

//public class StageEffectController : MonoBehaviour
//{
//    [Header("対象のエフェクト")]
//    [SerializeField] private ParticleSystem[] targetEffects;

//    [Header("敵に付けるタグ名")]
//    [SerializeField] private string enemyTag = "Enemy";

//    void Update()
//    {
//        //// 敵が全滅したらエフェクト停止
//        //GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
//        //if (enemies.Length == 0)
//        //{
//        //    StopAllEffects();
//        //}

//        // Spaceキー押したらエフェクト停止
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            StopAllEffects();
//        }
//    }

//    private void StopAllEffects()
//    {
//        foreach (var effect in targetEffects)
//        {
//            if (effect != null && effect.isPlaying)
//            {
//                var main = effect.main;   // mainモジュールを取得
//                main.loop = false;        // ループを切る
//                effect.Stop();            // 再生を停止
//            }
//        }
//    }

//}

using UnityEngine;

public class StageEffectController : MonoBehaviour
{
    [SerializeField] private GameObject[] targetObjects;

    private bool isActive = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isActive = !isActive;

            foreach (var obj in targetObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(isActive);
                }
            }
        }
    }
}
