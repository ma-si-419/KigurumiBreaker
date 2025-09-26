using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShotEnemyAttackCol : MonoBehaviour
{
    [SerializeField] public float speed;   // ’e‚Ì‘¬“x
    [SerializeField] public float lifeTime; // ’e‚Ìõ–½

    void Start()
    {
        Destroy(gameObject, lifeTime); // ˆê’èŠÔŒã‚É’e‚ğ”j‰ó
    }

    // Update is called once per frame
    void Update()
    {
        // ’e‚ğ‘O•û‚ÉˆÚ“®
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ƒvƒŒƒCƒ„[‚Éƒ_ƒ[ƒW‚ğ—^‚¦‚éˆ—
            Debug.Log("ƒvƒŒƒCƒ„[‚ÉUŒ‚‚µ‚½");
            Destroy(gameObject);    // ’e‚ğíœ
        }
        //else if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        //{
        //    // •Ç‚âáŠQ•¨‚É“–‚½‚Á‚½ê‡‚à’e‚ğíœ
        //    Destroy(gameObject);
        //}

    }

}
