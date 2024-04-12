using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour
{
    // PlaceHolder for the PlayerController
    private Player player;
    Rigidbody rb;

    [SerializeField] float speed = 5f;

    [SerializeField] float resetAttackTime = 0.5f;
    [SerializeField] float resetAttackTimeAfterCombo = 0.5f;
    [SerializeField] Vector2 parryTimeWindow;
    [SerializeField] float attackForce = 1f;
    [SerializeField] float rollForce = 2f;

    private bool isFacingRight;
    private bool isAttacking;
    private bool usedComboAttack;
    private bool isParrying;
    private bool isDoingParry;
    private bool isParryingInvoked;

    private bool drawAttackHitbox = true;
    private bool drawParryHitbox = true;

    [SerializeField] Transform hitboxCenter;
    [SerializeField] Vector2 attackHitboxAppearTime = new Vector2(0.2f, 0.5f);
    [SerializeField] Vector3 attackHitboxPos = new Vector3(0, 0, 0);
    [SerializeField] Vector3 attackHitboxSize = new Vector3(0.5f, 0.5f, 0.5f);

    [SerializeField] Vector2 parryHitboxAppearTime = new Vector2(0.2f, 0.5f);
    [SerializeField] Vector3 parryHitboxPos = new Vector3(0, 0, 0);
    [SerializeField] Vector3 parryHitboxSize = new Vector3(0.5f, 0.5f, 0.5f);

    private string status;

    [Header("Animation")]
    Animator anim;

    bool isAnimationDone = true;
    float animClipLength;

    public string[] animationIDs;
    string lastSavedDirection;
    AnimationClip[] clips;

    private void Awake()
    {
        player = ReInput.players.GetPlayer(0);
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        clips = anim.runtimeAnimatorController.animationClips;
    }

    private void FixedUpdate()
    {
        if (!isAnimationDone)
        {
            return;
        }

        Vector2 direction = InputController.instance.GetDirection().normalized;
        rb.velocity = new Vector3(direction.x, 0, direction.y) * speed;
    }

    private void Update()
    {
        Actions();
        MovementAnimations();

        if (isParryingInvoked && isParrying)
            DoParryOverlapCollider(parryHitboxAppearTime.y);

        drawAttackHitbox = isAttacking;
        drawParryHitbox = isParrying;
    }

    private void Actions()
    {
        if (!isAnimationDone)
        {
            return;
        }

        if (player.GetButtonDown("Attack"))
        {
            Attacking();
        }

        if (player.GetButtonDown("Parry"))
        {
            if (!isParryingInvoked)
            {
                isParryingInvoked = true;
                Invoke("Parrying", parryHitboxAppearTime.x);
            }
        }

        if (player.GetButtonDown("Roll"))
        {
            Rolling();
        }

        if (player.GetButtonDown("Ability"))
        {
            Ability();
        }
    }

    private void Attacking()
    {
        PlayAnimation(animationIDs[12], true);
        rb.velocity = Vector3.zero;
        Vector2 direction = InputController.instance.GetLastDirection().normalized;
        rb.velocity = new Vector3(direction.x, 0, direction.y) * attackForce;

        if (!isAttacking)
        {
            DoAttackOverlapCollider(resetAttackTime);
        }
        else if (!usedComboAttack)
        {
            DoAttackOverlapCollider(resetAttackTimeAfterCombo);
        }
    }

    private void Parrying()
    {
        /*PlayAnimation(animationIDs[13], true);
        rb.velocity = Vector3.zero;*/

        isParrying = true;
    }

    private void Rolling()
    {
        /*PlayAnimation(animationIDs[14], true);
        rb.velocity = Vector3.zero;
        Vector2 direction = InputController.instance.GetLastDirection().normalized;
        rb.velocity = new Vector3(direction.x, 0, direction.y) * rollForce;*/
    }

    private void Ability()
    {
        /*PlayAnimation(animationIDs[15], true);
        rb.velocity = Vector3.zero;*/
    }

    #region Hitboxes
    public void DoAttackOverlapCollider(float invokeTime)
    {
        if (!isAttacking)
        {
            isAttacking = true;
            Invoke("ResetAttack", invokeTime);
        }
        else if (!usedComboAttack)
        {
            isAttacking = true;
            usedComboAttack = true;
            Invoke("ResetCombo", invokeTime);
        }

        Vector3 temp = attackHitboxPos;
        if (isFacingRight)
        {
            temp.x *= -1;
        }

        Collider[] hitColliders = Physics.OverlapBox(transform.position + hitboxCenter.TransformDirection(temp), attackHitboxSize / 2, Quaternion.identity);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                Debug.Log("Hit enemy");
            }
        }
    }

    public void ResetAttack()
    {
        if (usedComboAttack)
            return;

        isAttacking = false;
    }

    public void ResetCombo()
    {
        isAttacking = false;
        usedComboAttack = false;
    }

    public void DoParryOverlapCollider(float invokeResetTime)
    {
        if (!isDoingParry)
        {
            isDoingParry = true;
            Invoke("ResetParry", invokeResetTime);
        }

        Vector3 temp = attackHitboxPos;
        if (isFacingRight)
        {
            temp.x *= -1;
        }

        Collider[] hitColliders = Physics.OverlapBox(transform.position + hitboxCenter.TransformDirection(temp), attackHitboxSize / 2, Quaternion.identity);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                Debug.Log("Hit enemy");
            }
        }
    }

    private void ResetParry()
    {
        isParrying = false;
        isDoingParry = false;
        isParryingInvoked = false;
    }

    public string PlayerStatus()
    {
        return status;
    }
    #endregion

    #region Animation
    public void MovementAnimations()
    {
        if (InputController.instance.GetDirection() != Vector2.zero)
        {
            switch (InputController.instance.GetDirectionString())
            {
                case "Up":
                    PlayAnimation(animationIDs[0]);
                    break;
                case "UpRight":
                    PlayAnimation(animationIDs[1]);
                    isFacingRight = true;
                    break;
                case "UpLeft":
                    PlayAnimation(animationIDs[2]);
                    isFacingRight = false;
                    break;
                case "Down":
                    PlayAnimation(animationIDs[3]);
                    break;
                case "DownRight":
                    PlayAnimation(animationIDs[4]);
                    isFacingRight = true;
                    break;
                case "DownLeft":
                    PlayAnimation(animationIDs[5]);
                    isFacingRight = false;
                    break;
                case "Right":
                    PlayAnimation(animationIDs[6]);
                    isFacingRight = true;
                    break;
                case "Left":
                    PlayAnimation(animationIDs[7]);
                    isFacingRight = false;
                    break;
                default:
                    break;
            }

            lastSavedDirection = InputController.instance.GetDirectionString();
        }
        else
        {
            switch (lastSavedDirection)
            {
                case "Up":
                    PlayAnimation(animationIDs[8]);
                    break;
                case "UpRight":
                    PlayAnimation(animationIDs[8]);
                    isFacingRight = true;
                    break;
                case "UpLeft":
                    PlayAnimation(animationIDs[8]);
                    isFacingRight = false;
                    break;
                case "Down":
                    PlayAnimation(animationIDs[9]);
                    break;
                case "DownRight":
                    PlayAnimation(animationIDs[10]);
                    isFacingRight = true;
                    break;
                case "DownLeft":
                    PlayAnimation(animationIDs[11]);
                    isFacingRight = false;
                    break;
                case "Right":
                    PlayAnimation(animationIDs[10]);
                    isFacingRight = true;
                    break;
                case "Left":
                    PlayAnimation(animationIDs[11]);
                    isFacingRight = false;
                    break;
                default:
                    break;
            }
        }
    }

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

    #region Debug
    public void DrawAttackHitbox()
    {
        Vector3 normalTemp = attackHitboxPos;
        if (isFacingRight)
        {
            normalTemp.x *= -1;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + hitboxCenter.TransformDirection(normalTemp), attackHitboxSize);
    }

    public void DrawParryHitbox()
    {
        Vector3 normalTemp = parryHitboxPos;
        if (isFacingRight)
        {
            normalTemp.x *= -1;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + hitboxCenter.TransformDirection(normalTemp), parryHitboxSize);
    }

    void OnDrawGizmos()
    {
        if (drawParryHitbox)
            DrawParryHitbox();
        
        if (drawAttackHitbox)
            DrawAttackHitbox();
    }
    #endregion
}
