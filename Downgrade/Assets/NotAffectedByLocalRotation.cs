using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotAffectedByLocalRotation : MonoBehaviour
{
    [SerializeField] private bool useInitialRotationAtAwake = false;
    private Quaternion initialRotation;

    private void Awake()
    {
        if (useInitialRotationAtAwake) { initialRotation = transform.rotation; return; }
        initialRotation = Quaternion.identity;
    }

    private void LateUpdate()
    {
        transform.rotation = initialRotation;
    }
}
