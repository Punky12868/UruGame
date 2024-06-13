using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRangedEnemy : EnemyBase
{
    [Header("Debug Colors")]
    [SerializeField] protected Color tooCloseColor = new Color(0, 1, 0, 1);
    [SerializeField] protected Color tooFarColor = new Color(0, 1, 0, 1);
    [SerializeField] protected Color avoidRangeColor = new Color(1, 1, 0, 1);

    [Header("RE Type")]
    [SerializeField] protected bool isStatic = false;

    [Header("RE Projectile")]
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected Transform projectileSpawnPoint;

    [Header("RE Combat")]
    [SerializeField] protected bool projectileCanBeParried = true;
    [SerializeField] protected float projectileLifeTime = 5;
    [SerializeField] protected float projectileSpeed = 5;

    [Header("RE Ranges")]
    [SerializeField] protected float tooFarRange;

    protected override void Movement()
    {
        if (isStunned || !IsAnimationDone()) return;
        if (isOnCooldown || isOnCooldown && isStatic) { PlayAnimation(1, false); if (isAttacking) { isAttacking = false; } return; }

        if (!isAttacking && DistanceFromTarget() < tooClose)
        {
            transform.position += -direction * speed * Time.deltaTime;
            isMoving = true; PlayAnimation(2, false);
        }
        else if (!isAttacking && DistanceFromTarget() > tooFarRange)
        {
            transform.position += direction * speed * Time.deltaTime;
            isMoving = true; PlayAnimation(2, false);
        }
        else isMoving = false;
    }

    protected override void Attack()
    {
        if (isOnCooldown || isStunned || isAttacking) return;

        if (isStatic)
        {
            isAttacking = true;
            PlayAnimation(3, true, true);
        }
        else
        {
            if (DistanceFromTarget() >= tooClose)
            {
                isAttacking = true;
                PlayAnimation(3, true, true);
            }
            else { if (isAttacking == true) isAttacking = false; }
        }
    }

    public void SummonProjectile()
    {
        GameObject prjctl = Instantiate(projectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        prjctl.GetComponent<ProjectileLogic>().SetVariables(projectileSpeed, attackDamage, projectileLifeTime, attackKnockback, projectileCanBeParried, direction, parriedSounds, gameObject);
    }
}
