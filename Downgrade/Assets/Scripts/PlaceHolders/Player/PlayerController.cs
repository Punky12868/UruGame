using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System.Diagnostics.Tracing;

public class PlayerController : MonoBehaviour
{
    // PlaceHolder for the PlayerController
    private Player input;
    private Rigidbody rb;
    private Animator anim;

    private Vector2 direction;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;

    [Header("Fighting")]
    [SerializeField] private float attackDamage = 5;

    [Header("Input")]
    [SerializeField] private float directionThreshold = 0.1f;
    [SerializeField] private float comboWindowTime;
    [SerializeField] private Vector2 parryWindowTime;

    [SerializeField] private Vector3 hitboxPos = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 hitboxSize = new Vector3(0.5f, 0.5f, 0.5f);

    [Header("Animation")]
    [SerializeField] private string[] animationIDs;
    private AnimationClip[] clips;
    private bool isAnimationDone = true;
    private float animClipLength;

    [Header("Debug")]
    [SerializeField] private bool debugTools = true;
    [ShowIf("debugTools", true, true)] [SerializeField] private bool drawHitbox = true;
    [ShowIf("debugTools", true, true)] [ShowIf("drawHitbox", true, true)] [SerializeField] private bool drawHitboxOnGameplay = true;
    [ShowIf("debugTools", true, true)] [ShowIf("drawHitbox", true, true)] [SerializeField] private float attackHitboxTime = 0.2f;
    [ShowIf("debugTools", true, true)] [ShowIf("drawHitbox", true, true)] [SerializeField] private Color attackHitboxColor = new Color(1, 0, 0, 1);
    [ShowIf("debugTools", true, true)] [ShowIf("drawHitbox", true, true)] [SerializeField] private Color parryHitboxColor = new Color(0, 1, 0, 1);

    //[Header("PlayerStatus")]
    private bool isFacingRight;

    private bool canCombo = false;
    private bool drawingAttackHitbox = false;

    private bool isParrying = false;
    private bool isRolling = false;

    private bool wasParryPressed = false;
    private bool wasParryInvoked = false;

    private void Awake()
    {
        input = ReInput.players.GetPlayer(0);
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        clips = anim.runtimeAnimatorController.animationClips;
    }

    private void Update()
    {
        Inputs();
        PlayerAnimations();

        if(isParrying)
            OverlapParry();
    }

    private void FixedUpdate()
    {
        if (direction.sqrMagnitude > directionThreshold)
        {
            rb.velocity = new Vector3(direction.x, 0, direction.y) * speed;
        }
    }

    private void Inputs()
    {
        direction = new Vector2(input.GetAxisRaw("Horizontal"), input.GetAxisRaw("Vertical"));

        if (input.GetButtonDown("Attack"))
        {
            OverlapAttack();
        }

        if (input.GetButtonDown("Parry"))
        {
            if (!wasParryPressed)
            {
                wasParryPressed = true;
                Invoke("ActivateParry", parryWindowTime.x);
            }
        }

        if (input.GetButtonDown("Roll"))
        {
        }
    }

    #region Invokes
    private void ActivateParry()
    {
        isParrying = true;
    }

    private void ResetParry()
    {
        isParrying = false;
        wasParryInvoked = false;
        wasParryPressed = false;
    }

    private void DrawingAttackHitbox()
    {
        drawingAttackHitbox = false;
    }

    private void ResetCombo()
    {
        canCombo = false;
    }
    #endregion

