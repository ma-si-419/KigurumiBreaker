using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugEffect : MonoBehaviour
{
    [SerializeField] List<GameObject> effects;

    GameObject obj;

    GameObject lastEffect;

    public int currentEffectIndex = 0;

    // Update is called once per frame
    void Update()
    {
        if (effects == null) return;

        // 数字の0が押されたらエフェクトを切り替え
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            currentEffectIndex++;

            if (currentEffectIndex >= effects.Count)
            {
                currentEffectIndex = 0;
            }
        }

        if (obj != lastEffect)
        {
            Destroy(lastEffect);
            lastEffect = obj;
        }


        // 数字の1が押されたらエフェクトをその場で再生
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (obj != null)
            {
                Destroy(obj);
            }

            Debug.Log("Effect 再生");

            obj = Instantiate(effects[currentEffectIndex], transform.position, Quaternion.identity);
        }

        // 数字の2が押されたらエフェクトを削除
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            if (obj != null)
            {
                Destroy(obj);

                Debug.Log("Effect 削除");
            }
        }
    }
}
