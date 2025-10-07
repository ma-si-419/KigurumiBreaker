using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    //アニメーションを制御するメソッド
    public void PlayIdle(bool isIdle)
    {
        _animator.SetBool("Idle", isIdle);
    }

    public void PlayChase(bool isChasing)
    {
        _animator.SetBool("Chase", isChasing);
    }

    public void PlayAttack(bool isAttack)
    {
        _animator.SetBool("Attack", isAttack);
    }

    public void PlayDeath(bool isDeath)
    {
        _animator.SetBool("Death", isDeath);
    }

    public void PlayDamage(bool isDamage)
    {
        _animator.SetBool("Damage", isDamage);
    }

}
