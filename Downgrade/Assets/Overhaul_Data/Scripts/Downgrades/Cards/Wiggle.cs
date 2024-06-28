using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wiggle : MonoBehaviour
{
    [SerializeField] float wiggleDistance = 1;
    [SerializeField] float wiggleSpeed = 1;

    void Update()
    {
        float yPosition = Mathf.Sin(Time.time * wiggleSpeed) * wiggleDistance;
        float xPosition = Mathf.Cos(Time.time * wiggleSpeed) * wiggleDistance;
        transform.localPosition = new Vector3(xPosition, yPosition, 0);
    }
}