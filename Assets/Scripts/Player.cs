using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] FloatingJoystick _joystick;
    [SerializeField] float _speed;

    public void FixedUpdate()
    {
        transform.Translate(_speed * _joystick.Direction);
    }
}
