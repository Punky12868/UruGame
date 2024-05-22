using System.Collections.Generic;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine;

public class BossBase : Subject, IAnimController
{
    #region Variables

    #region Hidden Variables

    protected AnimationHolder animHolder;
    protected List<AnimationClip> animationIDs;
    protected Rigidbody rb;
    protected Transform pivot;
    protected Transform target;
    protected Vector3 lastTargetDir;
    protected Vector3 targetDir;
    protected ParticleSystem.EmissionModule _particleEmission;

    protected bool isDead;
    protected bool isMoving;
    protected bool isAttacking;
    protected bool decidedFarAttack;
    protected bool hasConsideredFarAttack;
    protected bool isHitboxOn;
    protected string attackType;
    protected bool isSummoningObjects;
    protected bool isSpriteFlipped;
    protected bool hasQueuedAnimation;
    protected bool isOnCooldown;
    protected int currentFase;

    #endregion

    [Header("Boss General Stats")]
    [SerializeField] protected string bossName;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected int ammountOfFases;

    [Header("Ranges")]
    [SerializeField] protected float tooCloseRange;
    [SerializeField] protected float closeAttackRange;
    [SerializeField] protected float farAttackRange;
    [SerializeField] protected float wallAvoidanceRange;
    [SerializeField] protected float wallAvoidanceSpeed;

    [Header("Health")]
    [SerializeField] protected float health;
    protected float currentHealth;

    [Header("Attack")]
    [SerializeField] protected float attackdamage;
    [SerializeField] protected float attackKnockback;
    [SerializeField] protected float parryKnockback;
    [SerializeField] protected int maxOdds;
    [SerializeField] protected int farAttackOdds;

    [Header("Specials")]
    [SerializeField] protected GameObject spawnable;
    [SerializeField] protected float specialDamage;
    [SerializeField] protected float specialKnockback;
    [SerializeField] protected float lifeTime;
    [SerializeField] protected float zOffset;

    [Header("Hitbox")]
    [SerializeField] protected Transform hitboxCenter;
    [SerializeField] protected Vector3 hitboxSize;
    [SerializeField] protected float hitboxOffset;
    [SerializeField] protected string playerTag;

    [Header("Cooldowns")]
    [SerializeField] protected float farAttackDecitionCooldown;

