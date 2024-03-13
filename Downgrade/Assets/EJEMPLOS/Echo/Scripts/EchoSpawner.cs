using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoSpawner : MonoBehaviour
{
    [SerializeField] GameObject echoPrefab;
    [SerializeField] float yPos = 0.0002875328f;

    public void SpawnEcho()
    {
        Instantiate(echoPrefab, new Vector3(transform.position.x, yPos, transform.position.z), Quaternion.identity);

        //StartCoroutine(Spawn());
    }

    /*IEnumerator Spawn()
    {
        Instantiate(echoPrefab, new Vector3(transform.position.x, yPos, transform.position.z), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        Instantiate(echoPrefab, new Vector3(transform.position.x, yPos, transform.position.z), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        Instantiate(echoPrefab, new Vector3(transform.position.x, yPos, transform.position.z), Quaternion.identity);
    }*/
}
