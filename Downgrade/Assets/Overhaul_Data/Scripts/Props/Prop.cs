using Rewired.Data.Mapping;
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
    private bool isDestroyed = false;

    public GameObject highlighted;
    private Vector3 boxSize = new Vector3 (2, 1, 2);
    private float maxDistance = 10.0f;
    private void Awake()
    {  
        animator = GetComponent<Animator>();
        holdedItem = FindObjectOfType<PropsManager>().GetItem(itemType);
    }

    private void Update()
    {
        Vector3 colliderSize = boxSize /2;
        Collider[] hitColliders = Physics.OverlapBox(transform.position, colliderSize, Quaternion.identity);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                highlighted.SetActive(true);
            }
            else
            {
                highlighted.SetActive(false);
            }
        }



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

        Invoke("DestroyThis", spawnItemTime + 0.001f);
    }

    private void SpawnItem()
    {
        if (holdedItem != null)
        {
            Instantiate(holdedItem, transform.position + (new Vector3(0,0.1f,0)), Quaternion.identity);
        }
    }

    private void SpawnTrap()
    {
        if(isRotated90) { Instantiate(trap, transform.position + new Vector3(0, 0.1f, 0), new Quaternion(0.707106829f, 0, 0, 0.707106829f)); }
        else { Instantiate(trap, transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity); } 
    }

    private void DestroyThis()
    {
        Destroy(gameObject);
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
    //    Gizmos.DrawWireCube(Vector3.zero, boxSize);
    //}
}
