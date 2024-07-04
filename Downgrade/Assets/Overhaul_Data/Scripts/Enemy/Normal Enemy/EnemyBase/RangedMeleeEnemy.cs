using UnityEngine;

public class RangedMeleeEnemy : EnemyBase
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
    [SerializeField] protected float chargeAttackDamage = 5;
    [SerializeField] protected float chargeAttackKnockback = 15;
    [SerializeField] protected float chargeDecitionCooldown = 2.5f;
    [SerializeField] protected bool canParryChargeAttack = true;
    [SerializeField] protected bool hasChargeAttack = true;
    [SerializeField] protected bool projectileCanBeParried = true;
    [SerializeField] protected float projectileLifeTime = 5;
    [SerializeField] protected float projectileSpeed = 5;
    [SerializeField] protected AudioClip[] chargeAttackSounds;

    [Header("BE Hitbox")]
    [SerializeField] protected Transform hitboxCenter;
    [SerializeField] protected Vector3 attackHitboxSize = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] protected Vector3 chargedAttackHitboxSize = new Vector3(1, 1, 1);
    [SerializeField] protected float hitboxOffset;

    [Header("BE C_Attack Odds")]
    [SerializeField] protected int maxOdds = 1000;
    [SerializeField] protected int oddsToChargeAttack = 250;

    [Header("RE Ranges")]
    [SerializeField] protected float closeAttackRange = 1f;
    [SerializeField] protected float tooFarRange = 4;

    protected bool canChooseDirection = false;
    protected bool chooseDirection = false;
    protected bool meleeAttack = false;
    protected bool rangedAttack = false;

    protected bool normalAttack;

    protected override void Update()
    {
        base.Update();
        if (GameManager.Instance.IsGamePaused() || isDead) return;
        HitboxFaceToTarget();
        AttackOverlapCollider();
    }

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
            if (DistanceFromTarget() <= closeAttackRange)
            {
                int random = Random.Range(0, maxOdds + 1);

                if (!decidedChargeAttack)
                {
                    if (random < oddsToChargeAttack && !rangedAttack)
                    {
                        isAttacking = true; normalAttack = false; //attackHitboxOn = true;
                        PlayAnimation(7, true, true);
                        PlaySound(chargeAttackSounds);
                        meleeAttack = true;
                        decidedChargeAttack = true;
                        Invoke("ResetDecitionStatus", chargeDecitionCooldown);
                    }
                }

                chargeAttackedConsidered = true;
                Invoke("ResetConsideredDecitionStatus", chargeDecitionCooldown);
            }
            if (DistanceFromTarget() >= tooCloseRange && DistanceFromTarget() <= tooFarRange && !meleeAttack)
            {
                rangedAttack = true;
                isAttacking = true;
                canChooseDirection = true;
                PlayAnimation(3, true, true);
            }
            else { if (isAttacking == true) isAttacking = false; }
        }
    }

    protected void AttackOverlapCollider()
    {
        if (!attackHitboxOn /*|| isStunned*/ || isOnCooldown) return;

        Vector3 colliderSize = normalAttack ? attackHitboxSize : chargedAttackHitboxSize;
        float damage = normalAttack ? attackDamage : chargeAttackDamage;
        float knockback = normalAttack ? attackKnockback : chargeAttackKnockback;

        Collider[] hitColliders = Physics.OverlapBox(hitboxCenter.position, colliderSize, Quaternion.LookRotation(lastDirection));
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                PlayerControllerOverhaul player = hitCollider.GetComponent<PlayerControllerOverhaul>();

                if (player.GetPlayerState() == "Parry" && canBeParried)
                {
                    if (player.GetParryAvailable(transform))
                    {
                        knockback *= 2;
                        GetParried();
                        player.GetParryRewardProxy(enemyType);
                        player.TakeDamageProxy(0, knockback, -lastDirection);
                        attackHitboxOn = false; return;
                    }
                    else player.FailParry();
                }
                attackHitboxOn = false;
                player.TakeDamageProxy(damage, knockback, -direction);
            }
        }
    }

    public void SummonProjectile()
    {
        GameObject prjctl = Instantiate(projectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        prjctl.GetComponent<ProjectileLogic>().SetVariables
            (projectileSpeed, attackDamage, projectileLifeTime, attackKnockback, 
            projectileCanBeParried, SetTargetDir().normalized, parriedSounds, gameObject);
    }

    protected void HitboxFaceToTarget()
    {
        if (attackHitboxOn) return;

        Vector3 direction = (target.position - transform.position).normalized * hitboxOffset;
        Vector3 desiredPosition = transform.position + direction;
        Quaternion rotation = Quaternion.LookRotation(direction);
        hitboxCenter.rotation = rotation;
        hitboxCenter.position = new Vector3(desiredPosition.x, hitboxCenter.position.y, desiredPosition.z);
    }

    public void SetIsStatic(bool value) { isStatic = value; }
    public void SetMeleeAttack(bool value) { meleeAttack = value; }
    public void SetRangedAttack(bool value) { rangedAttack = value; }

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
