using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using System;

public class AnimationHolder : MonoBehaviour
{
    private Animator animator;
    private AnimationController anim;
    private List<AnimationClip> clips = new List<AnimationClip>();
    private List<AnimationClip> animationIDs = new List<AnimationClip>();

    [SerializeField] private List<AnimationCustomEvents> animations = new List<AnimationCustomEvents>();

    public void Initialize(Animator animator = null)
    {
        if (animator == null) animator = GetComponent<Animator>();
        else this.animator = animator;

        anim = new AnimationController();

        foreach (var clip in animator.runtimeAnimatorController.animationClips) clips.Add(clip);
        foreach (var animation in animations) animationIDs.Add(animation.animClip);

        anim.SetAnimationController(this, animator, clips, animationIDs);
    }

    public Animator GetAnimator() { return animator; }
    public AnimationController GetAnimationController() { return anim; }
    public List<AnimationClip> GetAnimationsIDs() { return animationIDs; }
    public List<AnimationClip> GetAnimationClips() { return clips; }
    public List<AnimationCustomEvents> GetAnimationCustomEvents() { return animations; }
}

[Serializable]
public class AnimationCustomEvents
{
    public AnimationClip animClip;
    public List<EventData> invokableEvents = new List<EventData>();
}

[Serializable]
public class EventData
{
    public float time;
    public UnityEvent events;
}
