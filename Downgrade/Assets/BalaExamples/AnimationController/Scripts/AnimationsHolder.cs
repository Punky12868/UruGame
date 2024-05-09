using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using System;

public class AnimationsHolder : MonoBehaviour
{
    private Animator animator;
    private AnimationController anim;
    private List<AnimationClip> clips = new List<AnimationClip>();
    private List<string> animationIDs = new List<string>();

    [SerializeField] private List<AnimationCustomEvents> animations = new List<AnimationCustomEvents>();

    public void Initialize()
    {
        animator = GetComponent<Animator>();
        anim = new AnimationController();

        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            clips.Add(clip);
        }

        foreach (var animation in animations)
        {
            animationIDs.Add(animation.animationId);
        }

        anim.SetAnimationController(this, animator, clips, animationIDs);
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public AnimationController GetAnimationController()
    {
        return anim;
    }

    public List<string> GetAnimationsIDs()
    {
        return animationIDs;
    }

    public List<AnimationClip> GetAnimationClips()
    {
        return clips;
    }

    public List<AnimationCustomEvents> GetAnimationCustomEvents()
    {
        return animations;
    }
}

[Serializable]
public class AnimationCustomEvents
{
    public string animationId;
    public List<EventData> invokableEvents = new List<EventData>();
}

[Serializable]
public class EventData
{
    public float time;
    public UnityEvent events;
}
