using System.Collections.Generic;
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
    [SerializeField] protected float ammountOfFases;

    [Header("Ranges")]
    [SerializeField] protected float tooCloseRange;
    [SerializeField] protected float closeAttackRange;
    [SerializeField] protected float farAttackRange;
    [SerializeField] protected float wallAvoidanceRange;

    [Header("Health")]
    [SerializeField] protected float health;
    protected float currentHealth;

    [Header("Attack")]
    [SerializeField] protected float attackdamage;
    [SerializeField] protected float attackKnockback;
    [SerializeField] protected float parryKnockback;
    [SerializeField] protected int maxOdds;
    [SerializeField] protected int farAttackOdds;
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

    [Header("Debug")]
    [SerializeField] private bool debugTools = true;
    [SerializeField] private bool drawDebug = true;
    [SerializeField] private bool drawDebugWhenHappening = true;
    [SerializeField] private Transform debugDrawCenter;
    [SerializeField] private Color tooCloseColor = new Color(0, 1, 0, 1);
    [SerializeField] private Color wallAvoidanceColor = new Color(1, 1, 0, 1);
    [SerializeField] private Color closeAttackColor = new Color(1, 0, 0, 1);
    [SerializeField] private Color farAttackColor = new Color(1, 0.5f, 0, 1);
    [SerializeField] private int segments = 8;
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

        if (debugDrawCenter == null) debugDrawCenter = this.transform;

        animHolder.GetAnimationController().PlayAnimation(animationIDs[0], null, false);
        PlaySound(spawnSounds);

        NotifyBossesObservers(AllBossActions.Spawned);

        //DowngradeSystem.Instance.SetEnemy(this);
    }

    protected virtual void Update()
    {
        if (GameManager.Instance.IsGamePaused())
            return;

        AllUtilityCallback();
        Attack();
        Movement();
    }

    #endregion

    #region Movement

    protected virtual void Movement()
    {
        if (!animHolder.GetAnimationController().isAnimationDone || isAttacking || isSummoningObjects || hasQueuedAnimation) return;

        if (isOnCooldown)
        {
            animHolder.GetAnimationController().PlayAnimation(animationIDs[1]);
            return;
        }

        if (TargetDistance() < tooCloseRange)
        {
            animHolder.GetAnimationController().PlayAnimation(animationIDs[1]);
        }
        else if (Vector3.Distance(target.position, transform.position) > tooCloseRange)
        {
            transform.position += targetDir * moveSpeed * Time.deltaTime;

            isMoving = true;
            animHolder.GetAnimationController().PlayAnimation(animationIDs[2]);
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
        if (!animHolder.GetAnimationController().isAnimationDone || isOnCooldown || decidedFarAttack) return;

        if (TargetDistance() <= closeAttackRange)
        {
            isAttacking = true;
            isOnCooldown = true;
            animHolder.GetAnimationController().PlayAnimation(animationIDs[3], null, true);

            // animation event for hitbox activation and deactivation
            // animation event to apply force to the rigidbody
        }
        else if (TargetDistance() <= farAttackRange && !hasConsideredFarAttack)
        {
            int random = Random.Range(0, maxOdds + 1);

            if (!decidedFarAttack)
            {
                if (random < farAttackOdds)
                {
                    isAttacking = true;
                    decidedFarAttack = true;

                    animHolder.GetAnimationController().PlayAnimation(animationIDs[3], null, true); // animation for the far attack
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

        SetLastTargetDirection();
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
                if (hitCollider.GetComponent<PlayerComponent>().GetPlayerState() == "Parry")
                {
                    float dotProd = Vector3.Dot(hitCollider.GetComponent<PlayerComponent>().GetLastDirection(), targetDir);
                    if (dotProd <= -0.45f && dotProd >= -1)
                    {
                        hitCollider.GetComponent<PlayerComponent>().GetParryReward(false, true);
                        hitCollider.GetComponent<PlayerComponent>().TakeDamage(0, parryKnockback, -targetDir);
                        Debug.Log("Player parried hit");
                    }
                    else
                    {
                        hitCollider.GetComponent<PlayerComponent>().TakeDamage(attackdamage, attackKnockback, -targetDir);
                        Debug.Log("Player normal hit - Failed Parry");
                    }
                }
                else
                {
                    hitCollider.GetComponent<PlayerComponent>().TakeDamage(attackdamage, attackKnockback, -targetDir);
                    Debug.Log("Player normal hit");
                }

                HitboxController(false);
            }
        }
    }

    #endregion

    #region Take Damage

    protected virtual void TakeDamage(float damage)
    {
    }

    #endregion

    #region Utility

    protected virtual void AllUtilityCallback()
    {
        SetTargetDirection();
        FlipPivot();
        RotateHitboxCentreToFaceThePlayer();
    }

    #region Targeting

    protected virtual void SetTargetDirection()
    {
        Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 enemyPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 dir = (targetPos - enemyPos).normalized;
        targetDir = dir;
    }

    protected virtual void SetLastTargetDirection()
    {
        Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 enemyPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 dir = (targetPos - enemyPos).normalized;
        lastTargetDir = dir;
    }

    protected virtual float TargetDistance()
    {
        return Vector3.Distance(target.position, transform.position);
    }

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
        if (!animHolder.GetAnimationController().isAnimationDone || isAttacking || isSummoningObjects || hasQueuedAnimation) return;

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

    #region Invokes
    protected virtual void ResetConsideredFarAttackDecitionStatus()
    {
        hasConsideredFarAttack = false;
    }
    #endregion

    #region Set Variables
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
    #endregion

    #endregion

    #region Debug

    public void DrawHitbox()
    {
        VisualizeBox.DisplayBox(hitboxCenter.position, hitboxSize, Quaternion.LookRotation(lastTargetDir), closeAttackColor);
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
