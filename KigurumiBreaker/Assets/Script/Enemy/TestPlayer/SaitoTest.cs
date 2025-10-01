using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaitoTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            Debug.Log("çUåÇÇ≥ÇÍÇΩ");
        }
    }
}
