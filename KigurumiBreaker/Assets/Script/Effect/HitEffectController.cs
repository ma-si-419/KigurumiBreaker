using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectController : MonoBehaviour
{
    private int lifeTime = 10;


    void FixedUpdate()
    {
        lifeTime--;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
