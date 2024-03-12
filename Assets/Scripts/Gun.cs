using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
    [SerializeField] private UnityEvent _fireStartEvent;
    [SerializeField] private UnityEvent _newBulletEvent;
    [SerializeField] private UnityEvent _fireStopEvent;
    [SerializeField] private float _fireRate;
    [Header("Flash")]
    [SerializeField] private GameObject _flashPrefab;
    [SerializeField] private float _flashLifetime;
    [Header("Bullet")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletLifetime;

    private Coroutine _fireCoroutine;
    private Pool<GameObject> _flashesPool;
    private Pool<GameObject> _bulletsPool;

    public void StartFire()
    {
        if (_fireCoroutine != null)
            return;

        _fireCoroutine = StartCoroutine(Fire());
        _fireStartEvent.Invoke();
    }

    public void StopFire()
    {
        if (_fireCoroutine == null)
            return;

        StopCoroutine(_fireCoroutine);
        _fireCoroutine = null;
        _fireStopEvent.Invoke();
    }

    private IEnumerator Fire()
    {
        yield return new WaitForSeconds(_fireRate);

        _flashesPool.Acquire();
        _bulletsPool.Acquire();

        _fireCoroutine = StartCoroutine(Fire());
    }

    private void Start()
    {
        _flashesPool = new Pool<GameObject>(
            () => {
                var flash = Instantiate(_flashPrefab, transform);
                flash.transform.localScale = transform.localScale;
                return flash;
            },
            (gameObject) => {
                gameObject.SetActive(true);
                StartCoroutine(ReleaseFlash(gameObject));
            },
            (gameObject) => gameObject.SetActive(false)
        );
        _bulletsPool = new Pool<GameObject>(
            () =>  Instantiate(_bulletPrefab),
            (gameObject) => {
                gameObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
                gameObject.SetActive(true);
                _newBulletEvent.Invoke();
                StartCoroutine(ReleaseBullet(gameObject));
            },
            (gameObject) => {
                gameObject.GetComponent<Bullet>().DeactivateTrail();
                gameObject.SetActive(false);
            }
        );
    }

    private IEnumerator ReleaseFlash(GameObject item)
    {
        yield return new WaitForSeconds(_flashLifetime);
        _flashesPool.Release(item);
    }

    private IEnumerator ReleaseBullet(GameObject item)
    {
        yield return new WaitForSeconds(_bulletLifetime);
        _bulletsPool.Release(item);
    }
}
