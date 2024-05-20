using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTest : MonoBehaviour, IAnimController
{
    private Player input;
    private AnimationHolder animHolder;
    private List<AnimationClip> animationIDs;

    private void Awake()
    {
        input = ReInput.players.GetPlayer(0);
        SetAnimHolder();
    }

    private void Update()
    {
        if (input.GetButtonDown("Attack")) animHolder.GetAnimationController().PlayAnimation(animationIDs[1], null, true);
        if (input.GetButtonDown("Parry")) animHolder.GetAnimationController().PlayAnimation(animationIDs[3], null, true);
        if (input.GetButtonDown("Roll")) animHolder.GetAnimationController().PlayAnimation(animationIDs[4], null, true);
        if (animHolder.GetAnimationController().isAnimationDone) animHolder.GetAnimationController().PlayAnimation(animationIDs[0]);

        if (input.GetButtonDown("Use Item"))
        {
            // chained animation, creates the list and then plays the full sequence
            List<AnimationClip> chainedAnimNames = new List<AnimationClip> { animationIDs[1], animationIDs[2], animationIDs[3], animationIDs[4] };
            animHolder.GetAnimationController().PlayAnimation(null, chainedAnimNames, true);
        }
    }

    public void TestEvents(string message) {Debug.Log(message);}

    public void SetAnimHolder()
    {
        animHolder = GetComponent<AnimationHolder>();
        animHolder.Initialize();
        animationIDs = animHolder.GetAnimationsIDs();
    }
}
