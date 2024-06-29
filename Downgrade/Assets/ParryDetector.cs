using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryDetector : MonoBehaviour
{
    public Transform objectB;
    public Transform hitboxCenter;
    public float parryDetectionAngle = 45f;

    void OnDrawGizmos()
    {
        if (objectB != null)
        {
            // Dirección adelante del hitbox
            Vector3 forwardA = hitboxCenter.forward;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(hitboxCenter.position, hitboxCenter.position + forwardA * 5f);

            // Dirección hacia el objeto B
            Vector3 directionToB = (objectB.position - transform.position).normalized;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + directionToB * 5f);

            // Ángulo de detección de parry
            Vector3 rightBoundary = Quaternion.Euler(0, parryDetectionAngle / 2, 0) * forwardA;
            Vector3 leftBoundary = Quaternion.Euler(0, -parryDetectionAngle / 2, 0) * forwardA;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(hitboxCenter.position, hitboxCenter.position + rightBoundary * 5f);
            Gizmos.DrawLine(hitboxCenter.position, hitboxCenter.position + leftBoundary * 5f);

            // Calcular el ángulo y determinar si está dentro del rango
            float angle = Vector3.Angle(forwardA, directionToB);
            if (angle < parryDetectionAngle / 2)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, objectB.position);
            }
        }
    }
}