    [Header("Audio")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip[] spawnSounds;
    [SerializeField] protected AudioClip[] normalAttackSounds;
    [SerializeField] protected AudioClip[] chargedAttackSounds;
    [SerializeField] protected AudioClip[] hitSounds;
    [SerializeField] protected AudioClip[] deathSounds;
    [SerializeField] protected AudioClip[] parrySounds;

    [Header("Dialogs"), TextArea]
    [SerializeField] protected string[] dialogLines;

    [Header("Misc")]
    [SerializeField] protected ParticleSystem hitParticleEmission;
    [SerializeField] protected bool flipSprite;

    [Header("Events")]
    [SerializeField] protected UnityEvent onHit;
    [SerializeField] protected UnityEvent onFaseChange;
    [SerializeField] protected UnityEvent onDeath;

    [Header("Debug")]
    [SerializeField] protected bool debugTools = true;
    [SerializeField] protected bool drawDebug = true;
    [SerializeField] protected bool drawDebugWhenHappening = true;
    [SerializeField] protected Transform debugDrawCenter;
    [SerializeField] protected Color tooCloseColor = new Color(0, 1, 0, 1);
    [SerializeField] protected Color wallAvoidanceColor = new Color(1, 1, 0, 1);
    [SerializeField] protected Color closeAttackColor = new Color(1, 0, 0, 1);
    [SerializeField] protected Color farAttackColor = new Color(1, 0.5f, 0, 1);
    [SerializeField] protected int segments = 8;
    #endregion

    #region Unity Methods

    protected virtual void Awake()
    {
        SetAnimHolder();

        rb = GetComponent<Rigidbody>();
        pivot = GetComponentInParent<Transform>();
        audioSource = GetComponent<AudioSource>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        if (hitboxOffset == 0) hitboxOffset = 1;
        currentHealth = health;
        currentFase = 1;

        _particleEmission = hitParticleEmission.emission;
        _particleEmission.enabled = false;

        if (FindObjectOfType<BossUI>()) FindObjectOfType<BossUI>().SetUI(this);
        if (debugDrawCenter == null) debugDrawCenter = this.transform;

        //animHolder.GetAnimationController().PlayAnimation(animationIDs[0], null, false);
        PlayAnimation(0, false);
        PlaySound(spawnSounds);

        NotifyBossesObservers(AllBossActions.Spawned);

        //DowngradeSystem.Instance.SetEnemy(this);
    }

    protected virtual void Update()
    {
        if (GameManager.Instance.IsGamePaused() || isDead)
            return;

        AllUtilityCallback();
        Attack();
        Hitbox();
        Movement();
        
    }

    #endregion

    #region Movement

    protected virtual void Movement()
    {
        if (!IsAnimationDone() || isAttacking || isSummoningObjects || hasQueuedAnimation) return;

        if (isOnCooldown)
        {
            //animHolder.GetAnimationController().PlayAnimation(animationIDs[1]);
            PlayAnimation(1);
            return;
        }

        if (TargetDistance() < tooCloseRange)
        {
            //animHolder.GetAnimationController().PlayAnimation(animationIDs[1]);
            PlayAnimation(1);
        }
        else if (Vector3.Distance(target.position, transform.position) > tooCloseRange)
        {
            transform.position += targetDir * moveSpeed * Time.deltaTime;

            isMoving = true;
            //animHolder.GetAnimationController().PlayAnimation(animationIDs[2]);
            PlayAnimation(2);
        }
        else
        {
            isMoving = false;
        }
    }

    #endregion

    #region Attack

    protected virtual void Attack()
    {
        if (!IsAnimationDone() || isOnCooldown || decidedFarAttack) return;

        if (TargetDistance() <= closeAttackRange)
        {
            isAttacking = true;
            isOnCooldown = true;
            //animHolder.GetAnimationController().PlayAnimation(animationIDs[3], null, true);
            PlayAnimation(3, true);

            // animation event for hitbox activation and deactivation
            // animation event to apply force to the rigidbody
        }
        else if (TargetDistance() <= farAttackRange && TargetDistance() > closeAttackRange &&  !hasConsideredFarAttack)
        {
            int random = Random.Range(0, maxOdds + 1);

            if (!decidedFarAttack)
            {
                if (random < farAttackOdds)
                {
                    isAttacking = true;
                    decidedFarAttack = true;
                    isOnCooldown = true;

                    //animHolder.GetAnimationController().PlayAnimation(animationIDs[4], null, true); // animation for the far attack
                    PlayAnimation(4, true);
                    // animation event to instantiate the spikes
                }
            }

            hasConsideredFarAttack = true;
            Invoke("ResetConsideredFarAttackDecitionStatus", farAttackDecitionCooldown);
        }
        else
        {
            if (isAttacking) isAttacking = false;
        }

        SetTargetDirection(false);
    }

    

    public virtual void Hitbox()
    {
        if (!isHitboxOn) return;

        Collider[] hitColliders = Physics.OverlapBox(hitboxCenter.position, hitboxSize, Quaternion.LookRotation(lastTargetDir));
        if (hitColliders.Length > 0) HitboxHit(hitColliders);
    }

    protected virtual void HitboxHit(Collider[] hit)
    {
        foreach (Collider hitCollider in hit)
        {
            if (hitCollider.CompareTag(playerTag))
            {
                if (hitCollider.GetComponent<PlayerControllerOverhaul>().GetPlayerState() == "Parry")
                {
                    float dotProd = Vector3.Dot(hitCollider.GetComponent<PlayerControllerOverhaul>().GetLastDirection(), targetDir);
                    if (dotProd <= -0.45f && dotProd >= -1)
                    {
                        hitCollider.GetComponent<PlayerControllerOverhaul>().GetParryRewardProxy(false, true);
                        hitCollider.GetComponent<PlayerControllerOverhaul>().TakeDamageProxy(0, parryKnockback, -targetDir);
                        Debug.Log("Player parried hit");
                    }
                    else
                    {
                        hitCollider.GetComponent<PlayerControllerOverhaul>().TakeDamageProxy(attackdamage, attackKnockback, -targetDir);
                        Debug.Log("Player normal hit - Failed Parry");
                    }
                }
                else
                {
                    hitCollider.GetComponent<PlayerControllerOverhaul>().TakeDamageProxy(attackdamage, attackKnockback, -targetDir);
                    Debug.Log("Player normal hit");
                }

                HitboxController(false);
            }
        }
    }

    #endregion

    #region Take Damage

    public virtual void TakeDamage(float damage)
    {
        onHit?.Invoke();
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    protected virtual void Death()
    {
        if (currentFase < ammountOfFases && ammountOfFases > 1)
        {
            onFaseChange?.Invoke();
            //animHolder.GetAnimationController().PlayAnimation(animationIDs[!], null, true); // 2nd fase anim
        }
        else
        {
            onDeath?.Invoke();
            isDead = true;
        }
    }

    #endregion

    #region Specials

    public void SpawnObjectOnAttack()
    {
        GameObject spikes = Instantiate(spawnable, transform.position, Quaternion.identity);
        //spikes.transform.position += spikes.transform.forward * zOffset;
        spikes.GetComponent<Spikes>().SetVariables(specialDamage, specialKnockback, lifeTime, lastTargetDir);
    }

    #endregion

    #region Utility

    protected virtual void AllUtilityCallback()
    {
        SetTargetDirection(true);
        FlipPivot();
        RotateHitboxCentreToFaceThePlayer();
    }

    #region Targeting

    protected virtual void SetTargetDirection(bool value)
    {
        Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 enemyPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 dir = (targetPos - enemyPos).normalized;

        if (value)
            targetDir = dir;
        else
            lastTargetDir = dir;
    }

    protected virtual float TargetDistance()
    {
        return Vector3.Distance(target.position, transform.position);
    }

    #endregion

    #region Wall Avoidance

    /*protected virtual void GetNearestWall(Vector3 dir)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, wallAvoidanceRange);

        foreach (Collider c in hitColliders)
        {
            if (c.CompareTag("Enemy") && c != GetComponent<Collider>() || c.CompareTag("Wall") || c.CompareTag("Destructible") || c.CompareTag("Limits"))
            {
                Vector3 wallDir = (c.transform.position - transform.position).normalized;
                wallDir.y = 0f;

                targetDir = Vector3.Slerp(targetDir, -wallDir, Time.deltaTime * wallAvoidanceSpeed);
                //targetDir = -wallDir;
            }
            else
            {
                targetDir = dir;
                targetDir = Vector3.Slerp(targetDir, dir.normalized, Time.deltaTime * wallAvoidanceSpeed);
            }
        }
    }*/

    #endregion

    #region Hitbox

    protected virtual void RotateHitboxCentreToFaceThePlayer()
    {
        if (isAttacking) return;

        Vector3 direction = (target.position - transform.position).normalized * hitboxOffset;
        Vector3 desiredPosition = transform.position + direction;
        Quaternion rotation = Quaternion.LookRotation(direction);
        hitboxCenter.rotation = rotation;
        hitboxCenter.position = new Vector3(desiredPosition.x, hitboxCenter.position.y, desiredPosition.z);
    }

    public virtual void HitboxController(bool value)
    {
        isHitboxOn = value;
    }

    #endregion

    #region Flip

    protected virtual void FlipPivot()
    {
        if (!IsAnimationDone() || isAttacking || isSummoningObjects || hasQueuedAnimation) return;

        Vector3 direction = (target.position - transform.position).normalized;

        if (direction.x > 0)
        {
            Flip(true);
        }
        else
        {
            Flip(false);
        }
    }

    protected virtual void Flip(bool value)
    {
        if (flipSprite)
        {
            pivot.localScale = value ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);

            isSpriteFlipped = value ? true : false;
        }
        else
        {
            pivot.localScale = value ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);

            isSpriteFlipped = value ? false : true;
        }
    }

