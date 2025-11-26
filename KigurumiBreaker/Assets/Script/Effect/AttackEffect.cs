using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    [SerializeField] private int _lifeTime = 99999;

    [SerializeField] private GameObject _effect;

    [SerializeField] private bool _isEffectPop = false;

    private void Awake()
    {
        if (_isEffectPop)
        {
            _effect.GetComponent<AttackEffect>().SetLifeTime(_lifeTime);

            Instantiate(_effect, transform.position, Quaternion.identity);
        }
    }

    private void FixedUpdate()
    {
        _lifeTime--;
        if (_lifeTime <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    public void SetPos(Vector3 pos)
    {
        transform.position = pos;

        if (_isEffectPop)
        {
            _effect.transform.position = pos;
        }
    }

    public void SetGameObject(GameObject obj)
    {
        _effect = obj;

        _isEffectPop = true;
    }
    public void SetLifeTime(int time)
    {
        _lifeTime = time;
    }
}
