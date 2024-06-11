using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBigEnemy : NewEnemyBase
{
    [Header("BE Combat")]
    [SerializeField] protected float chargeAttackDamage = 5;
    [SerializeField] protected float chargeAttackKnockback = 15;
    [SerializeField] protected float chargeDecitionCooldown = 2.5f;
    [SerializeField] protected bool canParryChargeAttack;

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
        HitboxFaceToTarget();
        Movement();
        Attack();
    }
    #endregion

    #region Logic
    protected override void Movement()
    {
        if (isStunned || isParried || !IsAnimationDone() || isOnCooldown) return;

        if (!isAttacking && Vector3.Distance(target.position, transform.position) < tooClose) PlayAnimation(1, false);
        else if (!isAttacking && Vector3.Distance(target.position, transform.position) > tooClose)
        {
            transform.position += direction * speed * Time.deltaTime;
            isMoving = true;
            PlayAnimation(2, false);
        }
        else isMoving = false;
    }

    protected override void Attack()
    {
        if (Vector3.Distance(target.position, transform.position) <= closeAttackRange)
        {
            isAttacking = true; normalAttack = true; attackHitboxOn = true;
            PlayAnimation(3, true, true);
        }
        else if (Vector3.Distance(target.position, transform.position) <= farAttackRange && !chargeAttackedConsidered)
        {
            int random = Random.Range(0, maxOdds + 1);

            if (!decidedChargeAttack)
            {
                if (random < oddsToChargeAttack)
                {
                    PlayAnimation(4, true, true);
                    isAttacking = true; normalAttack = false; attackHitboxOn = true;

                    decidedChargeAttack = true;
                    Invoke("ResetDecitionStatus", chargeDecitionCooldown);
                }
            }

            chargeAttackedConsidered = true;
            Invoke("ResetConsideredDecitionStatus", chargeDecitionCooldown);
        }
        else { if (isAttacking == true) isAttacking = false; }
    }

    public void AttackOverlapCollider()
    {
        if (!attackHitboxOn) return;

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
                        GetParried();
                        //player.GetParryRewardProxy(isBigEnemy, false);
                    }
                    else { player.TakeDamageProxy(damage, knockback, -direction); }
                }
                else { player.TakeDamageProxy(damage, knockback, -direction); }

                attackHitboxOn = false;
            }
        }
    }

    protected override void TakeDamage(float damage, float knockbackForce = 0)
    {
        if (isDead) return;
        currentHealth -= damage;
        rb.AddForce((transform.position - target.position).normalized * knockbackForce, ForceMode.Impulse);

        _particleEmission.enabled = true;
        Invoker.InvokeDelayed(ResetParticle, 0.1f);

        if (hasHealthBar)
        {
            healthBar.GetComponentInParent<CanvasGroup>().DOFade(1, onHitAppearSpeed).SetUpdate(UpdateType.Normal, true);
            Invoke("DissapearBar", onHitBarCooldown);
        }

        if (currentHealth <= 0) { Death(); PlaySound(deathSounds); }
        else { PlayAnimation(6, true, true); PlaySound(hitSounds); ResetStatusOnHit(); }
    }

    protected override void GetParried()
    {
        isStunned = true; isParried = true;
        PlaySound(parriedSounds); PlayAnimation(5, true, true);
    }

    protected override void Death()
    {
        isDead = true;
        PlaySound(deathSounds); PlayAnimation(7, false, false, true);
        if (destroyOnDeath) Destroy(gameObject, 0.5f);
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

    protected void ResetStatusOnHit()
    {
        isStunned = true;
        isAttacking = false;
        attackHitboxOn = false;
    }

    #region Invokes

    protected void ResetParticle() { _particleEmission.enabled = false; }
    protected void ResetDecitionStatus() { decidedChargeAttack = false; }
    protected void ResetConsideredDecitionStatus() { chargeAttackedConsidered = false; }

    #endregion

    #endregion
}
