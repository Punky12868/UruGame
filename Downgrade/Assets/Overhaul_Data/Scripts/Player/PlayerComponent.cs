using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(PlayerInteraction))]
[RequireComponent(typeof(PlayerUI))]

public class PlayerComponent : Subject
{
    #region Variables
    [HideInInspector] public Player input;
    [HideInInspector] public PlayerInteraction interactions;
    [HideInInspector] public PlayerInventory inventory;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public ParticleSystem.EmissionModule _parryParticleEmission;
    [HideInInspector] public ParticleSystem.EmissionModule _hitParticleEmission;
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public Vector3 lastDirection;

    [Header("Misc")]
    public float health = 100;
    public float healthBigEnemyReward = 10;
    public float healthBossReward = 45;
    public float speed = 5f;
    public float stamina = 100;
    public float staminaCooldown = 1.5f;
    public float staminaRegenSpeed = 5;
    public float staminaNormalReward = 10;
    public float staminaBigEnemyReward = 20;
    public float staminaBossReward = 50;
    public float staminaUsageRoll;
    public float staminaUsageAttack;
    public AnimationCurve rollSpeed;
    public float moveDuration = 0;
    public float rollInmunity = 1f;
    public float rollCooldown = 1f;
    public ParticleSystem parryParticleEmission;
    public ParticleSystem hitParticleEmission;

    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentStamina;
    [HideInInspector] public float staminaTimer;

    [Header("Fighting")]
    public Transform hitboxCenter;
    public float offset = 0.3f;
    public Vector2 attackDamage = new Vector2(3, 5);
    public float cooldownTime = 0.5f;
    public float comboCooldownTime = 0.5f;
    public bool canBeStaggered = false;
    public float damagedCooldown = 0.35f;
    public float attackForce;

    public float vfxSpeed = 1;
    public GameObject normalSlashVFX;
    public GameObject comboSlashVFX;
    [HideInInspector] public float normalVfxTime;
    [HideInInspector] public float comboVfxTime;
    [HideInInspector] public bool isNormalVFXPlaying = false;
    [HideInInspector] public bool isComboVFXPlaying = false;

    [HideInInspector] public float comboTime;

    [Header("Input")]
    public float directionThreshold = 0.1f;
    public Vector2 comboWindowTime;
    public Vector2 parryWindowTime;

    public Vector3 hitboxPos = new Vector3(0, 0, 0);
    public Vector3 hitboxSize = new Vector3(0.5f, 0.5f, 0.5f);

    [Header("Animation")]
    public string[] animationIDs;
    [HideInInspector] public AnimationClip[] clips;
    [HideInInspector] public bool isAnimationDone = true;
    [HideInInspector] public float animClipLength;

    [Header("AudioClips")]
    public AudioClip[] attackClips;
    public AudioClip[] rollClips;
    public AudioClip[] hitClips;
    public AudioClip[] deathClips;
    [HideInInspector] public AudioSource audioSource;

    [Header("Debug")]
    public bool debugTools = true;
    public bool drawHitbox = true;
    public bool drawHitboxOnGameplay = true;
    public float attackHitboxTime = 0.2f;
    public Color attackHitboxColor = new Color(1, 0, 0, 1);
    public Color parryHitboxColor = new Color(0, 1, 0, 1);

    //[Header("PlayerStatus")]
    [HideInInspector] public bool isFacingLeft;

    [HideInInspector] public bool isOnCooldown = false;
    [HideInInspector] public bool canCombo = false;
    [HideInInspector] public bool drawingAttackHitbox = false;

    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool canBeDamaged = true;
    [HideInInspector] public bool isParrying = false;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isRolling = false;

    [HideInInspector] public bool isNotHitting = false;
    [HideInInspector] public bool isNotKilling = false;

    [HideInInspector] public float notHittingTime;
    [HideInInspector] public float notKillingTime;

    [HideInInspector] public bool wasHitNotified;
    [HideInInspector] public bool wasKilledNotified;

