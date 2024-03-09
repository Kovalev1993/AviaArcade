using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] private float _lifetime;

    public float GetLifetime()
    {
        return _lifetime;
    }
}
