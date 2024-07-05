using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteItself : MonoBehaviour
{
    public float timeToLive = 2.0f;

    // Start is called before the first frame update
    void Awake()
    {
        Destroy(gameObject, timeToLive);
    }
}
