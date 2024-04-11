using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour
{
    // PlaceHolder for the PlayerController
    private Player player;
    [SerializeField] float speed = 5f;
    [SerializeField] float CooldownTime = 0.5f;
    [SerializeField] float attackForce = 1f;
    [SerializeField] float rollForce = 2f;
    Rigidbody rb;

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
            Parrying();
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
    }

    private void Parrying()
    {
        /*PlayAnimation(animationIDs[13], true);
        rb.velocity = Vector3.zero;*/
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
                    break;
                case "UpLeft":
                    PlayAnimation(animationIDs[2]);
                    break;
                case "Down":
                    PlayAnimation(animationIDs[3]);
                    break;
                case "DownRight":
                    PlayAnimation(animationIDs[4]);
                    break;
                case "DownLeft":
                    PlayAnimation(animationIDs[5]);
                    break;
                case "Right":
                    PlayAnimation(animationIDs[6]);
                    break;
                case "Left":
                    PlayAnimation(animationIDs[7]);
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
                    break;
                case "UpLeft":
                    PlayAnimation(animationIDs[8]);
                    break;
                case "Down":
                    PlayAnimation(animationIDs[9]);
                    break;
                case "DownRight":
                    PlayAnimation(animationIDs[10]);
                    break;
                case "DownLeft":
                    PlayAnimation(animationIDs[11]);
                    break;
                case "Right":
                    PlayAnimation(animationIDs[10]);
                    break;
                case "Left":
                    PlayAnimation(animationIDs[11]);
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
}
