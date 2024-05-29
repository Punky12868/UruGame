using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    Animator animator;
    GameObject holdedItem;
    [SerializeField] bool isHealing = true;
    [SerializeField] bool isRotated90 = false;
    [SerializeField] ItemType itemType;
    [SerializeField] string animName;
    [SerializeField] float spawnItemTime;
    [SerializeField] GameObject trap;

    bool isDestroyed = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        holdedItem = FindObjectOfType<PropsManager>().GetItem(itemType);
    }

    public void OnHit()
    {


        if (!isDestroyed && isHealing)
        {
            animator.Play(animName);
            Destroy(GetComponent<Collider>());
            Invoke("SpawnItem", spawnItemTime);
        }
        else if (!isDestroyed && !isHealing)
        {
            animator.Play(animName);
            Destroy(GetComponent<Collider>());
            Invoke("SpawnTrap", spawnItemTime);
        }
    }

    private void SpawnItem()
    {
        if (holdedItem != null)
        {
            Instantiate(holdedItem, transform.position, Quaternion.identity);
        }
    }

    private void SpawnTrap()
    {
        if(isRotated90) { Instantiate(trap, transform.position + new Vector3(0, 0.1f, 0), new Quaternion(0.707106829f, 0, 0, 0.707106829f)); }
        else { Instantiate(trap, transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity); } 
    }
}
