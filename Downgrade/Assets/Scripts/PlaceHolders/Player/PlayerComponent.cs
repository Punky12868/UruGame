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
    public float rollForce = 5f;
    public float rollInmunity = 1f;
    public float rollCooldown = 1f;

    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentStamina;
    [HideInInspector] public float staminaTimer;

    [Header("Fighting")]
    public Transform hitboxCenter;
    public float offset = 0.3f;
    public float attackDamage = 5;
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
    //[SerializeField] private float knockbackForce;

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
    [ShowIf("debugTools", true, true)] public bool drawHitbox = true;
    [ShowIf("debugTools", true, true)][ShowIf("drawHitbox", true, true)] public bool drawHitboxOnGameplay = true;
    [ShowIf("debugTools", true, true)][ShowIf("drawHitbox", true, true)] public float attackHitboxTime = 0.2f;
    [ShowIf("debugTools", true, true)][ShowIf("drawHitbox", true, true)] public Color attackHitboxColor = new Color(1, 0, 0, 1);
    [ShowIf("debugTools", true, true)][ShowIf("drawHitbox", true, true)] public Color parryHitboxColor = new Color(0, 1, 0, 1);

    //[Header("PlayerStatus")]
    [HideInInspector] public bool isFacingRight;

    [HideInInspector] public bool isOnCooldown = false;
    [HideInInspector] public bool canCombo = false;
    [HideInInspector] public bool drawingAttackHitbox = false;

    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool canBeDamaged = true;
    [HideInInspector] public bool isParrying = false;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isRolling = false;

    [HideInInspector] public bool wasParryPressed = false;
    [HideInInspector] public bool wasParryInvoked = false;

    [HideInInspector] public bool isDead = false;
    [HideInInspector] public string playerState;
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

        //NotifyObservers();
    }

    public void Update()
    {
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
    }

    public void FixedUpdate()
    {
        if (!canMove || isAttacking || isRolling)
            return;

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
                wasParryPressed = true;
                Invoke("ActivateParry", parryWindowTime.x);
            }
        }

        if (input.GetButtonDown("Roll"))
        {
            Roll();
        }

        if (input.GetButtonDown("UseItem"))
        {
            inventory.UseItem();
        }

        if (input.GetButtonDown("DropItem"))
        {
            inventory.DropItem();
        }
    }

    public void Roll()
    {
        if (isRolling || isAttacking || currentStamina < staminaUsageRoll)
            return;

        if (direction == Vector3.zero)
        {
            rb.AddForce(lastDirection.normalized * rollForce, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(direction.normalized * rollForce, ForceMode.Impulse);
        }

        currentStamina -= staminaUsageRoll;
        NotifyObservers(AllActions.LowStamina);
        isRolling = true;
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
            PlayAnimation(animationIDs[8], true, true); // Attack
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
            PlayAnimation(animationIDs[7], true); // Attack
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

        currentStamina -= staminaUsageAttack;
        rb.AddForce(lastDirection.normalized * attackForce, ForceMode.Impulse);
        NotifyObservers(AllActions.LowStamina);
        PlaySound(attackClips);
        Collider[] hitColliders = Physics.OverlapBox(hitboxCenter.position, hitboxSize, Quaternion.LookRotation(lastDirection));

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log("Hit");
                hit.GetComponent<EnemyBase>().TakeDamage(attackDamage);
            }

            if (hit.CompareTag("Destructible"))
            {
                hit.GetComponent<Prop>().OnHit();
            }
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
        PlayAnimation(animationIDs[9], true); // Parry
    }
    #endregion

    #region Rewards
    public void GetHealth(float healthReward)
    {
        currentHealth += healthReward;

        if (currentHealth > health)
            currentHealth = health;
    }

    public void GetStamina(float staminaReward)
    {
        currentStamina += staminaReward;

        if (currentStamina > stamina)
            currentStamina = stamina;
    }

    public void GetParryReward(bool isBigEnemy, bool isBoss)
    {
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
        else
        {
            GetStamina(staminaNormalReward);
        }
    }
    #endregion

    #region Damage
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

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            PlaySound(hitClips);
            NotifyObservers(AllActions.LowHealth);
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
        NotifyObservers(AllActions.Die);
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
                PlayAnimation(animationIDs[5]); // WalkLeft
                isFacingRight = false;
            }
            else if (direction.x > 0 && direction.z == 0)
            {
                PlayAnimation(animationIDs[4]); // WalkRight
                isFacingRight = true;
            }
            else if (direction.z > 0 && direction.x == 0)
            {
                PlayAnimation(animationIDs[3]); // WalkUp
            }
            else if (direction.z < 0 && direction.x == 0)
            {
                PlayAnimation(animationIDs[6]); // WalkDown
            }
            else if (direction.x < 0 && direction.z > 0)
            {
                PlayAnimation(animationIDs[2]); // WalkLeftUp
                isFacingRight = false;
            }
            else if (direction.x > 0 && direction.z > 0)
            {
                PlayAnimation(animationIDs[1]); // WalkRightUp
                isFacingRight = true;
            }
            else if (direction.x < 0 && direction.z < 0)
            {
                PlayAnimation(animationIDs[5]); // WalkLeftDown
                isFacingRight = false;
            }
            else if (direction.x > 0 && direction.z < 0)
            {
                PlayAnimation(animationIDs[4]); // WalkRightDown
                isFacingRight = true;
            }
        }

        // if the attack animation is playing, the player can't move, taking the current animation state
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(animationIDs[7]) || anim.GetCurrentAnimatorStateInfo(0).IsName(animationIDs[8]))
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
    public void PlayAnimation(string animName)
    {
        if (!isAnimationDone)
            return;

        for (int i = 0; i < animationIDs.Length; i++)
        {
            if (animName == animationIDs[i])
            {
                anim.Play(animName);
                return;
            }
        }
    }

    public void PlayAnimation(string animName, bool hasExitTime)
    {
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

    public void PlayAnimation(string animName, bool hasExitTime, bool bypassExitTime)
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

    #region Invokes
    public void ActivateMovement()
    {
        canMove = true;
    }

    public void ActivateParry()
    {
        isParrying = true;
        playerState = "Parry";
    }

    public void ResetParry()
    {
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
    #endregion

    #region GetCurrentStats
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    public string GetPlayerState()
    {
        return playerState;
    }

    public Vector3 GetLastDirection()
    {
        return lastDirection;
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
