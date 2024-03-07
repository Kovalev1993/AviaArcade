using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Moving")]
    [SerializeField] FloatingJoystick _movingJoystick;
    [SerializeField] float _speed;
    [Header("Fire")]
    [SerializeField] FloatingJoystick _fireJoystick;
    [SerializeField] GameObject _lightFlashPrefab;
    [SerializeField] Transform[] _lightFlashSpawnPoints = new Transform[2];
    [SerializeField] float _lightFireDelay;
    [SerializeField] float _lightFireLifetime;
    [SerializeField] GameObject _heavyFlashPrefab;
    [SerializeField] Transform[] _heavyFlashSpawnPoints = new Transform[2];
    [SerializeField] float _heavyFireDelay;
    [SerializeField] float _heavyFireLifetime;

    private Coroutine _fireCoroutine;
    private Pool<GameObject> _lightFlashesPool;
    private Pool<GameObject> _heavyFlashesPool;

    private void Start()
    {
        _lightFlashesPool = new Pool<GameObject>(
            () => Instantiate(_lightFlashPrefab, transform),
            (gameObject) => {
                gameObject.SetActive(true);
                StartCoroutine(ReleaseLightFlash(gameObject));
            },
            (gameObject) => gameObject.SetActive(false)
        );
        _heavyFlashesPool = new Pool<GameObject>(
            () => Instantiate(_heavyFlashPrefab, transform),
            (gameObject) => {
                gameObject.SetActive(true);
                StartCoroutine(ReleaseHeavyFlash(gameObject));
            },
            (gameObject) => gameObject.SetActive(false)
        );
    }

    private IEnumerator ReleaseLightFlash(GameObject item)
    {
        yield return new WaitForSeconds(_lightFireLifetime);
        _lightFlashesPool.Release(item);
    }

    private IEnumerator ReleaseHeavyFlash(GameObject item)
    {
        yield return new WaitForSeconds(_heavyFireLifetime);
        _heavyFlashesPool.Release(item);
    }

    private void FixedUpdate()
    {
        transform.Translate(_speed * _movingJoystick.Direction);
    }

    private void Update()
    {
        HandleFire();
    }

    private void HandleFire()
    {
        if (_fireCoroutine == null)
        {
            if (_fireJoystick.Vertical == -1)
                _fireCoroutine = StartCoroutine(FireHeavy());
            else if (_fireJoystick.Vertical == 1)
                _fireCoroutine = StartCoroutine(FireLight());
        }
        else if (_fireJoystick.Vertical == 0)
        {
            StopCoroutine(_fireCoroutine);
            _fireCoroutine = null;
        }
    }

    private IEnumerator FireLight()
    {
        yield return new WaitForSeconds(_lightFireDelay);
        foreach (var spawnPoint in _lightFlashSpawnPoints)
        {
            var flash = _lightFlashesPool.Acquire();
            flash.transform.localPosition = spawnPoint.localPosition;
            flash.transform.localRotation = spawnPoint.localRotation;
            flash.transform.localScale = spawnPoint.localScale;
        }
        _fireCoroutine = StartCoroutine(FireLight());
    }

    private IEnumerator FireHeavy()
    {
        yield return new WaitForSeconds(_heavyFireDelay);
        foreach (var spawnPoint in _heavyFlashSpawnPoints)
        {
            var flash = _heavyFlashesPool.Acquire();
            flash.transform.localPosition = spawnPoint.localPosition;
            flash.transform.localRotation = spawnPoint.localRotation;
            flash.transform.localScale = spawnPoint.localScale;
        }
        _fireCoroutine = StartCoroutine(FireHeavy());
    }
}
