using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlantTrap : EnemyBase
{
    // Start is called before the first frame update
    float radius = 2;
    float timeToAttack = 0f;
    bool isReadyToAttack;
    Transform goToAttack;
    [SerializeField] private Dictionary<GameObject, Transform> posibleTargets = new Dictionary<GameObject, Transform>();

    public override void Awake()
    {
        
        isStatic = true;
        base.Awake();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isAttacking)
        {
            PlayAnimation(animationIDs[1], false);
        }

        FlipPivot();

        if (timeToAttack < 5f && !GameManager.Instance.IsGamePaused())
        {
            timeToAttack += Time.fixedDeltaTime;
        }
        else if (timeToAttack >= 5f)
        {
            isReadyToAttack = true;
        }


        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hitColliders)
        {
            if ((hit.CompareTag("Player") || hit.CompareTag("Enemy")))
            {

                if (!posibleTargets.ContainsKey(hit.gameObject))
                {
                    if (hit.CompareTag("Player") && isReadyToAttack)
                    {
                        SelectAttackObjective(hitColliders);
                    }

                    if (hit.CompareTag("Enemy") && isReadyToAttack)
                    {
                        SelectAttackObjective(hitColliders);
                    }
                }
            }
        }

        List<GameObject> toRemove = new List<GameObject>();
        foreach (var obj in posibleTargets)
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
                toRemove.Add(obj.Key);
            }

        }

        foreach (var obj in toRemove)
        {
            posibleTargets.Remove(obj);
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
            PlayAnimation(animationIDs[3], true, false, true);
            PlaySound(hitSounds);
            if (knockbackForce != 0) rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
        }
    }


    private void SelectAttackObjective(Collider[] colliders)
    {
        int randomSelect = Random.Range(0, colliders.Length-1);
        Transform transform = colliders[randomSelect].transform;
        goToAttack = transform;
        MeleeBehaviour();
    }

    public override void Death()
    {
        PlaySound(deathSounds);
        PlayAnimation(animationIDs[4], false, false, true);
        RemoveComponentsOnDeath();
    }

    public override void MeleeBehaviour()
    {
        // Check if the player is in range, if its  in the close attack range, attack.
        // if its in the far attack range, decide if attack or move to the player. if its out of range, move to the player

        if (Vector3.Distance(goToAttack.position, transform.position) <= closeAttackRange)
        {
            // Attack the player
            isReadyToAttack = false;
            timeToAttack = 0;
            ActivateNormalAttackHitbox(normalAttackHitboxAppearTime.x);
            isAttacking = true;
            PlayAnimation(animationIDs[1], true, true);

            /*decidedChargeAttack = true;
            Invoke("ResetDecitionStatus", chargeDecitionCooldown / 2);*/
        }
        else
        {
            // Move to the player
            if (isAttacking == true)
            {
                isAttacking = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
