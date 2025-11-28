using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleManager : MonoBehaviour
{
    private List<GameObject> _enemies = new List<GameObject>();
    public List<GameObject> enemies => _enemies;

    private List<GameObject> _enemyAttacks = new List<GameObject>();

    private List<GameObject> _playerAttacks = new List<GameObject>();

    [SerializeField] private GameObject _player;

    [SerializeField] private GameObject _camera;

    private PlayerState _playerState;

    private CameraMove _cameraMove;
    public GameObject player => _player;

    private bool _isStop = false;
    private int _stopFrame = 0;

    private bool _isSlow = false;

    private float _slowFrame;

    private void Start()
    {
        _playerState = _player.GetComponent<PlayerState>();

        _cameraMove = _camera.GetComponent<CameraMove>();
    }

    private void FixedUpdate()
    {
        // スローモーション処理
        if (_isSlow)
        {
            _slowFrame--;

            if (_slowFrame < 0.0f)
            {
                _isSlow = false;
                Time.timeScale = 1.0f;
            }
        }

        // ヒットストップ処理
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

    public void OnMoveStage()
    {
        // ステージ移動時に残っている敵の攻撃とプレイヤーの攻撃をすべて削除する
        foreach (GameObject enemyAttack in _enemyAttacks)
        {
            Destroy(enemyAttack);
        }

        _enemyAttacks.Clear();

        foreach (GameObject playerAttack in _playerAttacks)
        {
            Destroy(playerAttack);
        }

        _playerAttacks.Clear();

        // エフェクトをすべて削除する
        


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
    public void AddPlayerAttack(GameObject playerAttack)
    {
        _playerAttacks.Add(playerAttack);
    }
    public void RemoveEnemyAttack(GameObject enemyAttack)
    {
        _enemyAttacks.Remove(enemyAttack);
    }
    public void RemovePlayerAttack(GameObject playerAttack)
    {
        _playerAttacks.Remove(playerAttack);
    }

    public void AddPlayerSpecialGauge(float addNum)
    {
        _playerState.AddSpecialGauge(addNum);
    }

    public void SlowTime(float time, float timeScale)
    {
        if (time <= 0) return;

        _slowFrame = time * timeScale;
        _isSlow = true;

        Time.timeScale = timeScale;
    }

    public void SetHitStop(int time)
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
