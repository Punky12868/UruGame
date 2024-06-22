using UnityEngine;
using System.Collections.Generic;

public class ReflectSprite : MonoBehaviour
{
    [SerializeField] private float reflectionOffset = 0f;
    [SerializeField] private Material reflectionMaterial;
    [SerializeField] private Material spriteReflectionMaterial;

    private struct ReflectableObject
    {
        public GameObject original;
        public GameObject reflection;

        public ReflectableObject(GameObject original, GameObject reflection)
        {
            this.original = original;
            this.reflection = reflection;
        }
    }

    private List<ReflectableObject> reflectableObjects;

    void Start()
    {
        reflectableObjects = new List<ReflectableObject>();
        ReflectionComponent[] objectsToReflect = FindObjectsOfType<ReflectionComponent>();

        foreach (ReflectionComponent obj in objectsToReflect)
        {
            GameObject reflection = CreateReflection(obj.gameObject);
            reflectableObjects.Add(new ReflectableObject(obj.gameObject, reflection));
        }
    }

    void Update() { foreach (var obj in reflectableObjects) UpdateReflection(obj.original, obj.reflection); }

    GameObject CreateReflection(GameObject originalObject)
    {
        GameObject reflection = new GameObject(originalObject.name + "_Reflection");

        if (originalObject.GetComponent<SpriteRenderer>() != null)
        {
            SpriteRenderer originalSpriteRenderer = originalObject.GetComponent<SpriteRenderer>();
            SpriteRenderer reflectionSpriteRenderer = reflection.AddComponent<SpriteRenderer>();
            reflectionSpriteRenderer.material = spriteReflectionMaterial;
            reflectionSpriteRenderer.sprite = originalSpriteRenderer.sprite;
            reflectionSpriteRenderer.flipY = true;
            reflectionSpriteRenderer.sortingOrder = originalSpriteRenderer.sortingOrder - 1;
        }
        else if (originalObject.GetComponent<MeshRenderer>() != null)
        {
            MeshFilter originalMeshFilter = originalObject.GetComponent<MeshFilter>();
            MeshRenderer originalMeshRenderer = originalObject.GetComponent<MeshRenderer>();

            MeshFilter reflectionMeshFilter = reflection.AddComponent<MeshFilter>();
            MeshRenderer reflectionMeshRenderer = reflection.AddComponent<MeshRenderer>();

            reflectionMeshFilter.mesh = originalMeshFilter.mesh;
            reflectionMeshRenderer.material = reflectionMaterial;
            //reflectionMeshRenderer.materials = originalMeshRenderer.materials;

            reflection.transform.localScale = new Vector3(
                originalObject.transform.localScale.x,
                -originalObject.transform.localScale.y,
                originalObject.transform.localScale.z
            );
        }

        return reflection;
    }

    void UpdateReflection(GameObject originalObject, GameObject reflection)
    {
        if (originalObject.GetComponent<SpriteRenderer>() != null)
        {
            SpriteRenderer originalSpriteRenderer = originalObject.GetComponent<SpriteRenderer>();
            reflection.transform.position = new Vector3(
                originalObject.transform.position.x,
                -originalObject.transform.position.y /*- originalSpriteRenderer.bounds.size.y*/ + reflectionOffset,
                originalObject.transform.position.z
            );

            SpriteRenderer reflectionSpriteRenderer = reflection.GetComponent<SpriteRenderer>();
            reflectionSpriteRenderer.sprite = originalSpriteRenderer.sprite;
            reflectionSpriteRenderer.flipX = originalSpriteRenderer.flipX;
        }
        else if (originalObject.GetComponent<MeshRenderer>() != null)
        {
            reflection.transform.position = new Vector3(
                originalObject.transform.position.x,
                -originalObject.transform.position.y /*- originalObject.GetComponent<MeshRenderer>().bounds.size.y*/ + reflectionOffset,
                originalObject.transform.position.z
            );

            reflection.transform.rotation = originalObject.transform.rotation;

            reflection.transform.localScale = new Vector3(
                originalObject.transform.localScale.x,
                -originalObject.transform.localScale.y,
                originalObject.transform.localScale.z
            );
        }
    }
}