using DG.Tweening;
using UnityEngine;

public class BigEnemyPlaceholder : EnemyBase
{
    // PlaceHolder for a Big Enemy

    public override void Awake()
    {
        isBigEnemy = true;
        base.Awake();
    }

    public void FixedUpdate()
    {
        if (GameManager.Instance.IsGamePaused())
            return;

        OnCooldown();
        Movement();
        Attack();
        FlipPivot();
    }

    public override void Movement()
    {
        if (isStunned || isParried || !isAnimationDone || isOnCooldown)
        {
            return;
        }

        if (!isAttacking && Vector3.Distance(target.position, transform.position) < tooClose)
        {
            PlayAnimation(animationIDs[1], false);
        }
        else if (!isAttacking && Vector3.Distance(target.position, transform.position) > tooClose)
        {
            // dont used rb.MovePosition, it will cause the enemy to teleport and dont use rb.velocity, it will cause the enemy to slide, use transform.position instead
            transform.position += targetDir * speed * Time.deltaTime;

            isMoving = true;

            PlayAnimation(animationIDs[2], false);
        }
        else
        {
            isMoving = false;
        }
    }

    public override void TakeDamage(float damage)
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
        PlayAnimation(animationIDs[5], true, false, true);
        Debug.Log("Parried");
        Invoke("ResetParried", time);
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
            Invoke("MoveOnNormalAttack", normalMoveAttackActivationTime);
            ActivateNormalAttackHitbox(normalAttackHitboxAppearTime.x);
            isAttacking = true;
            PlayAnimation(animationIDs[3], true, true);

            /*decidedChargeAttack = true;
            Invoke("ResetDecitionStatus", chargeDecitionCooldown / 2);*/
        }
        else if (Vector3.Distance(target.position, transform.position) <= farAttackRange && !chargeAttackedConsidered && hasChargeAttack)
        {
            // Decide if attack or move to the player

            int random = Random.Range(0, maxOdds + 1);

            if (!decidedChargeAttack)
            {
                if (random < oddsToChargeAttack)
                {
                    PlayAnimation(animationIDs[4], true, true);
                    ActivateChargedAttackHitbox(chargedAttackHitboxAppearTime.x);
                    Invoke("MoveOnChargeAttack", chargeMoveAttackActivationTime);
                    isAttacking = true;

                    decidedChargeAttack = true;
                    Invoke("ResetDecitionStatus", chargeDecitionCooldown);
                }
            }

            chargeAttackedConsidered = true;
            Invoke("ResetConsideredDecitionStatus", chargeDecitionCooldown);
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

    public override void MoveOnChargeAttack()
    {
        PlaySound(chargedAttackSounds);
        rb.velocity = lastTargetDir * moveOnChargeAttackForce;
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

            PlayAnimation(animationIDs[2], false);
        }
    }
}
