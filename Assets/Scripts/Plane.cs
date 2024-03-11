using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Plane : MonoBehaviour
{
    [HideInInspector] public UnityEvent ExplosionEvent = new();

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private SplineFollower _splineFollower;
    [SerializeField] private Transform _propeller;
    [SerializeField] private Vector3 _propellerRotationSpeed;
    [SerializeField] private MeshRenderer _bodyMeshRenderer;
    [SerializeField] private int _maxHealth;
    [SerializeField] private Transform _centerOfMass;
    [Header("Damage effects")]
    [SerializeField] GameObject _impactPrefab;
    [SerializeField] float _impactLifetime;
    [SerializeField] GameObject _explosion;
    [SerializeField] GameObject _smoke;
    [SerializeField] float _maxTorqueRoll;
    [SerializeField] float _torquePitch;
    [SerializeField] private List<Collider> _colliders = new();

    private Pool<GameObject> _impactsPool;
    private int _currentHealth;
    private float _bodyWidth;

    public void ResetPlane()
    {
        _currentHealth = _maxHealth;
        _explosion.SetActive(false);
        _smoke.SetActive(false);
        _splineFollower.enabled = true;
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        foreach (var collider in _colliders)
        {
            collider.enabled = true;
        }
    }

    private void Start()
    {
        ResetPlane();

        _propeller.Rotate(Random.value * 360 * _propellerRotationSpeed);

        _impactsPool = new Pool<GameObject>(
            () => Instantiate(_impactPrefab, transform),
            (gameObject) => {
                gameObject.SetActive(true);
                StartCoroutine(ReleaseImact(gameObject));
            },
            (gameObject) => gameObject.SetActive(false)
        );

        _bodyWidth = _bodyMeshRenderer.bounds.size.x;
        _rigidbody.centerOfMass = _centerOfMass.localPosition;
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
        if (_currentHealth < 0)
            return;

        _currentHealth -= damage;
        if (_currentHealth < 0)
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
        var inversedPosition = transform.InverseTransformPoint(position);
        _rigidbody.AddTorque(_maxTorqueRoll * inversedPosition.x / _bodyWidth, 0f, _torquePitch);
        foreach (var collider in _colliders)
        {
            collider.enabled = false;
        }
        ExplosionEvent.Invoke();
    }
}