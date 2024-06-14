using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallEnemyPlaceholder : OldEnemyBase
{
    // PlaceHolder for a Small Enemy
    [SerializeField] private Dictionary<GameObject, Collider> posibleTargets = new Dictionary<GameObject, Collider>();

    public override void Awake()
    {
        base.Awake();

    }
    public void FixedUpdate()
    {
        if (GameManager.Instance.IsGamePaused())
            return;

        OnCooldown();
        Movement();
        FlipPivot();


        if (posibleTargets.Count < 1)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;

        }
        else
        {

        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, closeAttackRange);

        for (int i = 1; i < hitColliders.Length; i++)
        {
            if ((hitColliders[i].CompareTag("Player") || hitColliders[i].CompareTag("Enemy") && hitColliders[i] != GetComponent<Collider>()))
            {
                if (!posibleTargets.ContainsKey(hitColliders[i].gameObject))
                {
                    posibleTargets[hitColliders[i].gameObject] = hitColliders[i];
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

        SelectAttackObjective(hitColliders);
    }
    private void SelectAttackObjective(Collider[] colliders)
    {
        if (isStunned || isParried || !isAnimationDone || isOnCooldown || decidedChargeAttack)
        {
            return;
        }

        if (isMelee)
        {
            List<Collider> Punchables = new List<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (isStatic)
                {
                    if (colliders[i].CompareTag("Player") || colliders[i].CompareTag("Enemy"))
                    {
                        Punchables.Add(colliders[i]);

                    }
                }
                else
                {
                    if (colliders[i].CompareTag("Player"))
                    {
                        Punchables.Add(colliders[i]);

                    }
                }
            }
            if (Punchables.Count > 0)
            {
                int randomSelect = Random.Range(0, Punchables.Count);
                Debug.Log("Se selecciono el numero: " + randomSelect + ". En una lista hasta: " + (Punchables.Count - 1) + ". Y el objeto seleccionado fue: " + Punchables[randomSelect].name);

                Transform transform = Punchables[randomSelect].transform;
                target = transform;
                MeleeBehaviour();
            }
        }
        

    }

    public override void Movement()
    {
        if (isStunned || !isAnimationDone || isOnCooldown)
        {
            return;
        }

        if (!isAttacking && Vector3.Distance(target.position, transform.position) < tooClose)
        {
            PlayAnimation(animationIDs[1], false);
        }
        else if (!isAttacking && Vector3.Distance(target.position, transform.position) > tooClose)
        {
            if (canRun)
            {
                if (Vector3.Distance(target.position, transform.position) > runRange && !reverseRunLogic)
                {
                    if (speed != runSpeed)
                        speed = runSpeed;

                    if (!isRunning)
                        isRunning = true;

                    PlayAnimation(animationIDs[3], false);
                }
                else if (Vector3.Distance(target.position, transform.position) < runRange && reverseRunLogic)
                {
                    if (speed != runSpeed)
                        speed = runSpeed;

                    if (!isRunning)
                        isRunning = true;

                    PlayAnimation(animationIDs[3], false);
                }
                else
                {
                    if (speed != walkingSpeed)
                        speed = walkingSpeed;

                    if (isRunning)
                        isRunning = false;

                    PlayAnimation(animationIDs[2], false);
                }
            }
            else
            {
                if (speed != walkingSpeed)
                    speed = walkingSpeed;

                PlayAnimation(animationIDs[2], false);
            }

            rb.MovePosition(transform.position + targetDir * speed * Time.deltaTime);
            isMoving = true;
        }
        else
        {
            isMoving = false;
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
            PlaySound(hitSounds);
            PlayAnimation(animationIDs[5], true/*, false, true*/);
            if (knockbackForce != 0) rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
        }
    }

    public override void ParriedEnemy(float time)
    {
        if (canBeParryStunned)
        {
            isStunned = true;
        }
        PlaySound(parrySounds);
        isParried = true;
        PlayAnimation(animationIDs[5], false, false, true);
        PlayAnimation(animationIDs[1], false, false, false, true);
        Debug.Log("Parried");
        Invoke("ResetParried", time);
    }

    public override void Death()
    {
        PlaySound(deathSounds);
        PlayAnimation(animationIDs[7], false, false, true);
        RemoveComponentsOnDeath();
        Destroy(gameObject, 0.5f);
    }

    public override void MeleeBehaviour()
    {
        // Check if the player is in range, if its  in the close attack range, attack.
        // if its in the far attack range, decide if attack or move to the player. if its out of range, move to the player

        if (Vector3.Distance(target.position, transform.position) <= closeAttackRange)
        {
            // Attack the player
            Invoke("MoveOnNormalAttack", normalMoveAttackActivationTime);
            ActivateNormalAttackHitbox(normalAttackHitboxAppearTime.x);
            isAttacking = true;
            PlayAnimation(animationIDs[4], true, true);
            if (target.CompareTag("Enemy") && target.gameObject != this.gameObject)
            {
                target.GetComponent<OldEnemyBase>().TakeDamage(normalAttackdamage, 0f, new Vector3(0, 0, 0));
            }
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

    public override void MoveOnNormalAttack()
    {
        PlaySound(normalAttackSounds);
        rb.velocity = lastTargetDir * moveOnNormalAttackForce;
    }

    public override void OnCooldown()
    {
        if (!isOnCooldown)
            return;

        if (!avoidingTarget)
        {
            PlayAnimation(animationIDs[1], true);
        }
        else
        {
            rb.MovePosition(transform.position + -targetDir * speed * Time.deltaTime);
            isMoving = true;

            PlayAnimation(animationIDs[2], false);
        }
    }
}
