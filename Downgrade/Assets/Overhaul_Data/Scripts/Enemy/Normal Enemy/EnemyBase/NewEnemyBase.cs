using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AnimationHolder))]
[RequireComponent(typeof(Rigidbody))]
public class NewEnemyBase : Subject, IAnimController
{
    #region Variables
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
    [SerializeField] protected float attackDamage = 5;
    [SerializeField] protected bool invertSprite;
    [SerializeField] protected bool destroyOnDeath;
    protected float currentHealth;

    [Header("Combat")]
    [SerializeField] protected float attackCooldown = 0.5f;
    [SerializeField] protected float attackKnockback = 5;
    [SerializeField] protected float parryStunTime = 3;
    //[SerializeField] protected float knockbackForce = 1;
    [SerializeField] protected bool canBeParried = true;

    [Header("Ranges")]
    [SerializeField] protected float tooClose = 0.3f;
    [SerializeField] protected float avoidRange = 2;

    [Header("Avoidance")]
    [SerializeField] protected float avoidanceRange = 0.5f;
    [SerializeField] protected float avoidanceSpeed = 7.5f;

    [Header("UI")]
    [SerializeField] protected Slider healthBar;
    [SerializeField] protected Slider healthBarBg;
    [SerializeField] protected float healthBarBgSpeed;
    [SerializeField] protected float onHitAppearSpeed, onHitDisappearSpeed;
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

    #region States

    protected bool isSpawning;
    protected bool isMoving;
    protected bool isRunning;
    protected bool isAttacking;
    protected bool attackHitboxOn;
    protected bool isNormalAttack;
    protected bool isChargedAttack;
    protected bool isStunned;
    protected bool isParried;
    protected bool isDead = false;
    protected bool isSpriteFlipped;
    protected bool isOnCooldown;
    protected bool decidedChargeAttack;
    protected bool chargeAttackedConsidered;
    protected bool avoidingTarget;
    protected bool hasHealthBar = true;

    #endregion
    #endregion

    #region Unity Methods
    protected virtual void Awake() { SetAwake(); }
    protected virtual void Update()
    {
        if (GameManager.Instance.IsGamePaused()) return;
        if (!isAttacking) lastDirection = SetTargetDir();
        direction = SetTargetDir() + SetAvoidanceDir();
        UpdateHealthUI();
        Attack();
    }
    #endregion

    #region Base Logic
    protected virtual void Movement() { }
    protected virtual void Attack() { }
    protected virtual void TakeDamage(float damage, float knockbackForce = 0) { }
    protected virtual void GetParried() { }
    protected virtual void GetStun() { }
    protected virtual void Death() { }
    #endregion

    #region Utility

    #region Direction
    protected Vector3 SetTargetDir()
    {
        Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 currentPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 dir = (targetPos - currentPos).normalized; return dir;
    }
    protected Vector3 SetAvoidanceDir()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, avoidanceRange);
        Collider nearestAvoidable = null;
        float minDistance = float.MaxValue;

        foreach (Collider c in hitColliders)
        {
            if (c.CompareTag("Enemy") && c != GetComponent<Collider>() || c.CompareTag("Wall") || c.CompareTag("Destructible") || c.CompareTag("Limits"))
            {
                float distance = Vector3.Distance(transform.position, c.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestAvoidable = c;
                }
            }
        }

        return -nearestAvoidable.transform.position.normalized;
    }
    protected float DistanceFromTarget() { return Vector3.Distance(target.position, transform.position); }

    public void MoveOnAttack(float value) { rb.velocity = lastDirection * value; }
    #endregion

    #region Health UI
    protected void UpdateHealthUI()
    {
        if (!hasHealthBar) return;
        healthBar.GetComponentInParent<CanvasGroup>().gameObject.transform.localScale = isSpriteFlipped ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
        if (healthBar.value != currentHealth) healthBar.value = currentHealth;
        healthBarBg.value = Mathf.Lerp(healthBarBg.value, currentHealth, Time.deltaTime * healthBarBgSpeed);
    }

    protected void DissapearBar()
    {
        healthBar.GetComponentInParent<CanvasGroup>().DOFade(0, onHitDisappearSpeed).SetUpdate(UpdateType.Normal, true);
    }
    #endregion

    #region Flip
    protected void Flip(bool value)
    {
        if (invertSprite)
        {
            transform.localScale = value ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
            isSpriteFlipped = value;
        }
        else
        {
            transform.localScale = value ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
            isSpriteFlipped = !value;
        }
    }
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
        if (clip == null || clip.Length == 0) { Debug.LogError("No AudioClips set on " + gameObject.name); return; }

        if (clip.Length == 1) { AudioManager.instance.PlayCustomSFX(clip[0], audioSource); return; }

        int random = Random.Range(0, clip.Length);
        AudioManager.instance.PlayCustomSFX(clip[random], audioSource);
    }
    #endregion

    #region Awake Variables
    protected void SetAwake()
    {
        hasHealthBar = SimpleSaveLoad.Instance.LoadData(FileType.Config, "hbar", true);
        SetUI();
        SetAnimHolder();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = health;
        _particleEmission = hitParticleEmission.emission;
        _particleEmission.enabled = false;

        SpawningSequence();
    }
    protected void SetUI()
    {
        if (!hasHealthBar)
        {
            healthBar.GetComponentInParent<CanvasGroup>().alpha = 0;
            healthBar.GetComponentInParent<Canvas>().gameObject.SetActive(false); return;
        }

        healthBar.GetComponentInParent<CanvasGroup>().alpha = 0;

        healthBar.maxValue = health; healthBarBg.maxValue = health;
        healthBar.value = health; healthBarBg.value = health;
    }
    protected void SpawningSequence()
    {
        isSpawning = true;
        PlaySound(spawnSounds);
        PlayAnimation(0, true);

        DowngradeSystem.Instance.SetEnemy(this);
        NotifyEnemyObservers(AllEnemyActions.Spawned);
    }
    #endregion
    #endregion
}
