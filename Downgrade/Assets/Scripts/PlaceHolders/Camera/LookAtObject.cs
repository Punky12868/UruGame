using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    // PlaceHolder for the point where the camera will look at

    Transform player;
    Transform scenario;
    //Transform[] enemies;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        scenario = GameObject.FindGameObjectWithTag("Scenario").transform;
    }

    private void Update()
    {
        // scenario and player only
        Vector3 pos = (player.position - scenario.position).normalized;

        if (transform.position != pos)
        {
            transform.position = pos;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.15f);
    }
}
