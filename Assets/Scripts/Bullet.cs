using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;

    private float _parentSpeed = 0f;

    public void SetParentSpeed(float parentSpeed)
    {
        _parentSpeed = parentSpeed;
    }

    private void FixedUpdate()
    {
        transform.Translate((_speed + _parentSpeed) * transform.forward);        
    }
}
