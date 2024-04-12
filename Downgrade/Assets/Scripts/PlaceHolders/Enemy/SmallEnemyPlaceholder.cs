using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallEnemyPlaceholder : EnemyBase
{
    // PlaceHolder for a Big Enemy

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

            Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
            Vector3 enemyPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Vector3 dir = (targetPos - enemyPos).normalized;
            rb.MovePosition(transform.position + dir * speed * Time.deltaTime);
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    public override void Death()
    {
        PlayAnimation(animationIDs[7], false);
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

    public override void MoveOnChargeAttack()
    {
        rb.velocity = lastTargetDir * moveOnChargeAttackForce;
    }

    public override void StunEnemy(float time)
    {
        isStunned = true;
        PlayAnimation(animationIDs[1], false);
        Invoke("ResetStun", time);
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
            Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
            Vector3 enemyPos = new Vector3(transform.position.x, transform.position.y, transform.position.y);
            Vector3 dir = -(targetPos - enemyPos).normalized;
            rb.MovePosition(transform.position + dir * speed * Time.deltaTime);
            isMoving = true;

            PlayAnimation(animationIDs[2], false);
        }
    }
}
