using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovieEnemy : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            animator.SetTrigger("B");
        }
    }

}
