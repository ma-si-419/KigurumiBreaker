using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private List<GameObject> _enemies = new List<GameObject>();
    public List<GameObject> enemies => _enemies;

    private List<GameObject> _enemyAttacks = new List<GameObject>();

    [SerializeField] private GameObject _player;

    [SerializeField] private GameObject _camera;

    private PlayerState _playerState;

    private CameraMove _cameraMove;
    public GameObject player => _player;

    private bool _isStop = false;
    private int _stopFrame = 0;

    private void Start()
    {
        _playerState = _player.GetComponent<PlayerState>();

        _cameraMove = _camera.GetComponent<CameraMove>();
    }


    private void FixedUpdate()
    {
        if (_isStop)
        {
            _stopFrame--;

            if (_stopFrame < 0)
            {
                _isStop = false;

                // ヒットストップ解除
                _playerState.SetStop(false);
                _playerState.StartAnimation();
                _cameraMove.SetStop(false);

                foreach (GameObject enemy in enemies)
                {
                    EnemyBase sc = enemy.GetComponent<EnemyBase>();
                    sc.SetStop(false);
                    sc.StartAnimation();
                }

                if (_enemyAttacks.Count > 0)
                {
                    Debug.Log("数" + _enemyAttacks.Count);

                    foreach (GameObject enemyAttack in _enemyAttacks)
                    {
                        enemyAttack.GetComponent<EnemyAttackCol>().SetStop(false);
                    }
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

    public void AddEnemyAttack(GameObject enemyAttack)
    {
        _enemyAttacks.Add(enemyAttack);
    }
    public void RemoveEnemyAttack(GameObject enemyAttack)
    {
        int enemyNum = _enemyAttacks.Count;

        _enemyAttacks.Remove(enemyAttack);

        int enemyNumAfter = _enemyAttacks.Count;

        Debug.Log("RemoveEnemyAttack:敵の攻撃判定を削除しました。削除前の数:" + enemyNum + "削除後の数:" + enemyNumAfter);
    }


    public void StopTime(int time)
    {
        if (time <= 0) return;

        _isStop = true;
        _stopFrame = time;

        // ヒットストップ開始
        _playerState.SetStop(true);
        _playerState.StopAnimation();
        _cameraMove.SetStop(true);

        foreach (GameObject enemy in enemies)
        {
            EnemyBase sc = enemy.GetComponent<EnemyBase>();
            sc.SetStop(true);
            sc.StopAnimation();
        }

        if (_enemyAttacks.Count > 0)
        {
            foreach (GameObject enemyAttack in _enemyAttacks)
            {
                enemyAttack.GetComponent<EnemyAttackCol>().SetStop(true);
            }
        }

    }
}
