using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTrap : MonoBehaviour
{
    private float despawnTime; //Maybe
    [SerializeField] private float radius;
    [SerializeField] private float multiplier = 0.7f;
    [SerializeField] private Dictionary<GameObject, float> originalSpeeds = new Dictionary<GameObject, float>(); 
    void Start()
    {
        
    }


    private void FixedUpdate()
    {
        if (despawnTime < 6 && !GameManager.Instance.IsGamePaused()) 
        {
           despawnTime +=  Time.fixedDeltaTime ;
        }
        else
        {
            Destroy(gameObject);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player") || hit.CompareTag("Enemy"))
            {
                float speedToSave = 1;
                if (!originalSpeeds.ContainsKey(hit.gameObject))
                {
                    if (hit.CompareTag("Player"))
                    {
                        speedToSave = hit.GetComponent<PlayerControllerOverhaul>().GetSpeed();
                        hit.GetComponent<PlayerControllerOverhaul>().SetSpeed(speedToSave * multiplier);
                    }

                    if (hit.CompareTag("Enemy"))
                    {
                        speedToSave = hit.GetComponent<EnemyBase>().GetSpeed();
                        hit.GetComponent<EnemyBase>().SetSpeed(speedToSave * multiplier);
                    }
                    originalSpeeds[hit.gameObject] = speedToSave;
                }
            }
        }

        // Restore speed
        List<GameObject> toRemove = new List<GameObject>();
        foreach (var obj in originalSpeeds)
        {
            bool isStillInside = false;
            foreach (var hit in hitColliders)
            {
                if (hit.gameObject == obj.Key)
                {
                    isStillInside = true;
                    break;
                }
            }
            if (!isStillInside)
            {
                if (obj.Key.CompareTag("Player"))
                {
                    obj.Key.GetComponent<PlayerControllerOverhaul>().SetSpeed(originalSpeeds[obj.Key]);
                }
                else if (obj.Key.CompareTag("Enemy") && obj.Key.GetComponent<EnemyBase>())
                {
                    obj.Key.GetComponent<EnemyBase>().SetSpeed(originalSpeeds[obj.Key]);  
                }
                toRemove.Add(obj.Key);
            }
        }

        // Remove restored objects from the dictionary
        foreach (var obj in toRemove)
        {
            originalSpeeds.Remove(obj);
        }
    }
   
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,radius);
    }
}
