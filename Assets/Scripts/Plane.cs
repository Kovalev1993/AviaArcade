using UnityEngine;

public class Plane : MonoBehaviour
{
    [SerializeField] private float _speed;

    public float GetSpeed()
    {
        return _speed;
    }

    private void FixedUpdate()
    {
        transform.Translate(_speed * transform.forward);
    }
}