    #region Animation
    private void PlayerAnimations()
    {
        if (direction == Vector2.zero)
        {
            PlayAnimation(animationIDs[0]); // Idle
        }
        else
        {
            if (direction.x < 0 && direction.y == 0)
            {
                PlayAnimation(animationIDs[5]); // WalkLeft
                isFacingRight = false;
            }
            else if (direction.x > 0 && direction.y == 0)
            {
                PlayAnimation(animationIDs[4]); // WalkRight
                isFacingRight = true;
            }
            else if (direction.y > 0 && direction.x == 0)
            {
                PlayAnimation(animationIDs[3]); // WalkUp
            }
            else if (direction.y < 0 && direction.x == 0)
            {
                PlayAnimation(animationIDs[6]); // WalkDown
            }
            else if (direction.x < 0 && direction.y > 0)
            {
                PlayAnimation(animationIDs[2]); // WalkLeftUp
                isFacingRight = false;
            }
            else if (direction.x > 0 && direction.y > 0)
            {
                PlayAnimation(animationIDs[1]); // WalkRightUp
                isFacingRight = true;
            }
            else if (direction.x < 0 && direction.y < 0)
            {
                PlayAnimation(animationIDs[5]); // WalkLeftDown
                isFacingRight = false;
            }
            else if (direction.x > 0 && direction.y < 0)
            {
                PlayAnimation(animationIDs[4]); // WalkRightDown
                isFacingRight = true;
            }
        }
    }

    #region AnimationController
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

    public void ResetAnimClipLenght()
    {
        isAnimationDone = true;
        animClipLength = 0;
    }
    #endregion

    #endregion

    #region Overlap Hitbox
    private void OverlapAttack()
    {
        
        if (!isAnimationDone)
        {
            return;
        }

        if (!drawingAttackHitbox)
        {
            drawingAttackHitbox = true;
            Invoke("DrawingAttackHitbox", attackHitboxTime);
        }


        if (!canCombo)
        {
            PlayAnimation(animationIDs[7], true); // Attack
            canCombo = true;
            Invoke("ResetCombo", comboWindowTime);
            Debug.Log("Attack");
        }
        else
        {
            PlayAnimation(animationIDs[8], true); // Attack
            Debug.Log("Combo");
        }

        Vector3 temp = hitboxPos;
        if (isFacingRight)
        {
            temp.x *= -1;
        }

        Collider[] hitColliders = Physics.OverlapBox(transform.position + transform.TransformDirection(temp), hitboxSize, Quaternion.identity);

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log("Hit");
                //hit.GetComponent<EnemyBase>().TakeDamage(attackDamage);
            }
        }
    }

    private void OverlapParry()
    {
        if (!wasParryInvoked)
        {
            wasParryInvoked = true;
            Invoke("ResetParry", parryWindowTime.y);
        }

        Vector3 temp = hitboxPos;
        if (isFacingRight)
        {
            temp.x *= -1;
        }

        Collider[] hitColliders = Physics.OverlapBox(transform.position + transform.TransformDirection(temp), hitboxSize, Quaternion.identity);

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log("Parry");
                //hit.GetComponent<EnemyBase>().StunEnemy(1);
            }
        }

        PlayAnimation(animationIDs[9], true); // Parry
    }
    #endregion

    #region Debug
    private void DrawAttackHitbox()
    {
        Vector3 temp = hitboxPos;
        if (isFacingRight)
        {
            temp.x *= -1;
        }

        Gizmos.color = attackHitboxColor;
        Gizmos.DrawWireCube(transform.position + transform.TransformDirection(temp), hitboxSize);
    }

    private void DrawParryHitbox()
    {
        Vector3 temp = hitboxPos;
        if (isFacingRight)
        {
            temp.x *= -1;
        }

        Gizmos.color = parryHitboxColor;
        Gizmos.DrawWireCube(transform.position + transform.TransformDirection(temp), hitboxSize);
    }

    private void OnDrawGizmos()
    {
        if (drawHitbox)
        {
            if (drawHitboxOnGameplay)
            {
                if (drawingAttackHitbox)
                    DrawAttackHitbox();

                if (isParrying)
                    DrawParryHitbox();
            }
            else
            {
                DrawAttackHitbox();
                DrawParryHitbox();
            }
        }
    }
    #endregion
}
