using Dreamteck.Splines;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Plane : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private SplineFollower _splineFollower;
    [SerializeField] private Transform _propeller;
    [SerializeField] private Vector3 _propellerRotationSpeed;
    [SerializeField] private int _health;
    [SerializeField] private UnityEvent _explosionEvent = new();
    [Header("Damage effects")]
    [SerializeField] GameObject _impactPrefab;
    [SerializeField] float _impactLifetime;
    [SerializeField] GameObject _explosion;
    [SerializeField] GameObject _smoke;

    private Pool<GameObject> _impactsPool;

    private void Start()
    {
        _propeller.Rotate(Random.value * 360 * _propellerRotationSpeed);

        _impactsPool = new Pool<GameObject>(
            () => Instantiate(_impactPrefab, transform),
            (gameObject) => {
                gameObject.SetActive(true);
                StartCoroutine(ReleaseImact(gameObject));
            },
            (gameObject) => gameObject.SetActive(false)
        );
    }

    private IEnumerator ReleaseImact(GameObject item)
    {
        yield return new WaitForSeconds(_impactLifetime);
        _impactsPool.Release(item);
    }

    private void FixedUpdate()
    {
        _propeller.Rotate(_propellerRotationSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Bullet>(out var bullet))
        {
            HideBullet(bullet.gameObject);
            CreateImpact(collision.GetContact(0).point);
            TakeDamage(bullet.GetDamage(), collision.GetContact(0).point);
        }
    }

    private void HideBullet(GameObject bullet)
    {
        bullet.SetActive(false);
    }

    private void CreateImpact(Vector3 position)
    {
        var impact = _impactsPool.Acquire();
        impact.transform.position = position;
    }

    private void TakeDamage(int damage, Vector3 position)
    {
        if (_health < 0)
            return;

        _health -= damage;
        if (_health < 0)
            Explode(position);
    }

    private void Explode(Vector3 position)
    {
        _explosion.transform.position = position;
        _explosion.SetActive(true);
        _smoke.transform.position = position;
        _smoke.SetActive(true);
        _splineFollower.enabled = false;
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        _explosionEvent.Invoke();
    }
}
