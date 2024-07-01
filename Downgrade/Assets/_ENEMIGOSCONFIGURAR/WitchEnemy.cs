using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WitchEnemy : EnemyBase
{
    [Header("Starting Conditions")]
    [SerializeField] protected float startingTime = 2;
    [Header("Debug Colors")]
    [SerializeField] protected bool drawAttackHitboxesDuringAttack = true;
    [SerializeField] protected Color tooCloseColor = new Color(0, 1, 0, 1);
    [SerializeField] protected Color avoidRangeColor = new Color(1, 1, 0, 1);
    [SerializeField] protected Color closeAttackColor = new Color(1, 0, 0, 1);
    [SerializeField] protected Color farAttackColor = new Color(1, 0.5f, 0, 1);

    [Header("Witch Combat")]
    [SerializeField] protected float parryKnockback = 100;
    [SerializeField] protected float specialAttackDamage = 5;
    [SerializeField] protected float chargeAttackKnockback = 15;
    [SerializeField] protected float chargeDecitionCooldown = 2.5f;
    [SerializeField] protected bool canParryChargeAttack = true;
    [SerializeField] protected bool hasChargeAttack = true;
    [SerializeField] protected AudioClip[] chargeAttackSounds;

    [Header("RE Projectile")]
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected Transform projectileSpawnPoint;
    [SerializeField] protected bool projectileCanBeParried = true;
    [SerializeField] protected float projectileLifeTime = 5;
    [SerializeField] protected float projectileSpeed = 5;

    [Header("Witch C_Attack Odds")]
    [SerializeField] protected int maxOdds = 1000;
    [SerializeField] protected int oddsToChargeAttack = 250;

    [Header("Witch Hitbox")]
    [SerializeField] protected Transform hitboxCenter;
    [SerializeField] protected Vector3 attackHitboxSize = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] protected Vector3 chargedAttackHitboxSize = new Vector3(1, 1, 1);
    [SerializeField] protected float hitboxOffset;

    [Header("Witch Ranges")]
    [SerializeField] protected float closeAttackRange = 0.8f;
    [SerializeField] protected float farAttackRange = 2;

    [Header("Witch Specials")]
    [SerializeField] protected float offset = 0.05f;
    [SerializeField] protected Transform centerAirPoint;

    protected bool normalAttack;
    protected bool witchStarting;
    protected bool witchStarted;
    protected int storedOdds;
    protected float storedMeleeRange;
    protected bool isOnSpecial;

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        if (FindObjectOfType<BossUI>()) FindObjectOfType<BossUI>().SetUI(this);
        NotifyBossesObservers(AllBossActions.Spawned);
    }

    protected override void Update()
    {
        if (GameManager.Instance.IsGamePaused() || isDead) return;

        if (witchStarting && !witchStarted)
        {
            if (!attackHitboxOn) lastDirection = SetTargetDir();
            MoveOnStarting();
            FlipFacingLastDir();
            return;
        }

        if (!attackHitboxOn) lastDirection = SetTargetDir();
        if (isSpawning || IsAnimationDone()) isSpawning = false;
        if (SetAvoidanceDir() != Vector3.zero) { direction = Vector3.Slerp(direction, SetAvoidanceDir().normalized, Time.deltaTime * avoidanceSpeed); }
        else direction = SetTargetDir();
        OnCooldownTimer();
        FlipFacingLastDir();
        UpdateHealthUI();
        Movement();
        Attack();

        if (isStunned)
        {
            stunnTime += Time.deltaTime;
            if (stunnTime >= 2f) { isStunned = false; stunnTime = 0; }
        }

        else stunnTime = 0;
        HitboxFaceToTarget();
        AttackOverlapCollider();
    }
    #endregion

    #region Logic
    protected override void Movement()
    {
        if (isStunned || isParried || !IsAnimationDone()) return;

        if (isOnSpecial && transform.position.y != centerAirPoint.position.y 
            && transform.position.x > centerAirPoint.position.x - offset && transform.position.x < centerAirPoint.position.x + offset
            && transform.position.z > centerAirPoint.position.z - offset && transform.position.z < centerAirPoint.position.z + offset)
        {
            transform.position = Vector3.MoveTowards(transform.position, centerAirPoint.position, speed * Time.deltaTime);
            //PlayAnimation(2, false);
        }
        if (isOnSpecial)
        {
            if (!isAttacking || isOnCooldown)
            {
                PlayAnimation(1, false);
            }
            return;
        }
        if (isOnCooldown || isAttacking) { PlayAnimation(1, false); if (isMoving) { isMoving = false; } return; }

        if (!isAttacking && DistanceFromTarget() < tooCloseRange)
        {
            transform.position += -direction * speed * Time.deltaTime;
            isMoving = true; PlayAnimation(2, false);
        }
        else if (!isAttacking && DistanceFromTarget() > tooCloseRange)
        {
            transform.position += direction * speed * Time.deltaTime;
            isMoving = true; PlayAnimation(2, false);
        }
        else isMoving = false;
    }

    protected override void SpawningSequence()
    {
        base.SpawningSequence();
        
        Invoke("StartWitch", startingTime);
    }

    protected void StartWitch() 
    { 
        centerAirPoint = FindObjectOfType<PartirEscenarioManager>().transform;
        target = centerAirPoint;
        witchStarting = true; 
        //isOnSpecial = true;
    }

    protected void MoveOnStarting()
    {
        if (witchStarted) return;
        Vector3 pos = new Vector3(centerAirPoint.position.x, 0, centerAirPoint.position.z);
        if (witchStarting && !witchStarted)
        {
            transform.position = Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime);
            PlayAnimation(2);
        }

        if (transform.position == pos)
        {
            witchStarted = true;
            storedMeleeRange = closeAttackRange;
            storedOdds = oddsToChargeAttack;
            closeAttackRange = 0;
            oddsToChargeAttack = maxOdds;
            target = FindObjectOfType<PlayerControllerOverhaul>().transform;
            Attack();
            Debug.Log("Witch Started");
        }
    }

    protected override void Attack()
    {
        if (isOnCooldown || attackHitboxOn || isStunned || isParried || isAttacking) return;

        if (DistanceFromTarget() <= closeAttackRange)
        {
            isAttacking = true; normalAttack = true; //attackHitboxOn = true;
            PlayAnimation(3, true, true);
            PlaySound(attackSounds);
        }
        else if (DistanceFromTarget() <= farAttackRange && !chargeAttackedConsidered && hasChargeAttack && !isOnSpecial)
        {
            int random = Random.Range(0, maxOdds + 1);

            if (!decidedChargeAttack)
            {
                if (random < oddsToChargeAttack)
                {
                    isAttacking = true; normalAttack = false; //attackHitboxOn = true;
                    PlayAnimation(4, true, true);
                    PlaySound(chargeAttackSounds);

                    decidedChargeAttack = true;
                    Invoke("ResetDecitionStatus", chargeDecitionCooldown);
                    if (oddsToChargeAttack == maxOdds) oddsToChargeAttack = storedOdds;
                    if (closeAttackRange == 0) closeAttackRange = storedMeleeRange;
                    isOnSpecial = true;
                }
            }

            chargeAttackedConsidered = true;
            Invoke("ResetConsideredDecitionStatus", chargeDecitionCooldown);
        }
        else if (isOnSpecial && transform.position == centerAirPoint.position)
        {
            isAttacking = true; normalAttack = true; //attackHitboxOn = true;
            PlayAnimation(3, true, true);
            PlaySound(attackSounds);
        }
        else { if (isAttacking == true) isAttacking = false; }
    }

    public void SummonProjectile()
    {
        GameObject prjctl = Instantiate(projectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        prjctl.GetComponent<ProjectileLogic>().SetVariables
            (projectileSpeed, attackDamage, projectileLifeTime, attackKnockback,
            projectileCanBeParried, SetTargetDir().normalized, parriedSounds, gameObject);
    }

    protected void AttackOverlapCollider()
    {
        if (!attackHitboxOn /*|| isStunned*/ || isOnCooldown) return;

        Vector3 colliderSize = normalAttack ? attackHitboxSize : chargedAttackHitboxSize;
        float damage = normalAttack ? attackDamage : specialAttackDamage;
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

    protected override void Death()
    {
        isDead = true;
        PlaySound(deathSounds); PlayAnimation(5, false, true);

        if (FindObjectOfType<WaveSystem>()) FindObjectOfType<WaveSystem>().UpdateDeadEnemies();
        if (hasHealthBar) healthBar.GetComponentInParent<CanvasGroup>().DOFade(0, 0.5f);
        if (hasHealthBar) Destroy(healthBar.GetComponentInParent<CanvasGroup>().gameObject, 0.499f);
        Destroy(GetComponent<Collider>());
        Invoker.CancelInvoke(DissapearBar);
        //Destroy(audioSource);
        if (destroyOnDeath) { Destroy(gameObject, 0.5f); return; }
        this.enabled = false;
    }

    #endregion

    #region Specials

    public void SpawnObjectOnAttack()
    {
        FindObjectOfType<PartirEscenarioManager>().PartirEscenario();
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
