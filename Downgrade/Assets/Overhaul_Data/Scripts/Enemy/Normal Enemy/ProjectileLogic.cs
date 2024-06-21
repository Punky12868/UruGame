using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLogic : MonoBehaviour
{
    [SerializeField] private bool invertSprite = false;
    [SerializeField] private bool faceDir = false;
    private float travelSpeed;
    private float damage;
    private float knockbackForce;
    private bool canBeParried;
    private bool isParried;
    private GameObject originalEnemy;
    private Vector3 direction;
    private AudioClip[] parrySounds;
    private Transform child;

    public void SetVariables(float speed, float dmg, float lftime, float knckbck, bool parry, Vector3 dir, AudioClip[] prrySnd, GameObject orgnlEnemy)
    {
        travelSpeed = speed;
        damage = dmg;
        direction = dir;
        canBeParried = parry;
        knockbackForce = knckbck;
        parrySounds = prrySnd;
        originalEnemy = orgnlEnemy;

        child = GetComponentInChildren<SpriteRenderer>().transform;

        Destroy(gameObject, lftime);
    }

    private void Update()
    {
        if (GameManager.Instance.IsGamePaused())
            return;

        if (GetComponent<SpriteRenderer>())
        {
            if (direction.x > 0 && !GetComponent<SpriteRenderer>().flipX)
            {
                GetComponent<SpriteRenderer>().flipX = invertSprite ? false : true;
            }
        }
        else
        {
            if (direction.x > 0 && !GetComponentInChildren<SpriteRenderer>().flipX)
            {
                GetComponentInChildren<SpriteRenderer>().flipX = invertSprite ? true : false;
            }
        }

        if (faceDir)
        {
            transform.Translate(direction.normalized * travelSpeed * Time.deltaTime);
            child.rotation = Quaternion.FromToRotation(Vector3.right, direction);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isParried)
        {
            if (canBeParried && other.GetComponent<PlayerControllerOverhaul>().GetPlayerState() == "Parry")
            {
                direction *= -1;
                isParried = true;
                originalEnemy.GetComponent<EnemyBase>().PlaySoundProxy(parrySounds);
                other.GetComponent<PlayerControllerOverhaul>().GetParryRewardProxy(EnemyType.None);
                Vector2 vectorDamage = other.GetComponent<PlayerControllerOverhaul>().GetDamage();
                damage = Random.Range(vectorDamage.x, vectorDamage.y);
            }
            else
            {
                if (!other.GetComponent<PlayerControllerOverhaul>().GetImmunity())
                {
                    other.GetComponent<PlayerControllerOverhaul>().TakeDamageProxy(damage, knockbackForce, -direction);
                    Destroy(gameObject);
                }
            }
            
            
        }

        if (other.CompareTag("Enemy") && isParried)
        {
            other.GetComponent<EnemyBase>().TakeDamageProxy(damage);
            Destroy(gameObject);
        }

        if (other.CompareTag("Wall") || other.CompareTag("Limits"))
        {
            Destroy(gameObject);
        }
    }
}
