using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AnimationController
{
    private AnimationController_Timer timer;
    private AnimationsHolder animHolder;
    private Animator anim;
    private List<AnimationClip> clips;
    private List<string> animationIDs;
    private float animClipLength;
    public bool isAnimationDone;
    public bool isPlayingChainAnim;

    private List<string> chainedAnimNames;
    private int chainedAnimCount;

    private Action action;
    private float timerDuration;
    private float elapsedTime;
    private bool isRunning;

    public void SetAnimationController(MonoBehaviour script, Animator _anim, List<AnimationClip> _clips, List<string> _animationIDs)
    {
        script.gameObject.AddComponent<AnimationController_Timer>().SetAnimatorController(this);
        timer = script.GetComponent<AnimationController_Timer>();
        timer.SetTimeSettings(true);

        animHolder = script.GetComponent<AnimationsHolder>();

        anim = _anim;
        clips = _clips;
        animationIDs = _animationIDs;
        isAnimationDone = true;
    }

    public void PlayAnimation(string animName = null, List<string> chainedAnimNames = null, bool hasExitTime = false, bool bypassExitTime = false, bool canBeBypassed = false)
    {
        if (bypassExitTime && !canBeBypassed)
            isAnimationDone = true;

        if (!isAnimationDone)
            return;

        if (!NullOrCero.isListNullOrCero(chainedAnimNames))
        {
            this.chainedAnimNames = chainedAnimNames;
            chainedAnimCount = this.chainedAnimNames.Count;
            isPlayingChainAnim = true;

            if (animName == null)
                animName = this.chainedAnimNames[0];
        }
        else if (!NullOrCero.isListNullOrCero(this.chainedAnimNames) && isPlayingChainAnim)
        {
            animName = this.chainedAnimNames[0];
        }

        for (int i = 0; i < animationIDs.Count; i++)
        {
            if (animName == animationIDs[i])
            {
                anim.Play(animName);

                InvokeEvents(animName);

                if (hasExitTime || chainedAnimCount > 0)
                {
                    isAnimationDone = false;

                    foreach (AnimationClip clip in clips)
                    {
                        if (clip.name == animName)
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
                                Invoke(animClipLength, () => { isAnimationDone = true; });
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

    private void InvokeEvents(string animName)
    {
        foreach (var animation in animHolder.GetAnimationCustomEvents())
        {
            if (animation.animationId == animName)
            {
                foreach (var eventData in animation.invokableEvents)
                {
                    Invoker.InvokeDelayed(eventData.events.Invoke, eventData.time);
                    return;
                }
            }
        }
    }

    public void UsingDeltaTime(bool useDeltaTime)
    {
        timer.SetTimeSettings(useDeltaTime);
    }

    private void PlayChainedAnimation()
    {
        if (chainedAnimCount > 0)
            PlayAnimation(chainedAnimNames[0], null, true);
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

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}