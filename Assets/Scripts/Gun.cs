using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
    [SerializeField] UnityEvent _fireStartEvent;
    [SerializeField] UnityEvent _newBulletEvent;
    [SerializeField] UnityEvent _fireStopEvent;
    [SerializeField] float _fireRate;
    [Header("Flash")]
    [SerializeField] GameObject _flashPrefab;
    [SerializeField] float _flashLifetime;
    [Header("Bullet")]
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] float _bulletLifetime;

    private Coroutine _fireCoroutine;
    private Pool<GameObject> _flashesPool;
    private Pool<GameObject> _bulletsPool;
    private float _parentSpeed = 0f;

    public void SetParentSpeed(float parentSpeed)
    {
        _parentSpeed = parentSpeed;
    }

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

        var flash = _flashesPool.Acquire();
        flash.transform.localScale = transform.localScale;

        var bullet = _bulletsPool.Acquire();
        bullet.transform.SetPositionAndRotation(transform.position, transform.rotation);

        _newBulletEvent.Invoke();

        _fireCoroutine = StartCoroutine(Fire());
    }

    private void Start()
    {
        _flashesPool = new Pool<GameObject>(
            () => Instantiate(_flashPrefab, transform),
            (gameObject) => {
                gameObject.SetActive(true);
                StartCoroutine(ReleaseFlash(gameObject));
            },
            (gameObject) => gameObject.SetActive(false)
        );
        _bulletsPool = new Pool<GameObject>(
            () => {
                var bullet = Instantiate(_bulletPrefab);
                bullet.GetComponent<Bullet>().SetParentSpeed(_parentSpeed);
                return bullet;
            },
            (gameObject) => {
                gameObject.SetActive(true);
                StartCoroutine(ReleaseBullet(gameObject));
            },
            (gameObject) => gameObject.SetActive(false)
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
