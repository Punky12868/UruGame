using UnityEngine;

public class RangedEnemy : EnemyBase
{
    [Header("Debug Colors")]
    [SerializeField] protected Color tooCloseColor = new Color(0, 1, 0, 1);
    [SerializeField] protected Color tooFarColor = new Color(1, 0, 1, 1);
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
    [SerializeField] protected float tooFarRange = 4;

    protected bool canChooseDirection = false;
    protected bool chooseDirection = false;

    protected override void Movement()
    {
        if (isStunned || !IsAnimationDone()) return;
        if (isOnCooldown && isStatic) { PlayAnimation(1, false); if (isAttacking) { isAttacking = false; } return; }
        if (isOnCooldown && isAttacking) isAttacking = false;
        if (canChooseDirection) { int randInt = Random.Range(0, 2); chooseDirection = randInt == 1; canChooseDirection = false; }

        if (!isAttacking)
        {
            if (DistanceFromTarget() < tooCloseRange)
            {
                transform.position += -direction * speed * Time.deltaTime;
                isMoving = true; PlayAnimation(2, false);
            }
            else if (DistanceFromTarget() > tooFarRange)
            {
                transform.position += direction * speed * Time.deltaTime;
                isMoving = true; PlayAnimation(2, false);
            }
            else if (isOnCooldown)
            {
                if (chooseDirection)
                {
                    transform.position += direction * speed * Time.deltaTime;
                    isMoving = true; PlayAnimation(2, false);
                }
                else
                {
                    transform.position += -direction * speed * Time.deltaTime;
                    isMoving = true; PlayAnimation(2, false);
                }
            }
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
            if (/*DistanceFromTarget() >= tooCloseRange && */DistanceFromTarget() <= tooFarRange)
            {
                isAttacking = true;
                canChooseDirection = true;
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

    #region Debug
    private void OnDrawGizmos()
    {
        if (!debugTools || debugDrawCenter == null) return;

        DrawRange(tooCloseRange, tooCloseColor);
        DrawRange(tooFarRange, tooFarColor);
        DrawRange(avoidanceRange, avoidRangeColor);

        Gizmos.color = avoidRangeColor;
        Gizmos.DrawLine(transform.position, transform.position + direction * avoidanceRange);
    }
    #endregion
}
