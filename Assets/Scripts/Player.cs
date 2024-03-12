using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform _model;
    [SerializeField] private Empennage _empennage;
    [Header("Moving")]
    [SerializeField] private FloatingJoystick _movingJoystick;
    [SerializeField] private float _rotationDeviationMax;
    [SerializeField] private float _deviatonSpeed;
    [SerializeField] private float _returnToLocalZeroDuration;
    [SerializeField] private Ease _returnToLocalZeroEasing;
    [Header("Fire")]
    [SerializeField] private FloatingJoystick _fireJoystick;
    [SerializeField] private Gun[] _lightGuns = new Gun[2];
    [SerializeField] private Gun[] _heavyGuns = new Gun[2];

    private Tween _moveToLocalZeroTween;
    private Tween _rotateToLocalZeroTween;

    private void Start()
    {
        _empennage.SetJoystick(_movingJoystick);
    }

    private void FixedUpdate()
    {
        HandleMoving();
    }

    private void HandleMoving()
    {
        if (_movingJoystick.Horizontal == 0 && _movingJoystick.Vertical == 0)
        {
            if (!_moveToLocalZeroTween.IsActive())
                _moveToLocalZeroTween = _model.DOLocalMove(Vector3.zero, _returnToLocalZeroDuration)
                    .SetEase(_returnToLocalZeroEasing)
                    .Play();

            if (!_rotateToLocalZeroTween.IsActive())
                _rotateToLocalZeroTween = _model.DOLocalRotate(Vector3.zero, _returnToLocalZeroDuration)
                    .SetEase(_returnToLocalZeroEasing)
                    .Play();
        }
        else
        {
            if (_moveToLocalZeroTween.IsActive())
                _moveToLocalZeroTween.Kill();
            if (_rotateToLocalZeroTween.IsActive())
                _rotateToLocalZeroTween.Kill();

            _model.localRotation = Quaternion.Euler(
                -_rotationDeviationMax * _movingJoystick.Direction.y, _rotationDeviationMax * _movingJoystick.Direction.x, 0
            );
            _model.transform.Translate(_deviatonSpeed * _movingJoystick.Direction);
        }
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
