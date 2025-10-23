using UnityEngine;

public class StageEffectController : MonoBehaviour
{
    [Header("対象のエフェクト")]
    [SerializeField] private ParticleSystem[] _targetEffects;

    [Header("敵に付けるタグ名")]
    [SerializeField] private string _enemyTag = "Enemy";

    void Update()
    {
        // 敵が全滅したらエフェクト停止
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(_enemyTag);
        if (enemies.Length == 0)
        {
            StopAllEffects();
        }

        // Spaceキー押したらエフェクト停止
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllEffects();
        }
    }

    private void StopAllEffects()
    {
        foreach (var effect in _targetEffects)
        {
            if (effect != null && effect.isPlaying)
            {
                var mainModule = effect.main; // mainモジュールを取得
                mainModule.loop = false;      // ループを切る
                effect.Stop();                // 再生を停止
            }
        }
    }
}
