using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLogic : MonoBehaviour
{
    private float travelSpeed;
    private float damage;
    private float knockbackForce;
    private bool canBeParried;
    private bool isParried;
    private GameObject originalEnemy;
    private Vector3 direction;
    private AudioClip[] parrySounds;

    public void SetVariables(float speed, float dmg, float lftime, float knckbck, bool parry, Vector3 dir, AudioClip[] prrySnd, GameObject orgnlEnemy)
    {
        travelSpeed = speed;
        damage = dmg;
        direction = dir;
        canBeParried = parry;
        knockbackForce = knckbck;
        parrySounds = prrySnd;
        originalEnemy = orgnlEnemy;

        Destroy(gameObject, lftime);
    }

    private void Update()
    {
        transform.Translate(direction.normalized * travelSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isParried)
        {
            if (canBeParried && other.GetComponent<PlayerController>().GetPlayerState() == "Parry")
            {
                direction *= -1;
                isParried = true;
                originalEnemy.GetComponent<EnemyBase>().PlaySound(parrySounds);
            }
            else
            {
                other.GetComponent<PlayerController>().TakeDamage(damage, knockbackForce, transform.position);
                Destroy(gameObject);
            }
            
            
        }

        if (other.CompareTag("Enemy") && isParried)
        {
            other.GetComponent<EnemyBase>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
