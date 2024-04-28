using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemyPlaceholder : EnemyBase
{
    // PlaceHolder for a Big Enemy

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
        if (isStunned || !isAnimationDone || isOnCooldown)
        {
            return;
        }

        if (isStatic)
        {
            PlayAnimation(animationIDs[1], false);

            if (isMoving)
            {
                isMoving = false;
            }

            return;
        }

        if (!isAttacking && Vector3.Distance(target.position, transform.position) > avoidRange)
        {
            PlayAnimation(animationIDs[1], false);
        }
        else if (!isAttacking && Vector3.Distance(target.position, transform.position) < avoidRange)
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

            rb.MovePosition(transform.position + -targetDir * speed * Time.deltaTime);
            isMoving = true;
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
            PlaySound(hitSounds);
            PlayAnimation(animationIDs[6], true/*, false, true*/);
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
    }

    public override void AvoidBehaviour()
    {
        // Check if the player is in range, if its  in the close attack range, attack.
        // if its in the far attack range, decide if attack or move to the player. if its out of range, move to the player

        if (isStatic)
        {
            Invoke("SummonProjectile", projectileSpawnTime);
            isAttacking = true;
            PlayAnimation(animationIDs[4], true, true);
        }
        else
        {
            if (Vector3.Distance(target.position, transform.position) >= avoidRange)
            {
                // Attacks the player

                Invoke("MoveOnNormalAttack", projectileSpawnTime);
                Invoke("SummonProjectile", projectileSpawnTime);
                isAttacking = true;
                PlayAnimation(animationIDs[4], true, true);
            }
            else
            {
                // Moves Away
                if (isAttacking == true)
                {
                    isAttacking = false;
                }
            }
        }
    }

    public override void MoveOnNormalAttack()
    {
        PlaySound(normalAttackSounds);
        rb.velocity = -targetDir * moveOnNormalAttackForce;
    }

    public override void OnCooldown()
    {
        if (!isOnCooldown)
            return;

        if (!avoidingTarget || isStatic)
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
