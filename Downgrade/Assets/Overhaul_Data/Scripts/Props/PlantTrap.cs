using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlantTrap : EnemyBase
{
    // Start is called before the first frame update
    float radius = 3;
    [SerializeField] private Dictionary<GameObject, float> posibleTargets = new Dictionary<GameObject, float>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player") || hit.CompareTag("Enemy"))
            {
                float speedToSave = 1;
                if (!posibleTargets.ContainsKey(hit.gameObject))
                {
                    if (hit.CompareTag("Player"))
                    {
                        
                    }

                    if (hit.CompareTag("Enemy"))
                    {
                        speedToSave = hit.GetComponent<EnemyBase>().speed;
                    }
                    posibleTargets[hit.gameObject] = speedToSave;
                }
            }
        }
    }

    public override void TakeDamage(float damage, float knockbackForce = 0, Vector3 dir = new Vector3())
    {
        if (isDead)
            return;

        if (hasHealthBar)
        {
            healthBar.GetComponentInParent<CanvasGroup>().DOFade(1, onHitAppearSpeed).SetUpdate(UpdateType.Normal, true);
            Invoke("DissapearBar", onHitBarCooldown);
        }

        if (hasKnockback)
        {
            rb.AddForce((transform.position - target.position).normalized * knockbackForce, ForceMode.Impulse);
        }

        currentHealth -= damage;

        _particleEmission.enabled = true;
        Invoker.InvokeDelayed(ResetParticle, 0.1f);

        if (currentHealth <= 0)
        {
            //RemoveComponentsOnDeath();
            Death();
        }
        else
        {
            //HIT ANIMATION
            PlayAnimation(animationIDs[6], true, false, true);
            PlaySound(hitSounds);
            if (knockbackForce != 0) rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
        }
    }


    public override void Death()
    {
        PlaySound(deathSounds);
        PlayAnimation(animationIDs[7], false, false, true);
        RemoveComponentsOnDeath();
    }

    public override void MeleeBehaviour()
    {
        // Check if the player is in range, if its  in the close attack range, attack.
        // if its in the far attack range, decide if attack or move to the player. if its out of range, move to the player

        if (Vector3.Distance(target.position, transform.position) <= closeAttackRange)
        {
            // Attack the player
            
            ActivateNormalAttackHitbox(normalAttackHitboxAppearTime.x);
            isAttacking = true;
            PlayAnimation(animationIDs[3], true, true);

            /*decidedChargeAttack = true;
            Invoke("ResetDecitionStatus", chargeDecitionCooldown / 2);*/
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
