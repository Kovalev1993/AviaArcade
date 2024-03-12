using UnityEngine;

public class Empennage : MonoBehaviour
{
    [SerializeField] private Transform _planeModel;
    [Header("Rudder")]
    [SerializeField] private float _maxRudderYaw;
    [SerializeField] private Transform _rudder;
    [Header("Elevator")]
    [SerializeField] private float _maxElevatorPitch;
    [SerializeField] private Transform _elevator;
    [Header("Elerons")]
    [SerializeField, Min(0)] private float _maxRoll;
    [SerializeField] private Transform _eleronLeft;
    [SerializeField] private Vector3 _eleronLeftRotationMin;
    [SerializeField] private Vector3 _eleronLeftRotationMax;
    [SerializeField] private Transform _eleronRight;
    [SerializeField] private Vector3 _eleronRightRotationMin;
    [SerializeField] private Vector3 _eleronRightRotationMax;

    private FloatingJoystick _movingJoystick;

    public void SetJoystick(FloatingJoystick movingJoystick)
    {
        _movingJoystick = movingJoystick;
    }

    private void FixedUpdate()
    {
        var angleZ = _planeModel.rotation.eulerAngles.z < 180 ? _planeModel.rotation.eulerAngles.z : -360 + _planeModel.rotation.eulerAngles.z;
        var normalizedTime = (Mathf.Clamp(angleZ, -_maxRoll, _maxRoll) + _maxRoll) / (2 * _maxRoll);
        _eleronLeft.localRotation = Quaternion.Lerp(
            Quaternion.Euler(_eleronLeftRotationMin),
            Quaternion.Euler(_eleronLeftRotationMax),
            normalizedTime
        );
        _eleronRight.localRotation = Quaternion.Lerp(
            Quaternion.Euler(_eleronRightRotationMax),
            Quaternion.Euler(_eleronRightRotationMin),
            normalizedTime
        );

        if (_movingJoystick != null)
        {
            _rudder.localRotation = Quaternion.Euler(-90, 0, -_movingJoystick.Horizontal * _maxRudderYaw);
            _elevator.localRotation = Quaternion.Euler(-90 - _movingJoystick.Vertical * _maxElevatorPitch, 0, 0);
        }
    }
}
