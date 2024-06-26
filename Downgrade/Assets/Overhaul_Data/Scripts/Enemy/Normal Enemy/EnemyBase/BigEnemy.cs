using UnityEngine;

public class BigEnemy : EnemyBase
{
    [Header("Debug Colors")]
    [SerializeField] protected bool drawAttackHitboxesDuringAttack = true;
    [SerializeField] protected Color tooCloseColor = new Color(0, 1, 0, 1);
    [SerializeField] protected Color avoidRangeColor = new Color(1, 1, 0, 1);
    [SerializeField] protected Color closeAttackColor = new Color(1, 0, 0, 1);
    [SerializeField] protected Color farAttackColor = new Color(1, 0.5f, 0, 1);

    [Header("BE Combat")]
    [SerializeField] protected float chargeAttackDamage = 5;
    [SerializeField] protected float chargeAttackKnockback = 15;
    [SerializeField] protected float chargeDecitionCooldown = 2.5f;
    [SerializeField] protected bool canParryChargeAttack = true;
    [SerializeField] protected bool hasChargeAttack = true;
    [SerializeField] protected AudioClip[] chargeAttackSounds;

    [Header("BE C_Attack Odds")]
    [SerializeField] protected int maxOdds = 1000;
    [SerializeField] protected int oddsToChargeAttack = 250;

    [Header("BE Hitbox")]
    [SerializeField] protected Transform hitboxCenter;
    [SerializeField] protected Vector3 attackHitboxSize = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] protected Vector3 chargedAttackHitboxSize = new Vector3(1, 1, 1);
    [SerializeField] protected float hitboxOffset;

    [Header("BE Ranges")]
    [SerializeField] protected float closeAttackRange = 0.8f;
    [SerializeField] protected float farAttackRange = 2;

    protected bool normalAttack;

    #region Unity Methods
    protected override void Update()
    {
        base.Update();
        if (GameManager.Instance.IsGamePaused() || isDead) return;
        HitboxFaceToTarget();
        AttackOverlapCollider();
    }
    #endregion

    #region Logic
    protected override void Attack()
    {
        if (isOnCooldown || attackHitboxOn || isStunned || isParried || isAttacking) return;

        if (DistanceFromTarget() <= closeAttackRange)
        {
            isAttacking = true; normalAttack = true; //attackHitboxOn = true;
            PlayAnimation(3, true, true);
            PlaySound(attackSounds);
        }
        else if (DistanceFromTarget() <= farAttackRange && !chargeAttackedConsidered && hasChargeAttack)
        {
            int random = Random.Range(0, maxOdds + 1);

            if (!decidedChargeAttack)
            {
                if (random < oddsToChargeAttack)
                {
                    isAttacking = true; normalAttack = false; //attackHitboxOn = true;
                    PlayAnimation(7, true, true);
                    PlaySound(chargeAttackSounds);

                    decidedChargeAttack = true;
                    Invoke("ResetDecitionStatus", chargeDecitionCooldown);
                }
            }

            chargeAttackedConsidered = true;
            Invoke("ResetConsideredDecitionStatus", chargeDecitionCooldown);
        }
        else { if (isAttacking == true) isAttacking = false; }
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
                    if (Vector3.Dot(player.GetLastDirection(), direction) <= -0.5f && Vector3.Dot(player.GetLastDirection(), direction) >= -1)
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

    #endregion

    #region Utility
    protected void HitboxFaceToTarget()
    {
        if (attackHitboxOn) return;

        Vector3 direction = (target.position - transform.position).normalized * hitboxOffset;
        Vector3 desiredPosition = transform.position + direction;
        Quaternion rotation = Quaternion.LookRotation(direction);
        hitboxCenter.rotation = rotation;
        hitboxCenter.position = new Vector3(desiredPosition.x, hitboxCenter.position.y, desiredPosition.z);
    }

    #region Invokes

    protected void ResetDecitionStatus() { decidedChargeAttack = false; }
    protected void ResetConsideredDecitionStatus() { chargeAttackedConsidered = false; }

    #endregion

    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        if (!debugTools || debugDrawCenter == null) return;

        DrawRange(tooCloseRange, tooCloseColor);
        DrawRange(avoidanceRange, avoidRangeColor);
        DrawRange(closeAttackRange, closeAttackColor);
        DrawRange(farAttackRange, farAttackColor);

        Gizmos.color = avoidRangeColor;
        Gizmos.DrawLine(transform.position, transform.position + direction * avoidanceRange);

        if (drawAttackHitboxesDuringAttack && !attackHitboxOn && isOnCooldown) return;
        Vector3 colliderSize = normalAttack ? attackHitboxSize : chargedAttackHitboxSize;
        DrawAttackHitbox(hitboxCenter.position, colliderSize, Color.red);
    }
    #endregion
}
