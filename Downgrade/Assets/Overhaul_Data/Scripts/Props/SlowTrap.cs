using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTrap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void FixedUpdate()
    {
        Physics.OverlapBox(transform.position, transform.localScale, Quaternion.identity);
    }
    // Update is called once per frame
    void Update()
    {
       
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
