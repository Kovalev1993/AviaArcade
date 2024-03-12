using Dreamteck.Splines;
using System.Collections;
using UnityEngine;

public class EnemiesRespawner : MonoBehaviour
{
    [SerializeField] private SplineComputer _splineComputer;
    [SerializeField] private PlayerCameras _playerCameras;
    [SerializeField] private SplineFollower _playerSplineFollower;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _respawnDelay;
    [SerializeField] private float _releaseDelay;
    [SerializeField] private float percentageDeltaToPlayer;

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
                plane.ExplosionEvent.AddListener(() => {
                    _playerCameras.StopEnemyAiming(gameObject.transform);
                });
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
