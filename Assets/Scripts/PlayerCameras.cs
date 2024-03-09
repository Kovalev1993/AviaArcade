using Cinemachine;
using UnityEngine;

public class PlayerCameras : MonoBehaviour
{
    [SerializeField] private int _averagePriority;
    [SerializeField] private CinemachineVirtualCamera _commonCamera;
    [SerializeField] private CinemachineVirtualCamera _lightFireCamera;
    [SerializeField] private CinemachineVirtualCamera _heavyFireCamera;
    [SerializeField] private float _lightImpulseMagnitude;
    [SerializeField] private CinemachineImpulseSource _lightFireImpulceSource;
    [SerializeField] private float _heavyImpulseMagnitude;
    [SerializeField] private CinemachineImpulseSource _hevyFireImpulceSource;


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

    public void MakeLightFireImpulse()
    {
        _lightFireImpulceSource.GenerateImpulse(_lightImpulseMagnitude * Random.onUnitSphere);
    }

    public void MakeHeavyFireImpulse()
    {
        _hevyFireImpulceSource.GenerateImpulse(_heavyImpulseMagnitude * Random.onUnitSphere);
    }
}
