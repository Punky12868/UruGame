using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnims : PlayerBase
{
    public override void PlayAnimation(string animName)
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

    public override void PlayAnimation(string animName, bool hasExitTime)
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

    public override void PlayAnimation(string animName, bool hasExitTime, bool bypassExitTime)
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
                return;
            }
        }
    }

    public override void ResetAnimClipUpdate()
    {
        if (animClipLength <= 0)
        {
            isAnimationDone = true;
        }
        else
        {
            animClipLength -= Time.deltaTime;
            isAnimationDone = false;
        }
    }

    public override void PlayerAnimations()
    {
        if (direction == Vector3.zero)
        {
            PlayAnimation(animationIDs[0]); // Idle
        }
        else
        {
            if (direction.x < 0 && direction.z == 0)
            {
                PlayAnimation(animationIDs[5]); // WalkLeft
                isFacingRight = false;
            }
            else if (direction.x > 0 && direction.z == 0)
            {
                PlayAnimation(animationIDs[4]); // WalkRight
                isFacingRight = true;
            }
            else if (direction.z > 0 && direction.x == 0)
            {
                PlayAnimation(animationIDs[3]); // WalkUp
            }
            else if (direction.z < 0 && direction.x == 0)
            {
                PlayAnimation(animationIDs[6]); // WalkDown
            }
            else if (direction.x < 0 && direction.z > 0)
            {
                PlayAnimation(animationIDs[2]); // WalkLeftUp
                isFacingRight = false;
            }
            else if (direction.x > 0 && direction.z > 0)
            {
                PlayAnimation(animationIDs[1]); // WalkRightUp
                isFacingRight = true;
            }
            else if (direction.x < 0 && direction.z < 0)
            {
                PlayAnimation(animationIDs[5]); // WalkLeftDown
                isFacingRight = false;
            }
            else if (direction.x > 0 && direction.z < 0)
            {
                PlayAnimation(animationIDs[4]); // WalkRightDown
                isFacingRight = true;
            }
        }

        // if the attack animation is playing, the player can't move, taking the current animation state
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(animationIDs[7]) || anim.GetCurrentAnimatorStateInfo(0).IsName(animationIDs[8]))
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }
    }
}