    [HideInInspector] public float notHittingTimeThreshold;
    [HideInInspector] public float notKillingTimeThreshold;

    [HideInInspector] public bool wasParryPressed = false;
    [HideInInspector] public bool wasParryInvoked = false;

    [HideInInspector] public bool isDead = false;
    public string playerState;
    #endregion

    #region Unity Methods
    public void Awake()
    {
        input = ReInput.players.GetPlayer(0);
        interactions = GetComponent<PlayerInteraction>();
        inventory = GetComponent<PlayerInventory>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        clips = anim.runtimeAnimatorController.animationClips;
        currentHealth = health;
        currentStamina = stamina;
        GetComponent<PlayerUI>().SetUI();
        normalVfxTime = -1;
        comboVfxTime = -1;

        DowngradeSystem.Instance.SetPlayer(this);
        _parryParticleEmission = parryParticleEmission.emission;
        _parryParticleEmission.enabled = false;
        _hitParticleEmission = hitParticleEmission.emission;
        _hitParticleEmission.enabled = false;
        Invoker.InvokeDelayed(DelayedAwake, 0.1f);
        //NotifyObservers();
    }

    private void DelayedAwake()
    {
        FindObjectOfType<CutOutObject>().AddTarget(transform);
    }

    public void Update()
    {
        NotHittingKillingTimer();
        Stamina();
        NormalSlashVFXController();
        ComboSlashVFXController();

        normalSlashVFX.GetComponent<Renderer>().material.SetFloat("_Status", normalVfxTime);
        comboSlashVFX.GetComponent<Renderer>().material.SetFloat("_Status", comboVfxTime);

        if (!canMove || isDead)
            return;

        Inputs();
        PlayerAnimations();

        if (isParrying)
            ParryLogic();

        CooldownUpdate();
        ResetAnimClipUpdate();
        RotateSprite();
    }

    public void FixedUpdate()
    {
        if (!canMove || isAttacking)
            return;

        
        if (isRolling)
        {
            moveDuration += Time.deltaTime;
            rb.velocity = lastDirection.normalized * rollSpeed.Evaluate(moveDuration);
            return;
        }
        else
        {
            moveDuration = 0;
        }

        RotateHitboxCentreToFaceTheDirection();

        if (direction.sqrMagnitude > directionThreshold)
        {
            rb.velocity = direction.normalized * speed;
            lastDirection = direction;
        }
    }
    #endregion

    #region Actions
    public void Inputs()
    {
        if (isAttacking || wasParryPressed)
        {
            direction = Vector3.zero;
        }
        else
        {
            direction = new Vector3(input.GetAxisRaw("Horizontal"), 0, input.GetAxisRaw("Vertical"));
        }

        if (input.GetButtonDown("Attack"))
        {
            OverlapAttack();
        }

        if (input.GetButtonDown("Parry"))
        {
            if (!wasParryPressed)
            {
                Debug.Log("Parry Pressed");

                PlayAnimation(animationIDs[5], true); // Parry
                wasParryPressed = true;
                Invoke("ActivateParry", parryWindowTime.x);

            }
        }

        if (input.GetButtonDown("Roll"))
        {
            Roll();
        }

        if (input.GetButtonDown("Use Item"))
        {
            if (inventory.HasItem())
            {
                NotifyPlayerObservers(AllPlayerActions.useItem);
            }
            else
            {
                NotifyPlayerObservers(AllPlayerActions.useEmptyItem);
            }

            inventory.UseItem();
        }

        if (input.GetButtonDown("Drop Item"))
        {
            if (inventory.HasItem())
            {
                NotifyPlayerObservers(AllPlayerActions.dropItem);
            }
            else
            {
                NotifyPlayerObservers(AllPlayerActions.dropEmptyItem);
            }

            inventory.DropItem();
        }
    }

