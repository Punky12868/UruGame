using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(AnimationHolder))]
[RequireComponent(typeof(Rigidbody))]
public class NewEnemyBase : Subject, IAnimController
{
    protected AnimationHolder animHolder;

    protected Rigidbody rb;
    protected List<AnimationClip> animationIDs;

    protected Transform target;
    protected Vector3 direction, lastDirection;

    [Header("Type")]
    [SerializeField] protected EnemyType enemyType;
    [SerializeField] protected EnemyBehaviour behaviourType;

    [Header("General")]
    [SerializeField] protected float health = 100;
    [SerializeField] protected float speed = 1;
    [SerializeField] protected float attackCooldown = 0.5f;
    [SerializeField] protected float knockbackForce = 1;
    [SerializeField] protected float attackDamage = 5;
    protected float currentHealth;

    [Header("Combat")]
    [SerializeField] protected float attackKnockback = 5;
    [SerializeField] protected float parryStunTime = 3;

    [Header("Ranges")]
    [SerializeField] protected float tooClose = 0.3f;

    [Header("Avoidance")]
    [SerializeField] protected float avoidanceRange = 0.5f;
    [SerializeField] protected float avoidanceSpeed = 7.5f;

    [Header("UI")]
    [SerializeField] protected Slider healthBar;
    [SerializeField] protected Slider healthBarBg;
    [SerializeField] protected float healthBarBgSpeed;
    [SerializeField] protected float onHitAppearSpeed;
    [SerializeField] protected float onHitDisappearSpeed;
    [SerializeField] protected float onHitBarCooldown;

    [Header("Particles")]
    [SerializeField] protected ParticleSystem hitParticleEmission;
    protected ParticleSystem.EmissionModule _particleEmission;

    [Header("AudioClips")]
    [SerializeField] protected AudioClip[] spawnSounds;
    [SerializeField] protected AudioClip[] attackSounds;
    [SerializeField] protected AudioClip[] hitSounds;
    [SerializeField] protected AudioClip[] deathSounds;
    [SerializeField] protected AudioClip[] parriedSounds;
    protected AudioSource audioSource;

    #region Utility

    #region AnimationController
    public void SetAnimHolder()
    {
        animHolder = GetComponent<AnimationHolder>();
        animHolder.Initialize();
        animationIDs = animHolder.GetAnimationsIDs();
    }

    private void PlayAnimation(int index, bool hasExitTime = false, bool bypassExitTime = false, bool canBeBypassed = false)
    {
        animHolder.GetAnimationController().PlayAnimation(animationIDs[index], null, hasExitTime, bypassExitTime, canBeBypassed);
    }

    private bool IsAnimationDone()
    {
        return animHolder.GetAnimationController().isAnimationDone;
    }
    #endregion

    #region Sounds
    public void PlaySound(AudioClip[] clip)
    {
        if (clip == null || clip.Length == 0) { Debug.LogError("No AudioClips set on " + gameObject.name); return; }

        if (clip.Length == 1) { AudioManager.instance.PlayCustomSFX(clip[0], audioSource); return; }

        int random = Random.Range(0, clip.Length);
        AudioManager.instance.PlayCustomSFX(clip[random], audioSource);
    }
    #endregion

    #endregion
}
