using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoController : MonoBehaviour
{
    [Header("Echo Behaviour")]
    [SerializeField] float speed;
    [SerializeField] float lifeTime;

    [Header("Extra Options")]
    [Tooltip("Adds a random rotation to the Y axis when instantiated")]
    [SerializeField] bool randomRotation;

    private void Awake()
    {
        if (randomRotation)
        {
            float randomY = Random.Range(0, 360);
            transform.Rotate(0, randomY, 0);
        }

        DestroyObject();
    }

    private void Update()
    {
        Vector3 vectorMesh = transform.localScale;
        float growing = speed * Time.deltaTime;
        transform.localScale = new Vector3(vectorMesh.x + growing, vectorMesh.y + growing, vectorMesh.z + growing);
    }

    private void DestroyObject()
    {
        Destroy(gameObject, lifeTime);
    }
}
