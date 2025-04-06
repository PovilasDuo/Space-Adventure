using UnityEngine;

public class AsteroidDestroyer : MonoBehaviour
{
    public float time = 15f;
    void Start()
    {
        Destroy(this.gameObject, time);
    }
}
