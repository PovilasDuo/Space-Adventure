using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidDestroyer : MonoBehaviour
{
    public float time = 10f;
    void Start()
    {
        Destroy(this.gameObject, time);
    }
}
