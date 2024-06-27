using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFollowFpsTest : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float speed = 1f;

    private void Update()
    {
        Vector3 direction = player.position - transform.position;
        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    private void LateUpdate()
    {
        transform.LookAt(player);
    }
}
