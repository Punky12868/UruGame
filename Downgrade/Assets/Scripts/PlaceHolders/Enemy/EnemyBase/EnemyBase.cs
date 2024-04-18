using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    // PlaceHolder for the EnemyBase

    #region Variables
    [HideInInspector] public Animator anim;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Transform pivot;
    [HideInInspector] public Transform target;
    [HideInInspector] public Vector3 lastTargetDir;
    [HideInInspector] public Vector3 targetDir;

    [HideInInspector] public float animClipLength;
    [HideInInspector] public bool isAnimationDone;

    [HideInInspector] public bool isBigEnemy;
    [HideInInspector] public bool isSpawning;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isNormalOverlapAttack;
    [HideInInspector] public bool isChargedOverlapAttack;
    [HideInInspector] public bool isInvokedNormalOverlapAttack;
    [HideInInspector] public bool isInvokedChargedOverlapAttack;
    [HideInInspector] public bool isStunned;
    [HideInInspector] public bool isParried;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isSpriteFlipped;
    [HideInInspector] public bool isOnCooldown;
    [HideInInspector] public bool decidedChargeAttack;
    [HideInInspector] public bool chargeAttackedConsidered;
    [HideInInspector] public bool avoidingTarget;
    [HideInInspector] public bool hasQueuedAnimation;

    [Header("AI Stats")]
    public float health = 100;
    [HideInInspector] public float currentHealth;
    public int normalAttackdamage = 5;
    public int normalAttackKnockback = 5;
    [ShowIf("isMelee", true, true)][ShowIf("hasChargeAttack", true, true)][ShowIf("isMelee", true, true)][ShowIf("hasChargeAttack", true, true)] public int chargeAttackDamage = 5;
    [ShowIf("isMelee", true, true)][ShowIf("hasChargeAttack", true, true)][ShowIf("isMelee", true, true)][ShowIf("hasChargeAttack", true, true)] public int chargeAttackKnockback = 15;
    [HideInInspector] public float speed;
    public float walkingSpeed = 1;
    public bool canRun;
    [ShowIf("canRun", true, true)] public bool reverseRunLogic;
    [ShowIf("canRun", true, true)] public float runSpeed = 1.5f;
    [ShowIf("canRun", true, true)] public float runRange = 1.5f;

    [ShowIf("isMelee", false, true)] public bool isStatic ;
    [ShowIf("isMelee", false, true)] public float projectileSpawnTime;
    [ShowIf("isMelee", false, true)] public float projectileLifeTime;
    [ShowIf("isMelee", false, true)] public float projectileSpeed;
    [ShowIf("isMelee", false, true)] public GameObject projectile;
    [ShowIf("isMelee", false, true)] public Transform projectileSpawnPoint;

    public bool hasKnockback;
    [ShowIf("hasKnockback", true, true)] public float knockbackForce = 5.5f;
    public bool canBeParried = true;
    [ShowIf("canBeParried", true, true)] public bool projectileCanBeParried = false;
    [ShowIf("isMelee", true, true)] [ShowIf("canBeParried", true, true)] [ShowIf("projectileCanBeParried", false, true)] public bool canBeParryStunned;
    [ShowIf("isMelee", true, true)] [ShowIf("canBeParried", true, true)] [ShowIf("projectileCanBeParried", false, true)] public bool canParryChargeAttack;
    [ShowIf("isMelee", true, true)] [ShowIf("canBeParried", true, true)] [ShowIf("projectileCanBeParried", false, true)] public float parryStunTime = 3;

    [Header("AI StunTime")]
    [ShowIf("canBeParryStunned", true, true)]  public float stunTime = 2;

    [Header("AI Stop range from player")]
    public float tooClose = 0.3f;

    [Header("AI Odds for charge attack")]
    [ShowIf("isMelee", true, true)] [ShowIf("hasChargeAttack", true, true)] public int maxOdds = 1000;
    [ShowIf("isMelee", true, true)] [ShowIf("hasChargeAttack", true, true)] public int oddsToChargeAttack = 250;

    [Header("AI Attack variables")]
    [ShowIf("avoidTarget", false, true)] public bool isMelee;
    [ShowIf("isMelee", true, true)] [SerializeField] Transform hitboxCenter;
    [ShowIf("isMelee", true, true)] [SerializeField] float hitboxOffset;
    [ShowIf("isMelee", true, true)] public bool hasChargeAttack;
    [ShowIf("isMelee", true, true)] public float closeAttackRange = 0.8f;
    [ShowIf("isMelee", true, true)] [ShowIf("hasChargeAttack", true, true)] public float farAttackRange = 2;

    [Header("AI Attack impulse")]
    public float moveOnNormalAttackForce = 10;
    [ShowIf("isMelee", true, true)] public float normalMoveAttackActivationTime = 0;

    [ShowIf("isMelee", true, true)] [ShowIf("hasChargeAttack", true, true)] public float moveOnChargeAttackForce = 60;
    [ShowIf("isMelee", true, true)] [ShowIf("hasChargeAttack", true, true)] public float chargeMoveAttackActivationTime = 0;

    [ShowIf("isMelee", true, true)] public Vector2 normalAttackHitboxAppearTime = new Vector2(0.2f, 0.5f);
    [ShowIf("isMelee", true, true)] public Vector3 normalAttackHitboxSize = new Vector3(0.5f, 0.5f, 0.5f);

    [ShowIf("isMelee", true, true)] [ShowIf("hasChargeAttack", true, true)] public Vector2 chargedAttackHitboxAppearTime = new Vector2(0.2f, 0.5f);
    [ShowIf("isMelee", true, true)] [ShowIf("hasChargeAttack", true, true)] public Vector3 chargedAttackHitboxSize = new Vector3(1, 1, 1);

    [Header("AI Cooldown")]
    public float attackCooldown = 0.5f;
    [ShowIf("isMelee", true, true)][ShowIf("hasChargeAttack", true, true)] public float chargeDecitionCooldown = 2.5f;

    [Header("AI Avoidance")]
    [ShowIf("isStatic", false, true)] public float enemyAvoidanceRange = 0.5f;
    [ShowIf("isStatic", false, true)] public float wallAvoidanceSpeed = 7.5f;
    [ShowIf("isMelee", false, true)][ShowIf("isStatic", false, true)] public bool avoidTarget;
    [ShowIf("isMelee", false, true)][ShowIf("avoidTarget", true, true)][ShowIf("isStatic", false, true)] public float avoidRange = 2;

    [Header("AI Animations")]
    [HideInInspector] public string queueAnimation;
    public string[] animationIDs;
    AnimationClip[] clips;

    [Header("Debug")]
    [SerializeField] private bool debugTools = true;
    [ShowIf("debugTools", true, true)] [SerializeField] private bool drawHitboxes = true;
    [ShowIf("debugTools", true, true)][ShowIf("drawHitboxes", true, true)] [SerializeField] private bool drawHitboxesOnGameplay = true;
    [ShowIf("debugTools", true, true)] [SerializeField] private Transform debugDrawCenter;
    [ShowIf("debugTools", true, true)] [SerializeField] private Color runRageColor = new Color(1, 0, 1,  1);
    [ShowIf("debugTools", true, true)] [SerializeField] private Color tooCloseColor = new Color(0, 1, 0, 1);
    [ShowIf("debugTools", true, true)] [SerializeField] private Color wallAvoidanceColor = new Color(1, 1, 0, 1);
    [ShowIf("debugTools", true, true)] [SerializeField] private Color closeAttackColor = new Color(1, 0, 0, 1);
    [ShowIf("debugTools", true, true)] [SerializeField] private Color farAttackColor = new Color(1, 0.5f, 0, 1);
    [ShowIf("debugTools", true, true)] [SerializeField] private Color avoidRangeColor = new Color(1, 1, 0, 1);
    [ShowIf("debugTools", true, true)] [SerializeField] private int segments = 8; // Number of line segments to approximate the circle
    #endregion

    public virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        pivot = GetComponentInParent<Transform>();
        clips = anim.runtimeAnimatorController.animationClips;
        target = GameObject.FindGameObjectWithTag("Player").transform;

        if (isStatic)
        {
            speed = 0;
        }
        else
        {
            speed = walkingSpeed;
        }

        currentHealth = health;

        if (debugDrawCenter == null)
            debugDrawCenter = this.transform;

        isAnimationDone = true;

        

        CheckStatus();

        isSpawning = true;
        PlayAnimation(animationIDs[0], true);
    }

    /*public virtual void FixedUpdate()
    {
        OnCooldown();
        Movement();
        Attack();
        FlipPivot();
    }*/

    public virtual void Update()
    {
        DoNormalAttackOverlapCollider(normalAttackHitboxAppearTime.y);
        DoChargeAttackOverlapCollider(chargedAttackHitboxAppearTime.y);
        ResetAnimClipUpdate();
        RotateHitboxCentreToFaceThePlayer();
        SetTargetDirection();
        QueueAnimation();
    }

    public void SetTargetDirection()
    {
        Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 enemyPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 dir = (targetPos - enemyPos).normalized;
        GetNearestEnemy(dir);
    }

    public void GetNearestEnemy(Vector3 dir)
    {
        if (avoidTarget && !isMelee)
        {
            targetDir = dir;
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyAvoidanceRange);

        foreach (Collider c in hitColliders)
        {
            if (c.CompareTag("Enemy") && c != GetComponent<Collider>() || c.CompareTag("Wall"))
            {
                Vector3 enemyDirection = (c.transform.position - transform.position).normalized;
                enemyDirection.y = 0f;

                //targetDir = Vector3.Slerp(targetDir, -enemyDirection, Time.deltaTime * wallAvoidanceSpeed);
                targetDir = -enemyDirection;
            }
            else
            {
                targetDir = dir;
                targetDir = Vector3.Slerp(targetDir, dir, Time.deltaTime * wallAvoidanceSpeed);
            }
        }
    }

    public virtual void Movement()
    {
    }

    public virtual void TakeDamage(float damage)
    {
    }

    public virtual void Death()
    {
    }

    #region Attack Behaviour
    public void Attack()
    {
        if (isStunned || isParried || !isAnimationDone || isOnCooldown || decidedChargeAttack)
        {
            return;
        }

        if (isMelee)
        {
            MeleeBehaviour();
        }
        else if (avoidTarget || isStatic)
        {
            AvoidBehaviour();
        }
        else
        {
            // Attack the player (Range combat)
            isAttacking = true;
        }

        Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 enemyPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 dir = (targetPos - enemyPos).normalized;
        lastTargetDir = dir;
    }

    public virtual void MeleeBehaviour()
    {
        // Check if the player is in range, if its  in the close attack range, attack.
        // if its in the far attack range, decide if attack or move to the player. if its out of range, move to the player
    }

    public void ResetDecitionStatus()
    {
        decidedChargeAttack = false;
    }
    public void ResetConsideredDecitionStatus()
    {
        chargeAttackedConsidered = false;
    }

    public virtual void AvoidBehaviour()
    {
    }

    public virtual void SummonProjectile()
    {
        GameObject prjctl = Instantiate(projectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        prjctl.GetComponent<ProjectileLogic>().SetVariables(projectileSpeed, normalAttackdamage, projectileLifeTime, normalAttackKnockback, projectileCanBeParried, targetDir /*lastTargetDir*/);
    }

    public virtual void MoveOnNormalAttack()
    {
    }

    public virtual void MoveOnChargeAttack()
    {
    }

    public void DoNormalAttackOverlapCollider(float removeTime)
    {
        if (!isNormalOverlapAttack)
            return;

        if (!isInvokedNormalOverlapAttack)
        {
            isInvokedNormalOverlapAttack = true;
            Invoke("RemoveAttackHitboxes", removeTime);
        }

        //Collider[] hitColliders = Physics.OverlapBox(hitboxCenter.position, chargedAttackHitboxSize, Quaternion.identity);
        Collider[] hitColliders = Physics.OverlapBox(hitboxCenter.position, normalAttackHitboxSize, Quaternion.LookRotation(lastTargetDir));
        //VisualizeBox.DisplayBox(hitboxCenter.position, normalAttackHitboxSize, Quaternion.LookRotation(direction), closeAttackColor);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                if (hitCollider.GetComponent<PlayerController>().GetPlayerState() == "Parry" && canBeParried)
                {
                    if (Vector3.Dot(hitCollider.GetComponent<PlayerController>().GetLastDirection(), targetDir) <= -0.5f && Vector3.Dot(hitCollider.GetComponent<PlayerController>().GetLastDirection(), targetDir) >= -1)
                    {
                        ParriedEnemy(parryStunTime);
                        hitCollider.GetComponent<PlayerController>().GetParryReward(isBigEnemy, false);
                        Debug.Log("Player parried hit");
                    }
                    else
                    {
                        hitCollider.GetComponent<PlayerController>().TakeDamage(normalAttackdamage, normalAttackKnockback, -targetDir);
                        Debug.Log("Player normal hit - Failed Parry");
                    }
                }
                else
                {
                    hitCollider.GetComponent<PlayerController>().TakeDamage(normalAttackdamage, normalAttackKnockback, -targetDir);
                    Debug.Log("Player normal hit");
                }

                RemoveAttackHitboxes();
            }
        }
    }

    public void DoChargeAttackOverlapCollider(float removeTime)
    {
        if (!isChargedOverlapAttack)
            return;

        if (!isInvokedChargedOverlapAttack)
        {
            isInvokedChargedOverlapAttack = true;
            Invoke("RemoveAttackHitboxes", removeTime);
        }

        //Collider[] hitColliders = Physics.OverlapBox(hitboxCenter.position, chargedAttackHitboxSize, Quaternion.identity);
        // rotate the hitbox to the direction of the player
        Collider[] hitColliders = Physics.OverlapBox(hitboxCenter.position, chargedAttackHitboxSize, Quaternion.LookRotation(lastTargetDir));
        //VisualizeBox.DisplayBox(hitboxCenter.position, chargedAttackHitboxSize, Quaternion.LookRotation(direction), farAttackColor);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                if (hitCollider.GetComponent<PlayerController>().GetPlayerState() == "Parry" && canBeParried && canParryChargeAttack)
                {
                    // dot product of the player direction and the enemy direction, if the player is facing the enemy, parry the charge attack
                    if (Vector3.Dot(hitCollider.GetComponent<PlayerController>().GetLastDirection(), targetDir) > 0.5f)
                    {
                        ParriedEnemy(parryStunTime);
                        hitCollider.GetComponent<PlayerController>().GetParryReward(isBigEnemy, true);
                        Debug.Log("Player parried charged hit");
                    }
                    else
                    {
                        hitCollider.GetComponent<PlayerController>().TakeDamage(chargeAttackDamage, chargeAttackKnockback, -targetDir);
                        Debug.Log("Player normal charged hit - Failed Parry");
                    }
                }
                else
                {
                    hitCollider.GetComponent<PlayerController>().TakeDamage(chargeAttackDamage, chargeAttackKnockback, -targetDir);
                    Debug.Log("Player normal charged hit");
                }

                RemoveAttackHitboxes();
            }
        }

        /*Gizmos.color = Color.red;

        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Gizmos.matrix = Matrix4x4.TRS(hitboxCenter.position, rotation, Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, normalAttackHitboxSize);

        Gizmos.matrix = Matrix4x4.identity;*/
    }

    public void ActivateNormalAttackHitbox(float time)
    {
        Invoke("NormalHitboxInvoke", time);
    }

    public void ActivateChargedAttackHitbox(float time)
    {
        Invoke("ChargedHitboxInvoke", time);
    }

    private void NormalHitboxInvoke()
    {
        isNormalOverlapAttack = true;

    }

    private void ChargedHitboxInvoke()
    {
        isChargedOverlapAttack = true;

    }

    public void RemoveAttackHitboxes()
    {
        isNormalOverlapAttack = false;
        isChargedOverlapAttack = false;

        isInvokedNormalOverlapAttack = false;
        isInvokedChargedOverlapAttack = false;
    }

    #endregion

    #region Stun
    public virtual void StunEnemy(float time)
    {
        isStunned = true;
        PlayAnimation(animationIDs[1], false, false, true);
        Debug.Log("Stunned");
        Invoke("ResetStun", time);
    }

    public virtual void ParriedEnemy(float time)
    {
    }

    public void ResetStun()
    {
        isStunned = false;
    }

    public void ResetParried()
    {
        isStunned = false;
        isParried = false;
    }
    #endregion

    #region Cooldown
    public void StartCooldown()
    {
        if (isStunned || isParried)
        {
            return;
        }

        isOnCooldown = true;
        Invoke("ResetCooldown", attackCooldown);

        int random = Random.Range(0, 2);

        if (random == 0)
        {
            avoidingTarget = true;
        }
        else
        {
            avoidingTarget = false;
        }
    }

    public virtual void OnCooldown()
    {
    }

    public void ResetCooldown()
    {
        isOnCooldown = false;
        isAttacking = false;
    }
    #endregion

    #region Animation
    public void PlayAnimation(string animName)
    {
        if (!isAnimationDone)
            return;

        for (int i = 0; i < animationIDs.Length; i++)
        {
            if (animName == animationIDs[i])
            {
                anim.Play(animName);
                return;
            }
        }
    }

    public void PlayAnimation(string animName, bool hasExitTime)
    {
        if (!isAnimationDone)
            return;

        for (int i = 0; i < animationIDs.Length; i++)
        {
            if (animName == animationIDs[i])
            {
                anim.Play(animName);

                if (hasExitTime)
                {
                    isAnimationDone = false;

                    foreach (AnimationClip clip in clips)
                    {
                        if (clip.name == animName)
                        {
                            animClipLength = clip.length;
                        }
                    }
                }
                return;
            }
        }
    }

    public void PlayAnimation(string animName, bool hasExitTime, bool activateCooldown)
    {
        if (!isAnimationDone)
            return;

        for (int i = 0; i < animationIDs.Length; i++)
        {
            if (animName == animationIDs[i])
            {
                anim.Play(animName);

                if (hasExitTime)
                {
                    isAnimationDone = false;

                    foreach (AnimationClip clip in clips)
                    {
                        if (clip.name == animName)
                        {
                            animClipLength = clip.length;
                        }
                    }
                }

                if (activateCooldown)
                {
                    Invoke("StartCooldown", animClipLength);
                }

                return;
            }
        }
    }

    public void PlayAnimation(string animName, bool hasExitTime, bool activateCooldown, bool bypassExitTime)
    {
        if (bypassExitTime)
            isAnimationDone = true;

        if (!isAnimationDone)
            return;

        for (int i = 0; i < animationIDs.Length; i++)
        {
            if (animName == animationIDs[i])
            {
                anim.Play(animName);

                if (hasExitTime)
                {
                    isAnimationDone = false;

                    foreach (AnimationClip clip in clips)
                    {
                        if (clip.name == animName)
                        {
                            animClipLength = clip.length;
                        }
                    }
                }

                if (activateCooldown)
                {
                    Invoke("StartCooldown", animClipLength);
                }

                return;
            }
        }
    }

    public void PlayAnimation(string animName, bool hasExitTime, bool activateCooldown, bool bypassExitTime, bool queuedAnimation)
    {
        if (bypassExitTime)
            isAnimationDone = true;

        if (queuedAnimation)
        {
            hasQueuedAnimation = true;
            queueAnimation = animName;
            return;
        }

        if (!isAnimationDone)
            return;

        for (int i = 0; i < animationIDs.Length; i++)
        {
            if (animName == animationIDs[i])
            {
                anim.Play(animName);

                if (hasExitTime)
                {
                    isAnimationDone = false;

                    foreach (AnimationClip clip in clips)
                    {
                        if (clip.name == animName)
                        {
                            animClipLength = clip.length;
                        }
                    }
                }

                if (activateCooldown)
                {
                    Invoke("StartCooldown", animClipLength);
                }

                return;
            }
        }
    }

    public void QueueAnimation()
    {
        if (hasQueuedAnimation && isAnimationDone)
        {
            PlayAnimation(queueAnimation, true, false);
            hasQueuedAnimation = false;
        }
    }

    public void ResetAnimClipUpdate()
    {
        if (animClipLength <= 0)
        {
            isAnimationDone = true;

            if (isSpawning)
                isSpawning = false;
        }
        else
        {
            animClipLength -= Time.deltaTime;
            isAnimationDone = false;
        }
    }
    #endregion

    #region Utility
    public void RemoveComponentsOnDeath()
    {
        isDead = true;
        FindObjectOfType<WaveSystem>().UpdateDeadEnemies();
        Destroy(rb);
        Destroy(GetComponent<Collider>());
        Destroy(this);
        //this.enabled = false;
    }

    public void FlipPivot()
    {
        if (isStunned || isParried || !isAnimationDone || isAttacking)
            return;

        Vector3 direction = (target.position - transform.position).normalized;

        if (avoidTarget)
        {
            if (avoidingTarget)
            {
                if (direction.x > 0)
                {
                    pivot.localScale = new Vector3(-1, 1, 1);

                    if (isSpriteFlipped)
                        isSpriteFlipped = true;
                }
                else
                {
                    pivot.localScale = new Vector3(1, 1, 1);

                    if (!isSpriteFlipped)
                        isSpriteFlipped = false;
                }
            }
            else
            {
                if (direction.x > 0)
                {
                    pivot.localScale = new Vector3(1, 1, 1);
                    //GetComponentInChildren<SpriteRenderer>().flipX = false;

                    if (isSpriteFlipped)
                        isSpriteFlipped = false;
                }
                else
                {
                    pivot.localScale = new Vector3(-1, 1, 1);
                    //GetComponentInChildren<SpriteRenderer>().flipX = true;

                    if (!isSpriteFlipped)
                        isSpriteFlipped = true;
                }
            }
        }
        else
        {
            if (direction.x > 0)
            {
                pivot.localScale = new Vector3(1, 1, 1);
                //GetComponentInChildren<SpriteRenderer>().flipX = false;

                if (isSpriteFlipped)
                    isSpriteFlipped = false;
            }
            else
            {
                pivot.localScale = new Vector3(-1, 1, 1);
                //GetComponentInChildren<SpriteRenderer>().flipX = true;

                if (!isSpriteFlipped)
                    isSpriteFlipped = true;
            }
        }
        
    }

    public void CheckStatus()
    {
        if (!debugTools)
            return;

        if (!GetComponent<Collider>())
            Debug.LogError("Collider is missing!. Add a Collider component to the enemy");

        if (rb == null)
            Debug.LogError("Rigidbody is missing!. Add a Rigidbody component to the enemy");

        if (anim == null)
            Debug.LogError("Animator is missing!. Add an Animator component to the enemy");

        if (target == null)
            Debug.LogError("Target is missing!. Set the Player tag to the target");

        if (animationIDs.Length <= 0)
            Debug.LogError("AnimationIDs are missing!");

        if (health <= 0)
            Debug.LogError("Health is missing!");

        if (normalAttackdamage <= 0)
            Debug.LogError("Damage is missing!");

        if (speed <= 0 && !isStatic)
            Debug.LogError("Speed is missing!");
    }

    public void RotateHitboxCentreToFaceThePlayer()
    {
        if (!isMelee || isAttacking)
            return;

        Vector3 direction = (target.position - transform.position).normalized * hitboxOffset;
        Vector3 desiredPosition = transform.position + direction;
        Quaternion rotation = Quaternion.LookRotation(direction);
        hitboxCenter.rotation = rotation;
        hitboxCenter.position = new Vector3(desiredPosition.x, hitboxCenter.position.y, desiredPosition.z);
    }

    public void DrawNormalAttackHitbox()
    {
        VisualizeBox.DisplayBox(hitboxCenter.position, normalAttackHitboxSize, Quaternion.LookRotation(lastTargetDir), closeAttackColor);
    }

    public void DrawChargedAttackHitbox()
    {
        VisualizeBox.DisplayBox(hitboxCenter.position, chargedAttackHitboxSize, Quaternion.LookRotation(lastTargetDir), farAttackColor);
    }

    public void DrawCloseAttackRange()
    {
        Vector3 center = debugDrawCenter.position;
        float angleIncrement = 360.0f / segments;

        Vector3 prevPoint = center + new Vector3(closeAttackRange, 0, 0); // Start point

        // Draw segments to approximate the circle
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleIncrement;
            Vector3 nextPoint = center + new Vector3(closeAttackRange * Mathf.Cos(angle * Mathf.Deg2Rad), 0, closeAttackRange * Mathf.Sin(angle * Mathf.Deg2Rad));

            Debug.DrawLine(prevPoint, nextPoint, closeAttackColor); // Draw line segment
            prevPoint = nextPoint;
        }

        // Draw the last segment to close the circle
        Debug.DrawLine(prevPoint, center + new Vector3(closeAttackRange, 0, 0), closeAttackColor);
    }

    public void DrawFarAttackRange()
    {
        Vector3 center = debugDrawCenter.position;
        float angleIncrement = 360.0f / segments;

        Vector3 prevPoint = center + new Vector3(farAttackRange, 0, 0); // Start point

        // Draw segments to approximate the circle
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleIncrement;
            Vector3 nextPoint = center + new Vector3(farAttackRange * Mathf.Cos(angle * Mathf.Deg2Rad), 0, farAttackRange * Mathf.Sin(angle * Mathf.Deg2Rad));

            Debug.DrawLine(prevPoint, nextPoint, farAttackColor); // Draw line segment
            prevPoint = nextPoint;
        }

        // Draw the last segment to close the circle
        Debug.DrawLine(prevPoint, center + new Vector3(farAttackRange, 0, 0), farAttackColor);
    }

    public void DrawAvoidRange()
    {
        Vector3 center = debugDrawCenter.position;
        float angleIncrement = 360.0f / segments;

        Vector3 prevPoint = center + new Vector3(avoidRange, 0, 0); // Start point

        // Draw segments to approximate the circle
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleIncrement;
            Vector3 nextPoint = center + new Vector3(avoidRange * Mathf.Cos(angle * Mathf.Deg2Rad), 0, avoidRange * Mathf.Sin(angle * Mathf.Deg2Rad));

            Debug.DrawLine(prevPoint, nextPoint, avoidRangeColor); // Draw line segment
            prevPoint = nextPoint;
        }

        // Draw the last segment to close the circle
        Debug.DrawLine(prevPoint, center + new Vector3(avoidRange, 0, 0), avoidRangeColor);
    }

    public void DrawTooCloseRange()
    {
        Vector3 center = debugDrawCenter.position;
        float angleIncrement = 360.0f / segments;

        Vector3 prevPoint = center + new Vector3(tooClose, 0, 0); // Start point

        // Draw segments to approximate the circle
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleIncrement;
            Vector3 nextPoint = center + new Vector3(tooClose * Mathf.Cos(angle * Mathf.Deg2Rad), 0, tooClose * Mathf.Sin(angle * Mathf.Deg2Rad));

            Debug.DrawLine(prevPoint, nextPoint, tooCloseColor); // Draw line segment
            prevPoint = nextPoint;
        }

        // Draw the last segment to close the circle
        Debug.DrawLine(prevPoint, center + new Vector3(tooClose, 0, 0), tooCloseColor);
    }

    public void DrawWallAvoidance()
    {
        // draws a line in the direction of the wall the enemy is avoiding
        Vector3 direction = targetDir;
        Debug.DrawRay(transform.position, direction * enemyAvoidanceRange, wallAvoidanceColor);
    }

    private void DrawRunRange()
    {
        Vector3 center = debugDrawCenter.position;
        float angleIncrement = 360.0f / segments;

        Vector3 prevPoint = center + new Vector3(runRange, 0, 0); // Start point

        // Draw segments to approximate the circle
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleIncrement;
            Vector3 nextPoint = center + new Vector3(runRange * Mathf.Cos(angle * Mathf.Deg2Rad), 0, runRange * Mathf.Sin(angle * Mathf.Deg2Rad));

            Debug.DrawLine(prevPoint, nextPoint, runRageColor); // Draw line segment
            prevPoint = nextPoint;
        }

        // Draw the last segment to close the circle
        Debug.DrawLine(prevPoint, center + new Vector3(runRange, 0, 0), runRageColor);
    }

    void OnDrawGizmos()
    {
        if (debugDrawCenter == null || !debugTools)
            return;

        if (drawHitboxes && target != null)
        {
            if (drawHitboxesOnGameplay)
            {
                if (isNormalOverlapAttack)
                    DrawNormalAttackHitbox();

                if (isChargedOverlapAttack)
                    DrawChargedAttackHitbox();
            }
            else
            {
                DrawNormalAttackHitbox();
                if (hasChargeAttack)
                    DrawChargedAttackHitbox();
            }
        }

        DrawTooCloseRange();
        DrawWallAvoidance();

        if (canRun)
            DrawRunRange();

        if (isMelee)
        {
            DrawCloseAttackRange();

            if (hasChargeAttack)
                DrawFarAttackRange();
        }
        else if (avoidTarget)
        {
            DrawAvoidRange();
        }

        Gizmos.color = wallAvoidanceColor;
        Gizmos.DrawWireSphere(transform.position, enemyAvoidanceRange);
    }
    #endregion
}
