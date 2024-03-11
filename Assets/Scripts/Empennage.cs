using UnityEngine;

public class Empennage : MonoBehaviour
{
    [SerializeField] private Transform _planeModel;
    [SerializeField, Min(0)] private float _maxRoll;
    [SerializeField] private float _maxYaw;
    [SerializeField] private Animator _animator;
    [SerializeField] private string _eleronsAnimationName;
    [SerializeField] private Transform _rudder;
    [SerializeField] private Vector3 _rudderRotationMax;

    private void FixedUpdate()
    {
        //_rudder.rotation = Quaternion.Euler(-90, 0, Mathf.Clamp(_planeModel.rotation.eulerAngles.z, -_maxRoll, _maxRoll) / _maxRoll);

        var angleZ = _planeModel.rotation.eulerAngles.z < 180 ? _planeModel.rotation.eulerAngles.z : -360 + _planeModel.rotation.eulerAngles.z;
        var normalizedTime = (Mathf.Clamp(angleZ, -_maxRoll, _maxRoll) + _maxRoll) / (2 * _maxRoll);
        _animator.Play(0, 1, 1f - normalizedTime);
        _animator.Play(0, 2, normalizedTime);
    }
}
