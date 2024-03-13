using System.Collections;
using UnityEngine;

public class BumpChecker : MonoBehaviour
{
    [SerializeField] private float _dangerousDistanceToEnemy;
    [SerializeField] private float _checkDistanceToEnemyInterval;
    [SerializeField] private Transform _player;

    private Transform _enemy;

    public void SetEnemy(Transform enemy)
    {
        _enemy = enemy;
        StartCoroutine(CheckDistance());
    }

    private IEnumerator CheckDistance()
    {
        yield return new WaitForSeconds(_checkDistanceToEnemyInterval);

        if (Vector3.Distance(_player.position, _enemy.position) <= _dangerousDistanceToEnemy)
            GameObject.FindGameObjectWithTag("LevelChanger").GetComponent<LevelChanger>().GoToMenu();

        StartCoroutine(CheckDistance());
    }

    public void ClearEnemy()
    {
        _enemy = null;
        StopAllCoroutines();
    }
}
