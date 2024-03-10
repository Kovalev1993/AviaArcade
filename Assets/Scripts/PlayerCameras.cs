using Cinemachine;
using UnityEngine;

public class PlayerCameras : MonoBehaviour
{
    [SerializeField] private int _averagePriority;
    [SerializeField] private CinemachineVirtualCamera _commonCamera;
    [SerializeField] private CinemachineVirtualCamera _lightFireCamera;
    [SerializeField] private CinemachineVirtualCamera _heavyFireCamera;
    [SerializeField] private float _heavyImpulseMagnitude;
    [SerializeField] private CinemachineImpulseSource _hevyFireImpulceSource;

    public void StopEnemyAiming()
    {
        _commonCamera.LookAt = _commonCamera.Follow;
    }

    public void StartEnemyAiming(Transform enemyTransform)
    {
        _commonCamera.LookAt = enemyTransform;
    }

    public void PrioritizeLightFireCamera()
    {
        _lightFireCamera.Priority = _averagePriority + 1;
        _heavyFireCamera.Priority = _averagePriority;
        _commonCamera.Priority = _averagePriority;
    }

    public void PrioritizeHeavyFireCamera()
    {
        _lightFireCamera.Priority = _averagePriority;
        _heavyFireCamera.Priority = _averagePriority + 1;
        _commonCamera.Priority = _averagePriority;
    }

    public void PrioritizeCommonCamera()
    {
        _lightFireCamera.Priority = _averagePriority;
        _heavyFireCamera.Priority = _averagePriority;
        _commonCamera.Priority = _averagePriority + 1;
    }

    public void MakeHeavyFireImpulse()
    {
        _hevyFireImpulceSource.GenerateImpulse(_heavyImpulseMagnitude * Random.onUnitSphere);
    }
}
