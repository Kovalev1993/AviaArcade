using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private float _speed;
    [SerializeField] private int _damage;

    public int GetDamage()
    {
        return _damage;
    }

    public void DeactivateTrail()
    {
        _trailRenderer.Clear();
    }

    private void FixedUpdate()
    {
        transform.Translate(_speed * transform.forward, Space.World);
    }
}
