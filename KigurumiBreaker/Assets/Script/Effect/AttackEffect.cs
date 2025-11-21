using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    private int _lifeTime = 60;

    [SerializeField] private GameObject _effect;

    private void Awake()
    {
        if (_effect != null)
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
        _effect.transform.position = pos;
    }

    public void SetGameObject(GameObject obj)
    {
        _effect = obj;
    }
    public void SetLifeTime(int time)
    {
        _lifeTime = time;
    }
}
