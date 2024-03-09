using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // PlaceHolder for the PlayerController

    [SerializeField] float speed = 5f;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector2 direction = InputDirection.instance.GetDirection().normalized;

        rb.velocity = new Vector3(direction.x, 0, direction.y) * speed;
    }
}
