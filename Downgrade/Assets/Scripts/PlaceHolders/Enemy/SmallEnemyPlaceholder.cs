using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallEnemyPlaceholder : EnemyBase
{
    // PlaceHolder for a Small Enemy

    public void FixedUpdate()
    {
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

    public override void TakeDamage(float damage)
    {
        if (isDead)
            return;

        if (hasKnockback)
        {
            rb.AddForce((transform.position - target.position).normalized * knockbackForce, ForceMode.Impulse);
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            //RemoveComponentsOnDeath();
            Death();
        }
        else
        {
            //HIT ANIMATION
            PlayAnimation(animationIDs[5], true/*, false, true*/);
        }
    }

    public override void ParriedEnemy(float time)
    {
        if (canBeParryStunned)
        {
            isStunned = true;
        }
        isParried = true;
        PlayAnimation(animationIDs[5], false, false, true);
        PlayAnimation(animationIDs[1], false, false, false, true);
        Debug.Log("Parried");
        Invoke("ResetParried", time);
    }

    public override void Death()
    {
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
            PlayAnimation(animationIDs[4], true, true);
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
