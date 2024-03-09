using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform _enemy;
    [SerializeField] private Plane _plane;
    [Header("Moving")]
    [SerializeField] FloatingJoystick _movingJoystick;
    [SerializeField] float _speed;
    [Header("Fire")]
    [SerializeField] FloatingJoystick _fireJoystick;
    [SerializeField] Gun[] _lightGuns = new Gun[2];
    [SerializeField] Gun[] _heavyGuns = new Gun[2];

    private void Start()
    {
        foreach (var gun in _lightGuns)
        {
            gun.SetParentSpeed(_plane.GetSpeed());
        }
        foreach (var gun in _heavyGuns)
        {
            gun.SetParentSpeed(_plane.GetSpeed());
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(_speed * _movingJoystick.Direction, Space.World);
        transform.LookAt(_enemy);
    }

    private void Update()
    {
        HandleFire();
    }

    private void HandleFire()
    {
        if (_fireJoystick.Vertical == -1)
        {
            foreach (var gun in _lightGuns)
            {
                gun.StopFire();
            }
            foreach (var gun in _heavyGuns)
            {
                gun.StartFire();
            }
        }
        else if (_fireJoystick.Vertical == 0)
        {
            foreach (var gun in _lightGuns)
            {
                gun.StopFire();
            }
            foreach (var gun in _heavyGuns)
            {
                gun.StopFire();
            }
        }
        else if (_fireJoystick.Vertical == 1)
        {
            foreach (var gun in _lightGuns)
            {
                gun.StartFire();
            }
            foreach (var gun in _heavyGuns)
            {
                gun.StopFire();
            }
        }
    }
}
