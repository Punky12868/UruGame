using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meh : MonoBehaviour
{
    [SerializeField] AspectRatioUtility a;

    private void Awake()
    {
        a = FindObjectOfType<AspectRatioUtility>();
    }
}
