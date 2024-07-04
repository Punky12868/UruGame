using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AnimationController
{
    private AnimationController_Timer timer;
    private AnimationHolder animHolder;
    private Animator anim;
    private List<AnimationClip> clips;
    private List<AnimationClip> animationIDs;
    private float animClipLength;
    public bool isAnimationDone;
    public bool isPlayingChainAnim;

    private List<AnimationClip> chainedAnimNames;
    private int chainedAnimCount;

    private Action action;
    private float timerDuration;
    private float elapsedTime;
    private bool isRunning;

    public void SetAnimationController(MonoBehaviour script, Animator _anim, List<AnimationClip> _clips, List<AnimationClip> _animationIDs)
    {
        script.gameObject.AddComponent<AnimationController_Timer>().SetAnimatorController(this);
        timer = script.GetComponent<AnimationController_Timer>();
        timer.SetTimeSettings(true);

        animHolder = script.GetComponent<AnimationHolder>();

        anim = _anim;
        clips = _clips;
        animationIDs = _animationIDs;
        isAnimationDone = true;
    }

    public void PlayAnimation(AnimationClip animClip = null, List<AnimationClip> chainedAnimNames = null, bool hasExitTime = false, bool bypassExitTime = false, bool canBeBypassed = false)
    {
        if (bypassExitTime && !canBeBypassed) isAnimationDone = true;
        if (!isAnimationDone) return;

        if (!NullOrCero.isListNullOrCero(chainedAnimNames))
        {
            this.chainedAnimNames = chainedAnimNames;
            chainedAnimCount = this.chainedAnimNames.Count;
            isPlayingChainAnim = true;

            if (animClip == null)
                animClip = this.chainedAnimNames[0];
        }
        else if (!NullOrCero.isListNullOrCero(this.chainedAnimNames) && isPlayingChainAnim)
        {
            animClip = this.chainedAnimNames[0];
        }

        for (int i = 0; i < animationIDs.Count; i++)
        {
            if (animClip == animationIDs[i])
            {
                if (hasExitTime) anim.Play(animClip.name, 0, 0);
                else anim.Play(animClip.name);

                /*if (!GetCurrentAnimationName(animClip.name)) anim.Play(animClip.name);
                else anim.Play(animClip.name, 0, 0);*/

                InvokeEvents(animationIDs[i]);

                if (hasExitTime || chainedAnimCount > 0)
                {
                    isAnimationDone = false;

                    foreach (AnimationClip clip in clips)
                    {
                        if (clip == animClip)
                        {
                            animClipLength = clip.length;

                            if (chainedAnimCount > 0)
                            {
                                float nextAnimDelay = 0.1f;
                                Invoke(animClipLength + nextAnimDelay, () =>
                                {
                                    if (this.chainedAnimNames.Count > 0)
                                    {
                                        this.chainedAnimNames.RemoveAt(0);
                                        chainedAnimCount--;
                                        PlayChainedAnimation();
                                    }
                                });
                                return;
                            }
                            else
                            {
                                Invoke(animClipLength, () => { isAnimationDone = true;});
                                isPlayingChainAnim = false;
                                return;
                            }
                        }
                    }
                }
                return;
            }
        }
    }

    private void InvokeEvents(AnimationClip animName)
    {
        foreach (var animation in animHolder.GetAnimationCustomEvents())
        {
            if (animation.animClip == animName)
            {
                foreach (var eventData in animation.invokableEvents)
                {
                    Invoker.InvokeDelayed(eventData.events.Invoke, eventData.time);
                }
            }
        }
    }

    public void UsingDeltaTime(bool useDeltaTime) { timer.SetTimeSettings(useDeltaTime); }
    private void PlayChainedAnimation() { if (chainedAnimCount > 0) PlayAnimation(chainedAnimNames[0], null, true); }

    private bool GetCurrentAnimationName(string clipName)
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(clipName);
    }

    public void Invoke(float delay, Action actionToInvoke)
    {
        this.action = actionToInvoke;
        this.timerDuration = delay;
        this.elapsedTime = 0f;
        this.isRunning = true;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (isRunning)
        {
            elapsedTime += deltaTime;
            if (elapsedTime >= timerDuration)
            {
                action?.Invoke();

                isAnimationDone = true;
                isRunning = false;
            }
        }
    }

    public float GetElapsedTime() { return elapsedTime; }
}