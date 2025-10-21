using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private List<GameObject> _enemies = new List<GameObject>();
    public List<GameObject> enemies => _enemies;

    [SerializeField] private GameObject _player;

    private PlayerState _playerState;
    public GameObject player => _player;

    private bool _isStop = false;
    private int _stopFrame = 0;

    private void Start()
    {
        _playerState = _player.GetComponent<PlayerState>();
    }


    private void FixedUpdate()
    {
        if (_isStop)
        {
            _stopFrame--;

            if (_stopFrame < 0)
            {
                _isStop = false;

                _playerState.SetStop(false);

                _playerState.StartAnimation();

                foreach (GameObject enemy in enemies)
                {
                    enemy.GetComponent<ZangiMove>().SetStop(false);
                }
            }
        }
    }


    public void AddEnemy(GameObject enemy)
    {
        _enemies.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        _enemies.Remove(enemy);
    }

    public void StopTime(int time)
    {
        _isStop = true;
        _stopFrame = time;

        _playerState.SetStop(true);

        _playerState.StopAnimation();

        foreach(GameObject enemy in enemies)
        {
            enemy.GetComponent<ZangiMove>().SetStop(true);
        }

    }
}
