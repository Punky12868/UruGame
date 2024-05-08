using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerAnimationTest : MonoBehaviour
{
    private Player input;
    private Animator animator;
    private AnimationController anim;

    [Header("Animation")]
    [SerializeField] private string[] animationIDs;
    private AnimationClip[] clips;
    private void Awake()
    {
        input = ReInput.players.GetPlayer(0);
        animator = GetComponent<Animator>();
        clips = animator.runtimeAnimatorController.animationClips;

        anim = new AnimationController();
        anim.SetAnimationController(this, animator, clips, animationIDs);
    }

    private void Update()
    {
        if (input.GetButtonDown("Attack"))
        {
            anim.PlayAnimation(animationIDs[1], null, true);
        }

        if (input.GetButtonDown("Parry"))
        {
            anim.PlayAnimation(animationIDs[3], null, true);
        }

        if (input.GetButtonDown("Roll"))
        {
            anim.PlayAnimation(animationIDs[4], null, true);
        }

        if (input.GetButtonDown("Use Item"))
        {
            // chained animation, creates the list and then plays the full sequence
            List<string> chainedAnimNames = new List<string> { animationIDs[1], animationIDs[2], animationIDs[3], animationIDs[4] };
            anim.PlayAnimation(null, chainedAnimNames, true);
        }

        if (anim.isAnimationDone)
        {
            anim.PlayAnimation(animationIDs[0]);
        }
    }
}
