using Dreamteck.Splines;
using System.Collections;
using UnityEngine;

public class Plane : MonoBehaviour
{
    [SerializeField] private SplineFollower _splineFollower;
    [SerializeField] private Transform _propeller;
    [SerializeField] private Vector3 _propellerRotationSpeed;
    [Header("Impact")]
    [SerializeField] GameObject _impactPrefab;
    [SerializeField] float _impactLifetime;
    [SerializeField] float _impactScale;

    private Pool<GameObject> _impactsPool;

    private void Start()
    {
        _propeller.Rotate(Random.value * 360 * _propellerRotationSpeed);

        _impactsPool = new Pool<GameObject>(
            () => {
                var impact = Instantiate(_impactPrefab, transform);
                impact.transform.localScale = _impactScale * Vector3.one;
                return impact;
            },
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
        var impact = _impactsPool.Acquire();
        impact.transform.position = collision.GetContact(0).point;
        collision.gameObject.SetActive(false);
    }
}
