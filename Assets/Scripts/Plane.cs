using Unity.VisualScripting;
using UnityEngine;

public class Plane : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private Transform _propeller;
    [SerializeField] private Vector3 _propellerRotationSpeed;

    public float GetSpeed()
    {
        return _movementSpeed;
    }

    private void Start()
    {
        _propeller.Rotate(Random.value * 360 * _propellerRotationSpeed);
    }

    private void FixedUpdate()
    {
        //transform.Translate(_movementSpeed * transform.forward);
        _propeller.Rotate(_propellerRotationSpeed);
    }
}
