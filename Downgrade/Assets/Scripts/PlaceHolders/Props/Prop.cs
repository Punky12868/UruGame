using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    Animator animator;
    GameObject holdedItem;
    [SerializeField] ItemType itemType;
    [SerializeField] string animName;
    [SerializeField] float spawnItemTime;
    [SerializeField] float spawnForce;

    bool isDestroyed = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        holdedItem = FindObjectOfType<PropsManager>().GetItem(itemType);
    }

    public void OnHit()
    {
        if (!isDestroyed)
        {
            animator.Play(animName);
            Destroy(GetComponent<Collider>());
            Invoke("SpawnItem", spawnItemTime);
        }
    }

    private void SpawnItem()
    {
        if (holdedItem != null)
        {
            GameObject spawnedItem = Instantiate(holdedItem, transform.position, Quaternion.identity);
            spawnedItem.GetComponent<Rigidbody>().AddForce(transform.up * spawnForce, ForceMode.Impulse);
        }
    }
}
