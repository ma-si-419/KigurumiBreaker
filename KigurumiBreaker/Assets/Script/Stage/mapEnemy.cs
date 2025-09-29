using UnityEngine;

public class mapEnemy : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Destroy(gameObject); // このスクリプトが付いているオブジェクトを削除
        }
    }
}
