using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    // PlaceHolder for the EnemyBase

    // TO DO: Add Stunned and Parried behaviour
    // TO DO: Add a way to avoid the player and attack from a distance
    // TO DO: Add a way for the enemy to damage the player / half way done, make the player get damaged when the enemy attacks
    // TO DO: Add a way for the enemy to take damage from the player

    #region Variables
    [HideInInspector] public Animator anim;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Transform pivot;
    [HideInInspector] public Transform target;
    [HideInInspector] public Vector3 lastTargetDir;

    [HideInInspector] public float animClipLength;
    [HideInInspector] public float animTimer;
    [HideInInspector] public bool isAnimationDone;

    [HideInInspector] public bool isSpawning;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isNormalOverlapAttack;
    [HideInInspector] public bool isChargedOverlapAttack;
    [HideInInspector] public bool isInvokedNormalOverlapAttack;
    [HideInInspector] public bool isInvokedChargedOverlapAttack;
    [HideInInspector] public bool isStunned; // TO DO
    [HideInInspector] public bool isParried; // TO DO  DOING...
    [HideInInspector] public bool isSpriteFlipped;
    [HideInInspector] public bool isOnCooldown;
    [HideInInspector] public bool decidedChargeAttack;
    [HideInInspector] public bool chargeAttackedConsidered;
    [HideInInspector] public bool avoidingTarget;

    [Header("AI Stats")]
    public float health = 100;
    public int damage = 5;
    public float walkingSpeed = 1;
    public bool canRun;
    [ShowIf("canRun", true, true)] public bool reverseRunLogic;
    [ShowIf("canRun", true, true)] public float runSpeed = 1.5f;
    [ShowIf("canRun", true, true)] public float runRange = 1.5f;

    public bool canBeParried = true;
    [ShowIf("canBeParried", true, true)] public bool canBeParryStunned;
    [ShowIf("canBeParried", true, true)] public Vector2 lightParryStunWindowTime = new Vector2(0.5f, 1.5f);

    [ShowIf("canBeParried", true, true)] public bool canParryChargeAttack;
    [ShowIf("canBeParried", true, true)] [ShowIf("canParryChargeAttack", true, true)] public Vector2 chargedParryStunWindowTime = new Vector2(0.5f, 1.5f);

    [HideInInspector] public float speed;

    [Header("AI StunTime")]
    public float stunTime = 2;

    [Header("AI Stop range from player")]
    public float tooClose = 0.3f;

    [Header("AI Odds for charge attack")]
    public int maxOdds = 1000;
    public int oddsToChargeAttack = 250;

    [Header("AI Attack variables")]
    [ShowIf("avoidTarget", false, true)] public bool isMelee;
    [ShowIf("isMelee", true, true)] public bool hasChargeAttack;
    [ShowIf("isMelee", true, true)] public float closeAttackRange = 0.8f;
    [ShowIf("isMelee", true, true)][ShowIf("hasChargeAttack", true, true)] public float farAttackRange = 2;

    [Header("AI Attack impulse")]
    [ShowIf("isMelee", true, true)] public float moveOnNormalAttackForce = 10;
    [ShowIf("isMelee", true, true)] public float normalMoveAttackActivationTime = 0;

    [ShowIf("isMelee", true, true)] [ShowIf("hasChargeAttack", true, true)] public float moveOnChargeAttackForce = 60;
    [ShowIf("isMelee", true, true)] [ShowIf("hasChargeAttack", true, true)] public float chargeMoveAttackActivationTime = 0;

    public Vector2 normalAttackHitboxAppearTime = new Vector2(0.2f, 0.5f);
    public Vector3 normalAttackHitboxPos = new Vector3(0, 0, 0);
    public Vector3 normalAttackHitboxSize = new Vector3(0.5f, 0.5f, 0.5f);

    [ShowIf("isMelee", true, true)] [ShowIf("hasChargeAttack", true, true)] public Vector2 chargedAttackHitboxAppearTime = new Vector2(0.2f, 0.5f);
    [ShowIf("isMelee", true, true)] [ShowIf("hasChargeAttack", true, true)] public Vector3 chargedAttackHitboxPos = new Vector3(0, 0, 0);
    [ShowIf("isMelee", true, true)] [ShowIf("hasChargeAttack", true, true)] public Vector3 chargedAttackHitboxSize = new Vector3(1, 1, 1);

    [Header("AI Cooldown")]
    public float attackCooldown = 0.5f;
    [ShowIf("isMelee", true, true)][ShowIf("hasChargeAttack", true, true)] public float chargeDecitionCooldown = 2.5f;

    [Header("AI Avoidance")]
    [ShowIf("isMelee", false, true)] public bool avoidTarget;
    [ShowIf("isMelee", false, true)][ShowIf("avoidTarget", true, true)] public float avoidRange = 2;

    [Header("AI Animations")]
    public string[] animationIDs;
    AnimationClip[] clips;

    [Header("Debug")]
    [SerializeField] private bool debugTools = true;
    [ShowIf("debugTools", true, true)] [SerializeField] private bool drawHitboxes = true;
    [ShowIf("debugTools", true, true)][ShowIf("drawHitboxes", true, true)] [SerializeField] private bool drawHitboxesOnGameplay = true;
    [ShowIf("debugTools", true, true)] [SerializeField] private Transform debugDrawCenter;
    [ShowIf("debugTools", true, true)] [SerializeField] private Color runRageColor = new Color(1, 0, 1,  1);
    [ShowIf("debugTools", true, true)] [SerializeField] private Color tooCloseColor = new Color(0, 1, 0, 1);
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
    }

    public virtual void Movement()
    {
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            RemoveComponentsOnDeath();
        }
    }

    public virtual void Death()
    {
    }

    #region Attack Behaviour
    public void Attack()
    {
        if (isStunned || !isAnimationDone || isOnCooldown || decidedChargeAttack)
        {
            return;
        }

        if (isMelee)
        {
            MeleeBehaviour();
        }
        else if (avoidTarget)
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
        // Check if the player is in range, if its in the avoid range, move away from the player
        if (Vector3.Distance(target.position, transform.position) <= avoidRange)
        {
            // Move away from the player
            if (isAttacking == true)
            {
                isAttacking = false;
            }
        }
        else
        {
            // Attack the player
            if (isAttacking == false)
            {
                isAttacking = true;
            }
        }
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

        Vector3 temp = normalAttackHitboxPos;
        if (isSpriteFlipped)
        {
            temp.x *= -1;
        }

        Collider[] hitColliders = Physics.OverlapBox(transform.position + debugDrawCenter.TransformDirection(temp), normalAttackHitboxSize / 2, Quaternion.identity);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Debug.Log("Player normal hit");
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

        Vector3 temp = chargedAttackHitboxPos;
        if (isSpriteFlipped)
        {
            temp.x *= -1;
        }

        Collider[] hitColliders = Physics.OverlapBox(transform.position + debugDrawCenter.TransformDirection(temp), chargedAttackHitboxSize / 2, Quaternion.identity);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Debug.Log("Player charged hit");
                RemoveAttackHitboxes();
            }
        }
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
    }

    public void ResetStun()
    {
        isStunned = false;
    }
    #endregion

    #region Cooldown
    public void StartCooldown()
    {
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

                Invoke("ResetAnimClipLenght", animClipLength);
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

                Invoke("ResetAnimClipLenght", animClipLength);

                if (activateCooldown)
                {
                    Invoke("StartCooldown", animClipLength);
                }

                return;
            }
        }
    }

    public void ResetAnimClipLenght()
    {
        isAnimationDone = true;
        animClipLength = 0;
        animTimer = 0;

        if (isSpawning)
            isSpawning = false;
    }
    #endregion

    #region Utility
    public void RemoveComponentsOnDeath()
    {
        Destroy(rb);
        Destroy(GetComponent<Collider>());
        this.enabled = false;
    }

    public void FlipPivot()
    {
        if (isStunned || !isAnimationDone || isAttacking)
            return;

        Vector3 direction = (target.position - transform.position).normalized;

        if (direction.x > 0)
        {
            pivot.localScale = new Vector3(1, 1, 1);

            if (isSpriteFlipped)
                isSpriteFlipped = false;
        }
        else
        {
            pivot.localScale = new Vector3(-1, 1, 1);

            if (!isSpriteFlipped)
                isSpriteFlipped = true;
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

        if (damage <= 0)
            Debug.LogError("Damage is missing!");

        if (speed <= 0)
            Debug.LogError("Speed is missing!");
    }

    public void DrawHitboxes()
    {
        Vector3 normalTemp = normalAttackHitboxPos;
        Vector3 chargedTemp = chargedAttackHitboxPos;
        if (isSpriteFlipped)
        {
            normalTemp.x *= -1;
            chargedTemp.x *= -1;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + debugDrawCenter.TransformDirection(normalTemp), normalAttackHitboxSize);

        if (hasChargeAttack)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + debugDrawCenter.TransformDirection(chargedTemp), chargedAttackHitboxSize);
        }
    }

    public void DrawNormalAttackHitbox()
    {
        Vector3 temp = normalAttackHitboxPos;
        if (isSpriteFlipped)
        {
            temp.x *= -1;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + debugDrawCenter.TransformDirection(temp), normalAttackHitboxSize);
    }

    public void DrawChargedAttackHitbox()
    {
        Vector3 temp = chargedAttackHitboxPos;
        if (isSpriteFlipped)
        {
            temp.x *= -1;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + debugDrawCenter.TransformDirection(temp), chargedAttackHitboxSize);
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

        if (drawHitboxes)
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
                DrawHitboxes();
            }
        }

        DrawTooCloseRange();

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
    }
    #endregion
}
