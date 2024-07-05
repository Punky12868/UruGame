using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

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
    [SerializeField] protected int oddsToSpecialAttack = 500;

    [Header("Witch Hitbox")]
    [SerializeField] protected Transform hitboxCenter;
    [SerializeField] protected Vector3 attackHitboxSize = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] protected Vector3 chargedAttackHitboxSize = new Vector3(1, 1, 1);
    [SerializeField] protected float hitboxOffset;

    [Header("Witch Ranges")]
    [SerializeField] protected float closeAttackRange = 0.8f;
    [SerializeField] protected float farAttackRange = 2;

    [Header("Witch Specials")]
    [SerializeField] protected int mortarsToSpawn = 3;
    [SerializeField] protected int mortarsSpawnRate = 1;
    [SerializeField] protected float bouncingSpeedMultiplier = 3;
    [SerializeField] protected float offset = 0.05f;
    [SerializeField] protected float yOffset = -1; 
    [SerializeField] protected Transform centerAirPoint;

    protected bool normalAttack;
    protected bool canAttack;
    protected bool witchStarting;
    protected bool witchStarted;
    protected int storedOdds;
    protected float storedMeleeRange;
    protected bool isOnSpecial;
    protected bool stoppingSpecial;
    protected bool wasOnSpecialOnce;
    protected bool isBouncing;
    protected bool bounced;
    protected float bounceTimer;
    protected bool onMortar;
    protected bool onGroundChange;
    protected int mortarsSpawned;
    protected float mortarTimer;

    protected Vector3 bounceDir;

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        if (FindObjectOfType<BossUI>()) FindObjectOfType<BossUI>().SetUI(this);
        NotifyBossesObservers(AllBossActions.Spawned);
        FindAnyObjectByType<PartirEscenarioManager>().OnJuntarAddListener(GoBackWitch);
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

        if (!witchStarted) return;

        if (!attackHitboxOn) lastDirection = SetTargetDir();
        if (isSpawning || IsAnimationDone()) isSpawning = false;
        if (SetAvoidanceDir() != Vector3.zero) { direction = Vector3.Slerp(direction, SetAvoidanceDir().normalized, Time.deltaTime * avoidanceSpeed); }
        else direction = SetTargetDir();
        OnCooldownTimer();
        FlipFacingLastDir();
        UpdateHealthUI();
        Movement();
        Mortar();
        Attack();

        if (bounced)
        {
            bounceTimer += Time.deltaTime;
            if (bounceTimer >= 0.5f) { bounced = false; bounceTimer = 0; }
        }

        if (isStunned)
        {
            stunnTime += Time.deltaTime;
            if (stunnTime >= 2f) { isStunned = false; stunnTime = 0; }
        }

        

        else stunnTime = 0;
        HitboxFaceToTarget();
    }
    #endregion

    #region Logic
    protected override void Movement()
    {
        if (isStunned || isParried /*|| !IsAnimationDone()*/ || onGroundChange) return;
        if (onMortar)
        {
            if (mortarsSpawned >= mortarsToSpawn)
            {
                onMortar = false;
                mortarsSpawned = 0;
            }
            return;
        }

        if (isOnSpecial && !stoppingSpecial && transform.position.y != centerAirPoint.position.y 
            && transform.position.x > centerAirPoint.position.x - offset && transform.position.x < centerAirPoint.position.x + offset
            && transform.position.z > centerAirPoint.position.z - offset && transform.position.z < centerAirPoint.position.z + offset)
        {
            transform.DOMoveY(centerAirPoint.position.y, 3).onComplete += () => { /*transform.DOMove(centerAirPoint.position, 1);*/ canAttack = true; };
            //PlayAnimation(2, false);
        }
        if (isOnSpecial)
        {
            if (!isAttacking || isOnCooldown)
            {
                PlayAnimation(0, false);
            }
            return;
        }
        else if (witchStarted)
        {
            if (!isBouncing)
            {
                bounceDir = direction;
                isBouncing = true;
            }
            else
            {
                if (!wasOnSpecialOnce) return;
                Vector3 fixedBounceDir = new Vector3(bounceDir.x, 0, bounceDir.z);
                transform.position = Vector3.MoveTowards(transform.position, transform.position + fixedBounceDir.normalized, speed * bouncingSpeedMultiplier* Time.deltaTime);
                //BouncingWallCollision();
            }
        }
    }
    /*private void BouncingWallCollision()
    {
        if (!isBouncing || bounced) return;
        Collider[] hitColliders = Physics.OverlapBox(transform.position, new Vector3(2, 2, 2));
        
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Limits"))
            {
                Debug.Log("Bouncing from " + direction);
                bounceDir = Reflect(rb.velocity, hitCollider.contactOffset);
                bounced = true;
                Debug.Log("Bouncing to " + bounceDir);
            }
        }
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Limits"))
        {
            bounceDir = Reflect(bounceDir, collision.GetContact(0).normal);
            bounced = true;
        }
    }

    Vector3 Reflect(Vector3 direction, Vector3 normal)
    {
        // Calcular la dirección reflejada
        return direction - 2 * Vector3.Dot(direction, normal) * normal;
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

    protected void GoBackWitch()
    {
        stoppingSpecial = true;
        transform.DOMove(Vector3.zero, 3).onComplete += () => { isOnSpecial = false; stoppingSpecial = false; };
    }

    protected void MoveOnStarting()
    {
        if (witchStarted) return;
        Vector3 pos = new Vector3(centerAirPoint.position.x, 0, centerAirPoint.position.z);
        if (witchStarting && !witchStarted)
        {
            transform.position = Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime);
            PlayAnimation(0);
        }

        if (transform.position == pos)
        {
            witchStarted = true;
            storedMeleeRange = closeAttackRange;
            storedOdds = oddsToSpecialAttack;
            closeAttackRange = 0;
            oddsToSpecialAttack = maxOdds;
            target = FindObjectOfType<PlayerControllerOverhaul>().transform;
            Attack();
            Debug.Log("Witch Started");
        }
    }

    protected override void Attack()
    {
        if (attackHitboxOn || isStunned || isParried || isAttacking) return;

        if (!isOnSpecial && witchStarted && canAttack && !onMortar)
        {
            if (isOnCooldown)
            {
                PlayAnimation(0);
                if (isAttacking == true) isAttacking = false;
                return;
            }
            isAttacking = true; normalAttack = true; //attackHitboxOn = true;
            PlayAnimation(1, true, true);
            PlaySound(attackSounds);
        }

        if (!chargeAttackedConsidered && hasChargeAttack && !isOnSpecial)
        {
            int random = Random.Range(0, maxOdds + 1);
            int randomAttackSelection = !wasOnSpecialOnce ? 0 : Random.Range(0, maxOdds + 1);
            Debug.Log("Random Attack Selection: " + randomAttackSelection);

            if (decidedChargeAttack) return;

            if (random < oddsToSpecialAttack)
            {
                if (randomAttackSelection <= 150)
                {
                    if (wasOnSpecialOnce)
                    {
                        DoSpecial(); return;
                    }
                    isAttacking = true; normalAttack = false; //attackHitboxOn = true;
                    PlayAnimation(2, true, true);
                    PlaySound(chargeAttackSounds);

                    decidedChargeAttack = true;
                    Invoke("ResetDecitionStatus", chargeDecitionCooldown);
                    if (oddsToSpecialAttack == maxOdds) oddsToSpecialAttack = 450;
                    if (closeAttackRange == 0) closeAttackRange = storedMeleeRange;
                    isOnSpecial = true;
                    if (!wasOnSpecialOnce) wasOnSpecialOnce = true;
                }
                else if (randomAttackSelection > 150 /*&& randomAttackSelection < 666*/)
                {
                    //if (decidedChargeAttack) return;
                    onMortar = true;
                }
                /*else if (randomAttackSelection > 666)
                {
                    if (decidedChargeAttack) return;

                    onGroundChange = true;
                    PlayAnimation(8);
                }*/
            }

            chargeAttackedConsidered = true;
            Invoke("ResetConsideredDecitionStatus", chargeDecitionCooldown);
        }
        else if (isOnSpecial && transform.position == centerAirPoint.position && canAttack)
        {
            if (isOnCooldown)
            {
                if (isAttacking == true) isAttacking = false;
                PlayAnimation(0);
                return;
            }
            isAttacking = true; normalAttack = true; //attackHitboxOn = true;
            PlayAnimation(3, true, true);
            PlaySound(attackSounds);
        }
        else { if (isAttacking == true) isAttacking = false; }
    }

    private void Mortar()
    {
        if (!onMortar) return;
        if (mortarTimer >= mortarsSpawnRate)
        {
            mortarTimer = 0;
            SummonMortarProjectile();
            mortarsSpawned++;
            PlayAnimation(4, true, true);
        }
        else mortarTimer += Time.deltaTime;
        isBouncing = false;
    }

    private void DoSpecial()
    {
        if (isOnSpecial) return;
        isBouncing = false;
        isOnSpecial = true;
        stoppingSpecial = false;
        canAttack = false;
        Vector3 vector3 = new Vector3(centerAirPoint.position.x, 0, centerAirPoint.position.z);
        transform.DOMove(vector3, 1).onComplete += () => {  Special(); };
    }

    private void Special()
    {
        canAttack = true;
        isAttacking = true; normalAttack = false; //attackHitboxOn = true;
        PlayAnimation(2, true, true);
        PlaySound(chargeAttackSounds);

        decidedChargeAttack = true;
        Invoke("ResetDecitionStatus", chargeDecitionCooldown);
        if (oddsToSpecialAttack == maxOdds) oddsToSpecialAttack = storedOdds + 250;
        if (closeAttackRange == 0) closeAttackRange = storedMeleeRange;
    }

    public void SummonProjectile()
    {
        if (isOnCooldown || onMortar) return;
        isOnCooldown = true;
        GameObject prjctl = Instantiate(projectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        prjctl.GetComponent<ProjectileLogic>().SetVariables
            (projectileSpeed, attackDamage, projectileLifeTime, attackKnockback,
            projectileCanBeParried, SetTargetDirWithYPos(), parriedSounds, gameObject);
    }

    public void SummonMortarProjectile()
    {
        //if (isOnCooldown) return;
        //isOnCooldown = true;
        GameObject prjctl = Instantiate(projectile, projectileSpawnPoint.position, Quaternion.identity);
        prjctl.GetComponent<ProjectileLogic>().SetVariables
            (projectileSpeed, attackDamage, projectileLifeTime, attackKnockback,
            false, Vector3.down, parriedSounds, gameObject, true);

        Debug.Log("Mortar Spawned");
    }

    protected override void TakeDamage(float damage, float knockbackForce = 0, Vector3 direction = new Vector3())
    {
        if (isDead) return;
        currentHealth -= damage;
        /*if (direction != Vector3.zero) rb.AddForce((transform.position + direction).normalized * knockbackForce, ForceMode.Impulse);
        else rb.AddForce((transform.position - target.position).normalized * knockbackForce, ForceMode.Impulse);*/
        sr.material.SetFloat("_HitFloat", 1);
        Invoke("HitMaterialReset", 0.2f);
        _particleEmission.enabled = true;
        Invoker.InvokeDelayed(ResetParticle, 0.1f);

        if (hasHealthBar)
        {
            healthBar.GetComponentInParent<CanvasGroup>().DOFade(1, onHitAppearSpeed).SetUpdate(UpdateType.Normal, true);
            //Invoker.CancelInvoke(DissapearBar);
            Invoke("DissapearBar", onHitBarCooldown);
        }

        if (currentHealth <= 0) { Death(); }
        else
        {
            PlaySound(hitSounds); /*ResetStatusOnHit();*/ DoCameraShake();
            if (hasHitAnimation) { PlayAnimation(4, true, true); }
        }
    }

    protected override void Death()
    {
        isDead = true;
        PlaySound(deathSounds); PlayAnimation(7, false, true);

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

    public void SetOnGroundChange(bool value) { onGroundChange = value; }

    public void IncrementMortarsSpawned() { mortarsSpawned++; }

    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        if (!debugTools || debugDrawCenter == null) return;
        DrawAttackHitbox(transform.position, new Vector3(2, 2, 2), Color.blue);
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