    #endregion

    #region Animation

    private void PlayAnimation(int index, bool hasExitTime = false, bool bypassExitTime = false, bool canBeBypassed = false)
    {
        animHolder.GetAnimationController().PlayAnimation(animationIDs[index], null, hasExitTime, bypassExitTime, canBeBypassed);
    }

    private bool IsAnimationDone()
    {
        return animHolder.GetAnimationController().isAnimationDone;
    }

    public void SetAnimHolder()
    {
        animHolder = GetComponent<AnimationHolder>();
        animHolder.Initialize(GetComponentInChildren<Animator>());
        animationIDs = animHolder.GetAnimationsIDs();
    }

    #endregion

    #region Audio

    protected virtual void PlaySound(AudioClip[] clip)
    {
        if (clip.Length > 0)
        {
            int random = Random.Range(0, clip.Length);
            AudioManager.instance.PlayCustomSFX(clip[random], audioSource);
        }
        else
        {
            AudioManager.instance.PlayCustomSFX(clip[0], audioSource);
        }
    }

    #endregion

    #region Set Variables / Events Callback

    public virtual void SetNewFase()
    {
        if (isDead) isDead = false;
        ChangeFaseUI();
        health = 550;
        currentHealth = health;
    }
    public virtual void SetAttack(bool value)
    {
        isAttacking = value;
        Debug.Log("Is attacking: " + isAttacking);
    }

