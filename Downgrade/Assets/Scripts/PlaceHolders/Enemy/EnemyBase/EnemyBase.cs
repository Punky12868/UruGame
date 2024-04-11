using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    // PlaceHolder for the EnemyBase
    #region Variables
    public Animator anim;
    public Rigidbody rb;
    public Transform pivot;
    public Transform target;

    public float animClipLength;
    public float animTimer;
    public bool isAnimationDone;

    public bool isSpawning;
    public bool isMoving;
    public bool isAttacking;
    public bool isStunned;
    public bool isOnCooldown;

    public bool avoidingTarget;

    public float health;
    public int damage;
    public float speed;
    public float stunTime;

    public float attackCooldown;

    [ShowIf("avoidTarget", false)] public bool isMelee;
    [ShowIf("isMelee", true)] public float closeAttackRange;
    [ShowIf("isMelee", true)] public float farAttackRange;

    [ShowIf("isMelee", false)] public bool avoidTarget;
    [ShowIf("isMelee", false)][ShowIf("avoidTarget", true)] public float avoidRange;

    public string[] animationIDs;
    AnimationClip[] clips;

    [Header("Debug")]
    [SerializeField] private Transform debugDrawCenter;
    [SerializeField] private Color closeAttackColor;
    [SerializeField] private Color farAttackColor;
    [SerializeField] private Color avoidRangeColor;
    [SerializeField] private int segments = 36; // Number of line segments to approximate the circle
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

    public virtual void FixedUpdate()
    {
        OnCooldown();
        Movement();
        Attack();
        FlipPivot();
    }

    public virtual void Movement()
    {
        if (isStunned || !isAnimationDone || isOnCooldown)
        {
            return;
        }

        if (!isAttacking)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
            isMoving = true;

            PlayAnimation(animationIDs[2], false);
        }
        else
        {
            isMoving = false;
        }
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
        PlayAnimation(animationIDs[6], false);
        RemoveComponentsOnDeath();
    }

    #region Attack Behaviour
    public virtual void Attack()
    {
        if (isStunned || !isAnimationDone || isOnCooldown)
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
    }

    public virtual void MeleeBehaviour()
    {
        // Check if the player is in range, if its  in the close attack range, attack.
        // if its in the far attack range, decide if attack or move to the player. if its out of range, move to the player
        if (Vector3.Distance(target.position, transform.position) <= closeAttackRange)
        {
            // Attack the player
            isAttacking = true;
            PlayAnimation(animationIDs[3], true, true);
        }
        else if (Vector3.Distance(target.position, transform.position) <= farAttackRange)
        {
            // Decide if attack or move to the player
        }
        else
        {
            // Move to the player
            if (isAttacking == true)
            {
                isAttacking = false;
            }
        }
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
    #endregion

    #region Stun
    public virtual void StunEnemy(float time)
    {
        isStunned = true;
        PlayAnimation(animationIDs[1], false);
        Invoke("ResetStun", time);
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
        if (!isOnCooldown)
            return;

        if (!avoidingTarget)
        {
            PlayAnimation(animationIDs[1], true);
        }
        else
        {
            Vector3 direction = -(target.position - transform.position).normalized;
            rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
            isMoving = true;

            PlayAnimation(animationIDs[2], false);
        }
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
        }
        else
        {
            pivot.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void CheckStatus()
    {
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

    void OnDrawGizmos()
    {
        if (debugDrawCenter == null)
            return;

        if (isMelee)
        {
            DrawCloseAttackRange();
            DrawFarAttackRange();
        }
        else if (avoidTarget)
        {
            DrawAvoidRange();
        }
    }
    #endregion
}