    public void Roll()
    {
        if (isRolling || isAttacking || currentStamina < staminaUsageRoll || direction == Vector3.zero)
            return;

        //rb.AddForce(direction.normalized * rollForce, ForceMode.Impulse);
        //rb.velocity = direction.normalized * rollSpeed.Evaluate(rollCooldown);

        isRolling = true;
        UseStamina(staminaUsageRoll);
        PlayAnimation(animationIDs[2], true, true); // Roll
        NotifyPlayerObservers(AllPlayerActions.Dodge);
        canBeDamaged = false;
        PlaySound(rollClips);
        Invoke("ResetDamage", rollInmunity);
        Invoke("ResetRoll", rollCooldown);
    }

    
    #endregion

    #region Combat

    #region Attack
    public void OverlapAttack()
    {
        if (currentStamina < staminaUsageAttack)
            return;

        bool isCombo = false;
        if (canCombo)
        {
            PlayAnimation(animationIDs[4], true, true); // Attack
            ActivateCooldown();
            ResetCooldown(comboCooldownTime);
            ResetCombo();
            Debug.Log("Combo");
            isCombo = true;
        }
        else
        {
            isCombo = false;
        }

        if (!isAnimationDone || isOnCooldown)
        {
            if (!isCombo)
            {
                return;
            }
        }

        if (!drawingAttackHitbox)
        {
            drawingAttackHitbox = true;
            Invoke("DrawingAttackHitbox", attackHitboxTime);
        }


        if (!canCombo && !isCombo)
        {
            PlayAnimation(animationIDs[3], true); // Attack
            ActivateCooldown();
            ResetCooldown(cooldownTime);
            Invoke("ActivateCombo", comboWindowTime.x);
            Invoke("ResetCombo", comboWindowTime.y);
            Debug.Log("Attack");
        }

        if (isCombo)
        {
            isComboVFXPlaying = true;
        }
        else
        {
            isNormalVFXPlaying = true;
        }

        UseStamina(staminaUsageAttack);
        rb.AddForce(lastDirection.normalized * attackForce, ForceMode.Impulse);
        PlaySound(attackClips);
        Collider[] hitColliders = Physics.OverlapBox(hitboxCenter.position, hitboxSize, Quaternion.LookRotation(lastDirection));

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log("Hit");

                int damage = Random.Range((int)attackDamage.x, (int)attackDamage.y + 1);

                if (hit.GetComponent<EnemyBase>())
                {
                    if (hit.GetComponent<EnemyBase>().currentHealth - damage <= 0)
                    {
                        ResetHittingKilling("Kill");
                        NotifyPlayerObservers(AllPlayerActions.KilledEnemy);
                    }
                    else
                    {
                        ResetHittingKilling("Hit");
                        NotifyPlayerObservers(AllPlayerActions.HitEnemy);
                    }

                    hit.GetComponent<EnemyBase>().TakeDamage(damage);
                }
                else if (hit.GetComponent<BossBase>())
                {
                    if (hit.GetComponent<BossBase>().GetCurrentHealth() - damage <= 0)
                    {
                        if (hit.GetComponent<BossBase>().GetCurrentFase() < hit.GetComponent<BossBase>().GetAllFases())
                        {
                            ResetHittingKilling("Kill");
                            NotifyPlayerObservers(AllPlayerActions.MidBoss);
                        }
                        else
                        {
                            ResetHittingKilling("Kill");
                            NotifyPlayerObservers(AllPlayerActions.EndBoss);
                        }
                    }
                    else
                    {
                        ResetHittingKilling("Hit");
                        NotifyPlayerObservers(AllPlayerActions.HitBoss);
                    }

                    hit.GetComponent<BossBase>().TakeDamage(damage);
                }
            }

