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
            PlayAnimation(animationIDs[0], false);
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

        for (int i = 1; i < hitColliders.Length; i++)
        {
            if ((hitColliders[i].CompareTag("Player") || hitColliders[i].CompareTag("Enemy")))
            {

                if (!posibleTargets.ContainsKey(hitColliders[i].gameObject))
                {
                    if (hitColliders[i].CompareTag("Player") && isReadyToAttack)
                    {
                        SelectAttackObjective(hitColliders);
                    }

                    if (hitColliders[i].CompareTag("Enemy") && isReadyToAttack)
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
        isReadyToAttack = false;
        timeToAttack = 0;
        List<Collider> Punchables = new List<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Player") || colliders[i].CompareTag("Enemy"))
            {
                Punchables.Add(colliders[i]);
                
            }
        }
        if (Punchables.Count> 0)
        {
            int randomSelect = Random.Range(0, Punchables.Count);
            Debug.Log("Se selecciono el numero: " + randomSelect + ". En una lista hasta: " + (Punchables.Count -1) + ". Y el objeto seleccionado fue: " + Punchables[randomSelect].name);

            Transform transform = Punchables[randomSelect].transform;
            goToAttack = transform;
            MeleeBehaviour();
        }
        
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
           
            ActivateNormalAttackHitbox(normalAttackHitboxAppearTime.x);
            isAttacking = true;
            PlayAnimation(animationIDs[1], true, true);
            if (goToAttack.gameObject.CompareTag("Enemy") && goToAttack.gameObject != this.gameObject)
            {
                goToAttack.GetComponent<EnemyBase>().TakeDamage(normalAttackdamage, 0f, new Vector3(0, 0, 0));
            }
            if (goToAttack.gameObject.CompareTag("Player"))
            {
                goToAttack.GetComponent<PlayerControllerOverhaul>().TakeDamageProxy(normalAttackdamage,0,transform.position);
            }


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

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(transform.position, radius);
    //}
}
