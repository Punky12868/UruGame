using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Bruja : Subject, IAnimController
{
    #region Variables
    protected AnimationHolder animHolder;

    protected Rigidbody rb;
    protected SpriteRenderer sr;
    protected List<AnimationClip> animationIDs;

    protected Transform target;
    protected Vector3 direction, lastDirection;

    [Header("Type")]
    [SerializeField] protected EnemyType enemyType;

    [Header("General")]
    [SerializeField] protected string bossName;
    [SerializeField] protected float health = 100;
    [SerializeField] protected float speed = 1;
    [SerializeField] protected float attackDamage = 5;
    [SerializeField] protected bool hasHitAnimation = true;
    [SerializeField] protected bool canBeParryStunned = true;
    [SerializeField] protected bool invertSprite = false;
    [SerializeField] protected bool destroyOnDeath = false;
    [SerializeField] protected bool isPlaceHolder = false;
    protected float currentHealth;

    [Header("Combat")]
    [SerializeField] protected float attackCooldown = 1.5f;
    [SerializeField] protected float attackKnockback = 10;
    [SerializeField] protected float parryStunTime = 3;
    //[SerializeField] protected float knockbackForce = 1;
    [SerializeField] protected bool canBeParried = true;
    protected float cooldown;

    [Header("CameraEffects")]
    [SerializeField] protected float cameraShakeDuration = 0.2f;
    [SerializeField] protected float cameraShakeMagnitude = 0.5f;
    [SerializeField] protected float cameraShakeGain = 0.5f;

    [Header("Ranges")]
    [SerializeField] protected float tooCloseRange = 0.3f;
    //[SerializeField] protected float avoidanceRange = 2;

    [Header("Avoidance")]
    [SerializeField] protected float avoidanceRange = 0.65f;
    [SerializeField] protected float avoidanceSpeed = 7.5f;

    [Header("UI")]
    [SerializeField] protected Slider healthBar;
    [SerializeField] protected Slider healthBarBg;
    [SerializeField] protected float healthBarBgSpeed = 5;
    [SerializeField] protected float onHitAppearSpeed = 1;
    [SerializeField] protected float onHitDisappearSpeed = 5;
    [SerializeField] protected float onHitBarCooldown = 5;

    [Header("Particles")]
    [SerializeField] protected ParticleSystem hitParticleEmission;
    [SerializeField] protected bool usingRipple = false;
    [SerializeField] protected GameObject ripplePrefab;
    protected ParticleSystem.EmissionModule _particleEmission;

    [Header("AudioClips")]
    [SerializeField] protected AudioClip[] placeHolderSounds;
    protected AudioSource audioSource;

    private bool isStarting;
    private bool isStartingMoving;
    private bool started;
    private bool isOnSpecial;
    private bool isMoving;
    private bool isSummoning;
    private bool isAttacking;

    #endregion

    #region Utility

    #region Direction
    protected Vector3 SetTargetDir()
    {
        Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 currentPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 dir = (targetPos - currentPos).normalized;
        Vector3 fixedDir = new Vector3(dir.x, 0, dir.z); return fixedDir;
    }
    protected Vector3 SetAvoidanceDir()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, avoidanceRange);
        Collider nearestAvoidable = null;
        float minDistance = float.MaxValue;

        foreach (Collider c in hitColliders)
        {
            if (c.CompareTag("Enemy") && c != GetComponent<Collider>() || c.CompareTag("Wall") || c.CompareTag("Destructible") || c.CompareTag("Limits") ||
                c.CompareTag("Prop"))
            {
                float distance = Vector3.Distance(transform.position, c.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestAvoidable = c;
                }
            }
        }
        if (nearestAvoidable == null) return Vector3.zero;
        Vector3 avoidDir = (transform.position - nearestAvoidable.transform.position).normalized;
        Vector3 fixedDir = new Vector3(avoidDir.x, 0, avoidDir.z);

        if (nearestAvoidable.CompareTag("Wall") || nearestAvoidable.CompareTag("Limits")) return -fixedDir;
        return fixedDir;
    }
    protected float DistanceFromTarget() { return Vector3.Distance(target.position, transform.position); }

    public void MoveOnAttack(float value) { rb.velocity = lastDirection * value; }
    #endregion

    #region AnimationController
    public void SetAnimHolder()
    {
        animHolder = GetComponent<AnimationHolder>();
        animHolder.Initialize(GetComponentInChildren<Animator>());
        animationIDs = animHolder.GetAnimationsIDs();
    }

    protected void PlayAnimation(int index, bool hasExitTime = false, bool bypassExitTime = false, bool canBeBypassed = false)
    {
        animHolder.GetAnimationController().PlayAnimation(animationIDs[index], null, hasExitTime, bypassExitTime, canBeBypassed);
    }

    protected bool IsAnimationDone()
    {
        return animHolder.GetAnimationController().isAnimationDone;
    }
    #endregion

    #region Sounds
    protected void PlaySound(AudioClip[] clip)
    {
        if (NullOrCero.isArrayNullOrCero(clip)) { Debug.LogError("No AudioClips set on " + gameObject.name); return; }

        if (clip.Length == 1) { AudioManager.instance.PlayCustomSFX(clip[0], audioSource); return; }

        int random = Random.Range(0, clip.Length);
        AudioManager.instance.PlayCustomSFX(clip[random], audioSource);
    }
    #endregion

    #endregion
}
