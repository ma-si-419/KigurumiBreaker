using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private List<GameObject> _enemies = new List<GameObject>();
    public List<GameObject> enemies => _enemies;

    [SerializeField] private GameObject _player; 

    public GameObject player => _player;

    public void AddEnemy(GameObject enemy)
    {
        _enemies.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy) 
    {
        _enemies.Remove(enemy);
    }

}