            if (hit.CompareTag("Destructible"))
            {
                hit.GetComponent<Prop>().OnHit();
            }
        }
    }

    private void NotHittingKillingTimer()
    {
        notHittingTime += Time.deltaTime;
        notKillingTime += Time.deltaTime;

        if (notHittingTime > notHittingTimeThreshold)
        {
            isNotHitting = true;
        }

        if (notKillingTime > notKillingTimeThreshold)
        {
            isNotKilling = true;
        }

        if (isNotHitting && !wasHitNotified)
        {
            wasHitNotified = true;
            NotifyPlayerObservers(AllPlayerActions.NotKilling);
        }

        if (isNotKilling && !wasKilledNotified)
        {
            wasKilledNotified = true;
            NotifyPlayerObservers(AllPlayerActions.NotKilling);
        }
    }

    private void ResetHittingKilling(string action)
    {
        if (action == "Hit")
        {
            notHittingTime = 0;

            isNotHitting = false;
            wasHitNotified = false;
        }
        else if (action == "Kill")
        {
            notKillingTime = 0;

            isNotKilling = false;
            wasKilledNotified = false;
        }
    }
    #endregion

    #region Stamina
    public void Stamina()
    {
        if (isRolling || isAttacking)
        {
            staminaTimer = 0;
        }
        else
        {
            if (staminaTimer < staminaCooldown)
            {
                staminaTimer += Time.deltaTime;
            }
            else
            {
                if (currentStamina < stamina)
                {
                    currentStamina += staminaRegenSpeed * Time.deltaTime;
                }
            }
        }
    }

    public void UseStamina(float value)
    {
        currentStamina -= value;

        if (currentStamina < 0)
            currentStamina = 0;

        NotifyPlayerObservers(AllPlayerActions.LowStamina);
    }
    #endregion

    #region VFX
    public void NormalSlashVFXController()
    {
        if (!isNormalVFXPlaying)
            return;

        normalVfxTime += Time.deltaTime * vfxSpeed;

        if (normalVfxTime >= 1)
        {
            normalVfxTime = -1;
            isNormalVFXPlaying = false;
        }
    }

    public void ComboSlashVFXController()
    {
        if (!isComboVFXPlaying)
            return;

        comboVfxTime += Time.deltaTime * vfxSpeed;

        if (comboVfxTime >= 1)
        {
            comboVfxTime = -1;
            isComboVFXPlaying = false;
        }
    }
    #endregion

    #region Parry
    public void ParryLogic()
    {
        if (!wasParryInvoked)
        {
            wasParryInvoked = true;
            Invoke("ResetParry", parryWindowTime.y);
        }
        //PlayAnimation(animationIDs[5], true); // Parry
    }
    #endregion

    #region Rewards
    public void GetHealth(float healthReward)
    {
        currentHealth += healthReward;

        if (currentHealth > health)
            currentHealth = health;

        NotifyPlayerObservers(AllPlayerActions.Heal);
    }

    public void GetStamina(float staminaReward)
    {
        currentStamina += staminaReward;

        if (currentStamina > stamina)
            currentStamina = stamina;
    }

    public void GetParryReward(bool isBigEnemy, bool isBoss, bool isProjectile = false)
    {
        _parryParticleEmission.enabled = true;
        Invoker.InvokeDelayed(DisableParryParticles, 0.1f);

        if (isBigEnemy)
        {
            GetStamina(staminaBigEnemyReward);
            GetHealth(healthBigEnemyReward);
        }
        else if (isBoss)
        {
            GetStamina(staminaBossReward);
            GetHealth(healthBossReward);
            // damage multiplier
        }
        else if (!isProjectile)
        {
            GetStamina(staminaNormalReward);
        }
    }

    
    #endregion

    #region Damage
    public void TakeDamage(float damage)
    {
        if (isDead || !canBeDamaged)
            return;

        if (canBeStaggered)
        {
            canMove = false;
            Invoke("ActivateMovement", damagedCooldown);
            PlayAnimation(animationIDs[0], false, true);
        }

        currentHealth -= damage;

        _hitParticleEmission.enabled = true;
        Invoker.InvokeDelayed(DisableHitParticles, 0.1f);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            PlayAnimation(animationIDs[6]);
            PlaySound(hitClips);
            NotifyPlayerObservers(AllPlayerActions.LowHealth);
        }
    }

    public void TakeDamage(float damage, float knockbackForce, Vector3 damagePos)
    {
        if (isDead || !canBeDamaged)
            return;

        rb.AddForce(-damagePos.normalized * knockbackForce, ForceMode.Impulse);

        if (canBeStaggered)
        {
            canMove = false;
            Invoke("ActivateMovement", damagedCooldown);
            PlayAnimation(animationIDs[0], false, true);
        }

        currentHealth -= damage;

        if (damage != 0)
        {
            _hitParticleEmission.enabled = true;
            Invoker.InvokeDelayed(DisableHitParticles, 0.1f);
        }
        else
        {
            _parryParticleEmission.enabled = true;
            Invoker.InvokeDelayed(DisableParryParticles, 0.1f);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            PlaySound(hitClips);
            NotifyPlayerObservers(AllPlayerActions.LowHealth);
        }
    }

    

    public void Die()
    {
        //Destroy(gameObject);
        Debug.Log("Dead");
        isDead = true;
        playerState = "Dead";

        direction = Vector3.zero;
        lastDirection = Vector3.zero;
        PlaySound(deathClips);
        NotifyPlayerObservers(AllPlayerActions.Die);
        FindObjectOfType<TextScreens>().OnDeath();
    }
    #endregion

    #endregion

    #region PlayerAnimations
    public void PlayerAnimations()
    {
        if (direction == Vector3.zero)
        {
            PlayAnimation(animationIDs[0]); // Idle
        }
        else
        {
            if (direction.x < 0 && direction.z == 0)
            {
                PlayAnimation(animationIDs[1]); // WalkLeft
                isFacingLeft = false;
            }
            else if (direction.x > 0 && direction.z == 0)
            {
                PlayAnimation(animationIDs[1]); // WalkRight
                isFacingLeft = true;
            }
            else if (direction.z > 0 && direction.x == 0)
            {
                PlayAnimation(animationIDs[1]); // WalkUp
            }
            else if (direction.z < 0 && direction.x == 0)
            {
                PlayAnimation(animationIDs[1]); // WalkDown
            }
            else if (direction.x < 0 && direction.z > 0)
            {
                PlayAnimation(animationIDs[1]); // WalkLeftUp
                isFacingLeft = false;
            }
            else if (direction.x > 0 && direction.z > 0)
            {
                PlayAnimation(animationIDs[1]); // WalkRightUp
                isFacingLeft = true;
            }
            else if (direction.x < 0 && direction.z < 0)
            {
                PlayAnimation(animationIDs[1]); // WalkLeftDown
                isFacingLeft = false;
            }
            else if (direction.x > 0 && direction.z < 0)
            {
                PlayAnimation(animationIDs[1]); // WalkRightDown
                isFacingLeft = true;
            }
        }

        // if the attack animation is playing, the player can't move, taking the current animation state
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(animationIDs[3]) || anim.GetCurrentAnimatorStateInfo(0).IsName(animationIDs[4]))
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }
    }
    #endregion

    #region Utility

    #region AnimationController
    public void PlayAnimation(string animName, bool hasExitTime = false, bool bypassExitTime = false)
    {
        if (bypassExitTime)
            isAnimationDone = true;

        if (!isAnimationDone)
            return;

        for (int i = 0; i < animationIDs.Length; i++)
        {
            if (animName == animationIDs[i])
            {
                anim.Play(animName);

                if (hasExitTime)
                {
                    isAnimationDone = false;

                    foreach (AnimationClip clip in clips)
                    {
                        if (clip.name == animName)
                        {
                            animClipLength = clip.length;
                        }
                    }
                }
                return;
            }
        }
    }

    public void ResetAnimClipUpdate()
    {
        if (animClipLength <= 0)
        {
            isAnimationDone = true;
        }
        else
        {
            animClipLength -= Time.deltaTime;
            isAnimationDone = false;
        }
    }
    #endregion

    #region Sounds
    public void PlaySound(AudioClip[] clip)
    {
        if (clip.Length > 0)
        {
            int random = Random.Range(0, clip.Length);
            AudioManager.instance.PlayCustomSFX(clip[random], audioSource);
        }
        else
        {
            AudioManager.instance.PlayCustomSFX(clip[0], audioSource);
        }
    }
    #endregion

    #region RotateHitbox
    public void RotateHitboxCentreToFaceTheDirection()
    {
        if (lastDirection == Vector3.zero)
            return;

        Vector3 direction = lastDirection.normalized;
        Vector3 desiredPosition = transform.position + direction * offset;
        Quaternion rotation = Quaternion.LookRotation(direction);
        hitboxCenter.rotation = rotation;
        hitboxCenter.position = new Vector3(desiredPosition.x, hitboxCenter.position.y, desiredPosition.z);
    }
    #endregion

    #region Rotate Sprite
    public void RotateSprite()
    {
        if (isFacingLeft)
        {
            //transform.rotation = Quaternion.Euler(0, 0, 0);
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            //transform.rotation = Quaternion.Euler(0, 180, 0);
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }
    #endregion

    #region Invokes
    public void ActivateMovement()
    {
        canMove = true;
    }

    public void ActivateParry()
    {
        Debug.Log("Parry Active");
        isParrying = true;
        playerState = "Parry";
    }

    public void ResetParry()
    {
        Debug.Log("Parry Reset");
        isParrying = false;
        wasParryInvoked = false;
        wasParryPressed = false;
        playerState = "";
    }

    public void DrawingAttackHitbox()
    {
        drawingAttackHitbox = false;
    }

    public void ActivateCombo()
    {
        canCombo = true;
    }

    public void ResetCombo()
    {
        canCombo = false;
    }

    public void ActivateCooldown()
    {
        isOnCooldown = true;
    }

    public void CooldownUpdate()
    {
        if (comboTime <= 0)
        {
            isOnCooldown = false;
        }
        else
        {
            comboTime -= Time.deltaTime;
        }
    }

    public void ResetCooldown(float time)
    {
        comboTime = time;
    }

    public void ResetRoll()
    {
        isRolling = false;
    }

    public void ResetDamage()
    {
        canBeDamaged = true;
    }

    public void DisableParryParticles()
    {
        _parryParticleEmission.enabled = false;
    }

    public void DisableHitParticles()
    {
        _hitParticleEmission.enabled = false;
    }
    #endregion

    #region GetCurrentStats
    public float GetHealth()
    {
        return currentHealth;
    }

    public float GetStamina()
    {
        return currentStamina;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public Vector2 GetDamage()
    {
        return attackDamage;
    }

    public string GetPlayerState()
    {
        return playerState;
    }

    public Vector3 GetLastDirection()
    {
        return lastDirection;
    }

    public bool IsNotHittingEnemy()
    {
        return isNotHitting;
    }

    public bool IsNotKillingEnemy()
    {
        return isNotKilling;
    }
    #endregion

    #region SetStats
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetDamage(Vector2 damage)
    {
        attackDamage = damage;
    }
    #endregion

    #endregion

    #region Debug
    public void DrawAttackHitbox()
    {
        if (lastDirection == Vector3.zero)
        {
            VisualizeBox.DisplayBox(hitboxPos + hitboxCenter.position, hitboxSize, Quaternion.identity, attackHitboxColor);
        }
        else
        {
            VisualizeBox.DisplayBox(hitboxPos + hitboxCenter.position, hitboxSize, Quaternion.LookRotation(lastDirection), attackHitboxColor);
        }
    }

    public void OnDrawGizmos()
    {
        if (drawHitbox)
        {
            if (drawHitboxOnGameplay)
            {
                if (drawingAttackHitbox)
                    DrawAttackHitbox();
            }
            else
            {
                DrawAttackHitbox();
            }
        }
    }
    #endregion
}
