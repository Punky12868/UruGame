using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AnimationHolder))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyBase : Subject, IAnimController
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
    [SerializeField] protected EnemyBehaviour behaviourType;

    [Header("General")]
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
    [SerializeField] protected AudioClip[] spawnSounds;
    [SerializeField] protected AudioClip[] attackSounds;
    [SerializeField] protected AudioClip[] hitSounds;
    [SerializeField] protected AudioClip[] deathSounds;
    [SerializeField] protected AudioClip[] parriedSounds;
    protected AudioSource audioSource;

    #region States

    protected bool isSpawning;
    protected bool isMoving;
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
    protected float stunnTime;

    #endregion
    #endregion

    #region Unity Methods
    protected virtual void Awake() { SetAwake(); }
    protected virtual void Update()
    {
        if (GameManager.Instance.IsGamePaused() || isDead) return;
        if (!attackHitboxOn) lastDirection = SetTargetDir();
        if (isSpawning || IsAnimationDone()) isSpawning = false;
        if (SetAvoidanceDir() != Vector3.zero) { direction = Vector3.Slerp(direction, SetAvoidanceDir().normalized, Time.deltaTime * avoidanceSpeed); }
        else direction = SetTargetDir();
        OnCooldownTimer();
        FlipFacingLastDir();
        UpdateHealthUI();
        Movement();
        Attack();

        if (isStunned)
        {
            stunnTime += Time.deltaTime;
            if (stunnTime >= 2f) { isStunned = false; stunnTime = 0; }
        }
        else stunnTime = 0;
    }
    #endregion

    #region Base Logic
    protected virtual void Movement() 
    {
        if (isStunned || isParried || !IsAnimationDone()) return;
        if (isOnCooldown) { PlayAnimation(1, false); if (isMoving) { isMoving = false; } return; }

        if (!isAttacking && DistanceFromTarget() < tooCloseRange)
        {
            transform.position += -direction * speed * Time.deltaTime;
            isMoving = true; PlayAnimation(2, false);
        } 
        else if (!isAttacking && DistanceFromTarget() > tooCloseRange)
        {
            transform.position += direction * speed * Time.deltaTime;
            isMoving = true; PlayAnimation(2, false);
        }
        else isMoving = false;
    }

    protected virtual void GetParried() 
    {
        if (canBeParryStunned) { isStunned = true; isParried = true; PlayAnimation(5, true, true); }
        PlaySound(parriedSounds);
        MoveOnAttack(-attackKnockback);
    }

    protected virtual void Death()
    {
        isDead = true;
        PlaySound(deathSounds); PlayAnimation(6, false, true);

        if (FindObjectOfType<WaveSystem>()) FindObjectOfType<WaveSystem>().UpdateDeadEnemies();
        healthBar.GetComponentInParent<CanvasGroup>().DOFade(0, 0.5f);
        Destroy(healthBar.GetComponentInParent<CanvasGroup>().gameObject, 0.499f);
        Destroy(GetComponent<Collider>());
        Invoker.CancelInvoke(DissapearBar);
        //Destroy(audioSource);
        if (destroyOnDeath) { Destroy(gameObject, 0.5f); return; }
        Destroy(this);
    }

    protected virtual void TakeDamage(float damage, float knockbackForce = 0, Vector3 direction = new Vector3()) 
    {
        if (isDead) return;
        currentHealth -= damage;
        if (direction != Vector3.zero) rb.AddForce((transform.position + direction).normalized * knockbackForce, ForceMode.Impulse);
        else rb.AddForce((transform.position - target.position).normalized * knockbackForce, ForceMode.Impulse);
        sr.material.SetFloat("_HitFloat", 1);
        Invoke("HitMaterialReset", 0.2f);
        _particleEmission.enabled = true;
        Invoker.InvokeDelayed(ResetParticle, 0.1f);

        if (hasHealthBar)
        {
            healthBar.GetComponentInParent<CanvasGroup>().DOFade(1, onHitAppearSpeed).SetUpdate(UpdateType.Normal, true);
            //Invoker.CancelInvoke(DissapearBar);
            Invoke("DissapearBar", onHitBarCooldown);
        }

        if (currentHealth <= 0) { Death(); }
        else 
        { 
            PlaySound(hitSounds); ResetStatusOnHit();
            if (hasHitAnimation) { PlayAnimation(4, true, true); }
        }
    }

    protected virtual void Attack() { }
    protected virtual void GetStun() { }
    #endregion

    #region Utility

    protected void ResetStatusOnHit()
    {
        isStunned = true;
        isAttacking = false;
        attackHitboxOn = false;
    }

    protected void OnCooldownEnd()
    {
        isOnCooldown = false;
        isStunned = false;
        cooldown = attackCooldown;
    }

    protected void OnCooldownTimer()
    {
        if (!isOnCooldown) return;
        if (cooldown > 0) cooldown -= Time.deltaTime; else OnCooldownEnd();
    }

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

    #region Health UI
    protected void UpdateHealthUI()
    {
        if (!hasHealthBar) return;
        healthBar.GetComponentInParent<CanvasGroup>().gameObject.transform.localScale = isSpriteFlipped ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        if (healthBar.value != currentHealth) healthBar.value = currentHealth;
        healthBarBg.value = Mathf.Lerp(healthBarBg.value, currentHealth, Time.deltaTime * healthBarBgSpeed);
    }

    protected void DissapearBar() { healthBar.GetComponentInParent<CanvasGroup>().DOFade(0, onHitDisappearSpeed).SetUpdate(UpdateType.Normal, true); }
    #endregion

    public void DoCameraShake() { GameManager.Instance.CameraShake(cameraShakeDuration, cameraShakeMagnitude, cameraShakeGain); }

    #region Flip
    protected void FlipFacingLastDir() 
    {
        if (isStunned || isParried /*|| !IsAnimationDone() || isOnCooldown*/) return;
        Flip(Vector3.Dot(lastDirection, Vector3.right) >= 0); 
    }

    protected void Flip(bool value)
    {
        if (invertSprite) { transform.localScale = value ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1); isSpriteFlipped = value; }
        else { transform.localScale = value ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1); isSpriteFlipped = !value; }
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
        if (NullOrCero.isArrayNullOrCero(clip)) { Debug.LogError("No AudioClips set on " + gameObject.name); return; }

        if (clip.Length == 1) { AudioManager.instance.PlayCustomSFX(clip[0], audioSource); return; }

        int random = Random.Range(0, clip.Length);
        AudioManager.instance.PlayCustomSFX(clip[random], audioSource);
    }
    #endregion

    #region Awake Variables
    protected void SetAwake()
    {
        hasHealthBar = SimpleSaveLoad.Instance.LoadData(FileType.Config, "hbar", true);
        sr = GetComponentInChildren<Animator>().gameObject.GetComponent<SpriteRenderer>();
        SetUI();
        SetAnimHolder();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = health;
        _particleEmission = hitParticleEmission.emission;
        _particleEmission.enabled = false;
        cooldown = attackCooldown;

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
        if(!isPlaceHolder) PlayAnimation(0, true);

        DowngradeSystem.Instance.SetEnemy(this);
        NotifyEnemyObservers(AllEnemyActions.Spawned);
    }
    #endregion

    #region Get - Set - Proxys - Invokes

    #region Get
    public EnemyType GetEnemyType() { return enemyType; }
    public EnemyBehaviour GetBehaviourType() { return behaviourType; }
    public bool GetIsDead() { return isDead; }
    public bool GetIsStunned() { return isStunned; }
    public bool GetIsParried() { return isParried; }
    public bool GetIsMoving() { return isMoving; }
    public bool GetIsAttacking() { return isAttacking; }
    public bool GetIsSpriteFlipped() { return isSpriteFlipped; }
    public bool GetIsOnCooldown() { return isOnCooldown; }
    public bool GetIsNormalAttack() { return isNormalAttack; }
    public bool GetIsChargedAttack() { return isChargedAttack; }
    public bool GetChargeAttackedConsidered() { return chargeAttackedConsidered; }
    public bool GetDecidedChargeAttack() { return decidedChargeAttack; }
    public bool GetAvoidingTarget() { return avoidingTarget; }
    public bool GetCanBeParried() { return canBeParried; }
    public bool GetHasHealthBar() { return hasHealthBar; }
    public float GetCurrentHealth() { return currentHealth; }
    public float GetAttackDamage() { return attackDamage; }
    public float GetAttackKnockback() { return attackKnockback; }
    public float GetParryStunTime() { return parryStunTime; }
    public float GetAvoidanceSpeed() { return avoidanceSpeed; }
    public float GetAvoidanceRange() { return avoidanceRange; }
    public float GetTooClose() { return tooCloseRange; }
    public float GetSpeed() { return speed; }
    public float GetAttackCooldown() { return attackCooldown; }
    public bool GetAttackHitboxOn() { return attackHitboxOn; }
    public bool GetIsUsingRipple() { return usingRipple; }
    public Vector3 GetDirection() { return direction; }
    public Vector3 GetLastDirection() { return lastDirection; }
    public Transform GetTarget() { return target; }

    #endregion

    #region Set
    public void SetIsDead(bool value) { isDead = value; }
    public void SetIsStunned(bool value) { isStunned = value; }
    public void SetIsParried(bool value) { isParried = value; }
    public void SetIsMoving(bool value) { isMoving = value; }
    public void SetIsAttacking(bool value) { isAttacking = value; }
    public void SetIsSpriteFlipped(bool value) { isSpriteFlipped = value; }
    public void SetIsOnCooldown(bool value) { isOnCooldown = value; }
    public void SetIsNormalAttack(bool value) { isNormalAttack = value; }
    public void SetIsChargedAttack(bool value) { isChargedAttack = value; }
    public void SetChargeAttackedConsidered(bool value) { chargeAttackedConsidered = value; }
    public void SetDecidedChargeAttack(bool value) { decidedChargeAttack = value; }
    public void SetAvoidingTarget(bool value) { avoidingTarget = value; }
    public void SetCanBeParried(bool value) { canBeParried = value; }
    public void SetCurrentHealth(float value) { currentHealth = value; }
    public void SetAttackDamage(float value) { attackDamage = value; }
    public void SetAttackKnockback(float value) { attackKnockback = value; }
    public void SetParryStunTime(float value) { parryStunTime = value; }
    public void SetAvoidanceSpeed(float value) { avoidanceSpeed = value; }
    public void SetAvoidanceRange(float value) { avoidanceRange = value; }
    public void SetTooClose(float value) { tooCloseRange = value; }
    public void SetSpeed(float value) { speed = value; }
    public void SetAttackCooldown(float value) { attackCooldown = value; }
    public void SetAttackHitboxOn(bool value) { attackHitboxOn = value; }
    public void SetVectorDirection(Vector3 value) { direction = value; }
    public void SetLastVectorDirection(Vector3 value) { lastDirection = value; }
    public void SetTransformDirection(Transform value) { direction = value.position.normalized; }
    public void SetLastTransformDirection(Transform value) { lastDirection = value.position.normalized; }
    public void SetTarget(Transform value) { target = value; }
    #endregion

    #endregion Proxys
    public void TakeDamageProxy(float damage, float knockbackForce = 0, Vector3 dir = new Vector3()) { TakeDamage(damage, knockbackForce, dir); }
    public void PlaySoundProxy(AudioClip[] clip) { PlaySound(clip); }
    #region

    #region Invokes
    protected void ResetParticle() { _particleEmission.enabled = false; }
    protected void HitMaterialReset() 
    { sr.material.SetFloat("_HitFloat", 0); }
    #endregion

    #endregion
    #endregion

    #region Debug

    [Header("Debug Tools")]
    [SerializeField] protected bool debugTools = true;
    [SerializeField] protected Transform debugDrawCenter;
    [SerializeField] protected int segments = 8;

    protected void DrawAttackHitbox(Vector3 pos, Vector3 size, Color color)
    {
        VisualizeBox.DisplayBox(pos, size, Quaternion.LookRotation(lastDirection), color);
    }

    protected void DrawRange(float range, Color color)
    {
        Vector3 center = debugDrawCenter.position;
        float angleIncrement = 360.0f / segments;

        Vector3 prevPoint = center + new Vector3(range, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleIncrement;
            Vector3 nextPoint = center + new Vector3(range * Mathf.Cos(angle * Mathf.Deg2Rad), 0, range * Mathf.Sin(angle * Mathf.Deg2Rad));

            Debug.DrawLine(prevPoint, nextPoint, color);
            prevPoint = nextPoint;
        }

        Debug.DrawLine(prevPoint, center + new Vector3(range, 0, 0), color);
    }
    #endregion
}
