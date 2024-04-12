using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigEnemyPlaceholder : EnemyBase
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
            Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
            Vector3 enemyPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Vector3 dir = (targetPos - enemyPos).normalized;
            rb.MovePosition(transform.position + dir * speed * Time.deltaTime);
            isMoving = true;

            PlayAnimation(animationIDs[2], false);
        }
        else
        {
            isMoving = false;
        }
    }

    public override void Death()
    {
        PlayAnimation(animationIDs[6], false);
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
            Vector3 enemyPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Vector3 dir = -(targetPos - enemyPos).normalized;
            rb.MovePosition(transform.position + dir * speed * Time.deltaTime);

            PlayAnimation(animationIDs[2], false);
        }
    }
}
