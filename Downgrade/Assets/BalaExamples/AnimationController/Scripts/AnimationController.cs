using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AnimationController
{
    private Animator anim;
    private AnimationClip[] clips;
    private string[] animationIDs;
    private float animClipLength;
    public bool isAnimationDone;
    public bool isPlayingChainAnim;

    private List<string> chainedAnimNames;
    private int chainedAnimCount;

    private Action action;
    private float timerDuration;
    private float elapsedTime;
    private bool isRunning;

    public void SetAnimationController(MonoBehaviour script, Animator _anim, AnimationClip[] _clips, string[] _animationIDs)
    {
        script.gameObject.AddComponent<AnimationController_Timer>().SetAnimatorController(this, true);
        anim = _anim;
        clips = _clips;
        animationIDs = _animationIDs;
        isAnimationDone = true;

        Debug.Log("Animation Controller Initialized");
    }

    public void PlayAnimation(string animName = null, List<string> chainedAnimNames = null, bool hasExitTime = false, bool bypassExitTime = false, bool canBeBypassed = false)
    {
        if (bypassExitTime && !canBeBypassed)
            isAnimationDone = true;

        if (!isAnimationDone)
            return;

        if (!isListNullOrCero(chainedAnimNames))
        {
            this.chainedAnimNames = chainedAnimNames;
            chainedAnimCount = this.chainedAnimNames.Count;
            isPlayingChainAnim = true;

            if (animName == null)
                animName = this.chainedAnimNames[0];

            Debug.Log("Chained Animations");
        }
        else if (!isListNullOrCero(this.chainedAnimNames) && isPlayingChainAnim)
        {
            animName = this.chainedAnimNames[0];
            Debug.Log("Chained AnimationsAAAAAAAAAA");
        }

        for (int i = 0; i < animationIDs.Length; i++)
        {
            if (animName == animationIDs[i])
            {
                anim.Play(animName);

                if (hasExitTime || chainedAnimCount > 0)
                {
                    isAnimationDone = false;

                    foreach (AnimationClip clip in clips)
                    {
                        if (clip.name == animName)
                        {
                            animClipLength = clip.length;
                            Debug.Log("Set animation exit time");

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

                                Debug.Log("Play new chained anim: " + this.chainedAnimNames[0]);
                                return;
                            }
                            else
                            {
                                Invoke(animClipLength, () => { isAnimationDone = true; });
                                Debug.Log("Set animation status to done");
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

    private void PlayChainedAnimation()
    {
        if (chainedAnimCount > 0)
            PlayAnimation(chainedAnimNames[0], null, true);
    }

    private bool isListNullOrCero(List<string> value)
    {
        if (value == null)
        {
            return true;
        }
        else if (value.Count <= 0)
        {
            return true;
        }
        return false;
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
}