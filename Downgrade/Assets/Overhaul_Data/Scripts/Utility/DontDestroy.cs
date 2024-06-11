using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        DontDestroy[] objs = FindObjectsOfType<DontDestroy>();
        foreach (DontDestroy obj in objs) { if (obj != this) { Destroy(gameObject); return; } }
        DontDestroyOnLoad(gameObject);
    }
}
