using Dreamteck.Splines;
using System.Collections;
using UnityEngine;

public class EnemiesRespawner : MonoBehaviour
{
    [SerializeField] SplineComputer _splineComputer;
    [SerializeField] PlayerCameras _playerCameras;
    [SerializeField] SplineFollower _playerSplineFollower;
    [SerializeField] GameObject _enemyPrefab;
    [SerializeField] float _respawnDelay;
    [SerializeField] float _releaseDelay;
    [SerializeField] float percentageDeltaToPlayer;

    private Pool<GameObject> _enemiesPool;
    private GameObject _currentEnemy;

    private void Start()
    {
        _enemiesPool = new Pool<GameObject>(
            () => Instantiate(_enemyPrefab),
            (gameObject) => {
                var plane = gameObject.GetComponent<Plane>();
                plane.ResetPlane(_splineComputer, (float)_playerSplineFollower.GetPercent());
                plane.GetFinishDisplacementEvent().AddListener(() => {
                    _playerCameras.StartEnemyAiming(gameObject.transform);
                });
                plane.ExplosionEvent.AddListener(_playerCameras.StopEnemyAiming);
                plane.ExplosionEvent.AddListener(ReleaseEnemyAfterDelay);

                gameObject.SetActive(true);
            },
            (gameObject) => {
                var plane = gameObject.GetComponent<Plane>();
                plane.GetFinishDisplacementEvent().RemoveAllListeners();
                plane.ExplosionEvent.RemoveAllListeners();
                gameObject.SetActive(false);
                SpawnNewEnemyAfterDelay();
            }
        );

        SpawnNewEnemyAfterDelay();
    }

    private void SpawnNewEnemyAfterDelay()
    {
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(_respawnDelay);
        _currentEnemy = _enemiesPool.Acquire();
    }

    private void ReleaseEnemyAfterDelay()
    {
        StartCoroutine(ReleaseEnemy());
    }

    private IEnumerator ReleaseEnemy()
    {
        yield return new WaitForSeconds(_releaseDelay);
        _enemiesPool.Release(_currentEnemy);
    }
}