    public virtual void SetFarAttackDecision(bool value)
    {
        decidedFarAttack = value;
        Debug.Log("Decided far attack: " + decidedFarAttack);
    }

    public virtual void SetCooldown(bool value)
    {
        isOnCooldown = value;
        Debug.Log("Is on cooldown: " + isOnCooldown);
    }

    public virtual void MoveOnAttack(float force)
    {
        rb.velocity = lastTargetDir * force;
    }

    public virtual void EnableHitParticle(float resetTime)
    {
        if (resetTime == 0) resetTime = 0.1f;

        _particleEmission.enabled = true;
        Invoker.InvokeDelayed(ResetHitParticle, resetTime);
    }

    public virtual void ChangeFaseUI()
    {
        FindObjectOfType<BossUI>().SetChangeFase();
    }
    #endregion

    #region Get Variables

    public string GetBossName() { return bossName; }
    public float GetHealth() { return health; }
    public float GetCurrentHealth() { return currentHealth; }
    public int GetAllFases() { return ammountOfFases; }
    public int GetCurrentFase() { return currentFase; }

    #endregion

    #region Invokes

    protected virtual void ResetHitParticle()
    {
        _particleEmission.enabled = false;
    }
    protected virtual void ResetConsideredFarAttackDecitionStatus()
    {
        hasConsideredFarAttack = false;
    }
    #endregion

    #endregion

    #region Debug

    public void DrawHitbox()
    {
        if (lastTargetDir == Vector3.zero) VisualizeBox.DisplayBox(hitboxCenter.position, hitboxSize, Quaternion.identity, closeAttackColor);
        else VisualizeBox.DisplayBox(hitboxCenter.position, hitboxSize, Quaternion.LookRotation(lastTargetDir), closeAttackColor);
    }

    public void DrawWallAvoidance()
    {
        // Draws a line in the direction of the wall the enemy is avoiding
        Vector3 direction = targetDir;
        Debug.DrawRay(transform.position, direction * wallAvoidanceRange, wallAvoidanceColor);
    }

    public void DrawRange(float range, Color color)
    {
        Vector3 center = debugDrawCenter.position;
        float angleIncrement = 360.0f / segments;

        Vector3 prevPoint = center + new Vector3(range, 0, 0); // Start point

        // Draw segments to approximate the circle
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleIncrement;
            Vector3 nextPoint = center + new Vector3(range * Mathf.Cos(angle * Mathf.Deg2Rad), 0, range * Mathf.Sin(angle * Mathf.Deg2Rad));

            Debug.DrawLine(prevPoint, nextPoint, color); // Draw line segment
            prevPoint = nextPoint;
        }

        // Draw the last segment to close the circle
        Debug.DrawLine(prevPoint, center + new Vector3(range, 0, 0), color);
    }

    private void OnDrawGizmos()
    {
        if (!debugTools) return;

        if (drawDebug)
        {
            if (drawDebugWhenHappening)
            {
                //return;
            }

            DrawHitbox();
            DrawWallAvoidance();
            DrawRange(tooCloseRange, tooCloseColor);
            DrawRange(wallAvoidanceRange, wallAvoidanceColor);
            DrawRange(closeAttackRange, closeAttackColor);
            DrawRange(farAttackRange, farAttackColor);
        }

    }

    #endregion
}
