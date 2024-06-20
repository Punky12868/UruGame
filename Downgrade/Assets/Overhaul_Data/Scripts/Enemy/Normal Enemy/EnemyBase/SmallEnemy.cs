using UnityEngine;

public class SmallEnemy : EnemyBase
{
    [Header("Debug Colors")]
    [SerializeField] protected bool drawAttackHitboxesDuringAttack = true;
    [SerializeField] protected Color tooCloseColor = new Color(0, 1, 0, 1);
    [SerializeField] protected Color avoidRangeColor = new Color(1, 1, 0, 1);
    [SerializeField] protected Color closeAttackColor = new Color(1, 0, 0, 1);

    [Header("SE Hitbox")]
    [SerializeField] protected Transform hitboxCenter;
    [SerializeField] protected Vector3 attackHitboxSize = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] protected float hitboxOffset;

    [Header("SE Ranges")]
    [SerializeField] protected float closeAttackRange = 0.8f;

    #region Unity Methods
    protected override void Update()
    {
        base.Update();
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
            isAttacking = true; //attackHitboxOn = true;
            PlayAnimation(3, true, true);
            PlaySound(attackSounds);
        }
        else { if (isAttacking == true) isAttacking = false; }
    }

    protected void AttackOverlapCollider()
    {
        if (!attackHitboxOn || isStunned || isOnCooldown) return;

        Vector3 colliderSize = attackHitboxSize;
        float damage = attackDamage;
        float knockback = attackKnockback;

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
                        GetParried();
                        player.GetParryRewardProxy(enemyType);
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
        if (isAttacking) return;

        Vector3 direction = (target.position - transform.position).normalized * hitboxOffset;
        Vector3 desiredPosition = transform.position + direction;
        Quaternion rotation = Quaternion.LookRotation(direction);
        hitboxCenter.rotation = rotation;
        hitboxCenter.position = new Vector3(desiredPosition.x, hitboxCenter.position.y, desiredPosition.z);
    }
    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        if (!debugTools || debugDrawCenter == null) return;
        
        DrawRange(tooCloseRange, tooCloseColor);
        DrawRange(avoidanceRange, avoidRangeColor);
        DrawRange(closeAttackRange, closeAttackColor);

        Gizmos.color = avoidRangeColor;
        Gizmos.DrawLine(transform.position, transform.position + direction * avoidanceRange);

        if (drawAttackHitboxesDuringAttack && !attackHitboxOn || isOnCooldown) return;
        DrawAttackHitbox(hitboxCenter.position, attackHitboxSize, Color.red);
    }
    #endregion
}
