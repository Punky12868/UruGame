using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : Subject
{
    // PlaceHolder for the PlayerController
    private Player input;
    private PlayerInteraction interactions;
    private Inventory inventory;
    private Rigidbody rb;
    private Animator anim;

    private Vector3 direction;
    private Vector3 lastDirection;

    [Header("Misc")]
    [SerializeField] private float health = 100;
    [SerializeField] private float healthBigEnemyReward = 10;
    [SerializeField] private float healthBossReward = 45;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float stamina = 100;
    [SerializeField] private float staminaCooldown = 1.5f;
    [SerializeField] private float staminaRegenSpeed = 5;
    [SerializeField] private float staminaNormalReward = 10;
    [SerializeField] private float staminaBigEnemyReward = 20;
    [SerializeField] private float staminaBossReward = 50;
    [SerializeField] private float staminaUsageRoll;
    [SerializeField] private float staminaUsageAttack;
    [SerializeField] private float rollForce = 5f;
    [SerializeField] private float rollInmunity = 1f;
    [SerializeField] private float rollCooldown = 1f;

    float currentHealth;
    float currentStamina;
    float staminaTimer;

    [Header("Fighting")]
    [SerializeField] private Transform hitboxCenter;
    [SerializeField] private float offset = 0.3f;
    [SerializeField] private float attackDamage = 5;
    [SerializeField] private float cooldownTime = 0.5f;
    [SerializeField] private float comboCooldownTime = 0.5f;
    [SerializeField] private bool canBeStaggered = false;
    [SerializeField] private float damagedCooldown = 0.35f;
    [SerializeField] private float attackForce;

    [SerializeField] private float vfxSpeed = 1;
    [SerializeField] private GameObject normalSlashVFX;
    [SerializeField] private GameObject comboSlashVFX;
    private float normalVfxTime;
    private float comboVfxTime;
    private bool isNormalVFXPlaying = false;
    private bool isComboVFXPlaying = false;
    //[SerializeField] private float knockbackForce;

    private float comboTime;

    [Header("Input")]
    [SerializeField] private float directionThreshold = 0.1f;
    [SerializeField] private Vector2 comboWindowTime;
    [SerializeField] private Vector2 parryWindowTime;

    [SerializeField] private Vector3 hitboxPos = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 hitboxSize = new Vector3(0.5f, 0.5f, 0.5f);

    [Header("Animation")]
    [SerializeField] private string[] animationIDs;
    private AnimationClip[] clips;
    private bool isAnimationDone = true;
    private float animClipLength;

    [Header("AudioClips")]
    [SerializeField] private AudioClip[] attackClips;
    [SerializeField] private AudioClip[] rollClips;
    [SerializeField] private AudioClip[] hitClips;
    [SerializeField] private AudioClip[] deathClips;
    [HideInInspector] public AudioSource audioSource;

    [Header("Debug")]
    [SerializeField] private bool debugTools = true;
    [ShowIf("debugTools", true, true)] [SerializeField] private bool drawHitbox = true;
    [ShowIf("debugTools", true, true)] [ShowIf("drawHitbox", true, true)] [SerializeField] private bool drawHitboxOnGameplay = true;
    [ShowIf("debugTools", true, true)] [ShowIf("drawHitbox", true, true)] [SerializeField] private float attackHitboxTime = 0.2f;
    [ShowIf("debugTools", true, true)] [ShowIf("drawHitbox", true, true)] [SerializeField] private Color attackHitboxColor = new Color(1, 0, 0, 1);
    [ShowIf("debugTools", true, true)] [ShowIf("drawHitbox", true, true)] [SerializeField] private Color parryHitboxColor = new Color(0, 1, 0, 1);

    //[Header("PlayerStatus")]
    private bool isFacingRight;

    private bool isOnCooldown = false;
    private bool canCombo = false;
    private bool drawingAttackHitbox = false;

    private bool canMove = true;
    private bool canBeDamaged = true;
    private bool isParrying = false;
    private bool isAttacking = false;
    private bool isRolling = false;

    private bool wasParryPressed = false;
    private bool wasParryInvoked = false;

    private bool isDead = false;
    private string playerState;

    private void Awake()
    {
        input = ReInput.players.GetPlayer(0);
        interactions = GetComponent<PlayerInteraction>();
        inventory = GetComponent<Inventory>();
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

    private void Update()
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

        if(isParrying)
            ParryLogic();

        CooldownUpdate();
        ResetAnimClipUpdate();
    }

    private void FixedUpdate()
    {
        if (!canMove || isAttacking || isRolling)
            return;

        RotateHitboxCentreToFaceTheDirection();

        if (direction.sqrMagnitude > directionThreshold)
        {
            rb.velocity = direction.normalized * speed;
            lastDirection = direction;
            /*if (rb.velocity.magnitude > speed)
                rb.velocity = rb.velocity.normalized * speed;*/
        }
    }

    private void Inputs()
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

        if (input.GetButtonDown("Interact"))
        {
            interactions.Interact();
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

    private void Stamina()
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

    private void Roll()
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

    private void Die()
    {
        //Destroy(gameObject);
        Debug.Log("Dead");
        isDead = true;
        playerState = "Dead";

        direction = Vector3.zero;
        lastDirection = Vector3.zero;
        PlaySound(deathClips);
        NotifyObservers(AllActions.Die);
        FindObjectOfType<DeathScreen>().OnDeath();
    }

    private void RotateHitboxCentreToFaceTheDirection()
    {
        if (lastDirection == Vector3.zero)
            return;

        Vector3 direction = lastDirection.normalized;
        Vector3 desiredPosition = transform.position + direction * offset;
        Quaternion rotation = Quaternion.LookRotation(direction);
        hitboxCenter.rotation = rotation;
        hitboxCenter.position = new Vector3(desiredPosition.x, hitboxCenter.position.y, desiredPosition.z);
    }

    #region Invokes
    private void ActivateMovement()
    {
        canMove = true;
    }

    private void ActivateParry()
    {
        isParrying = true;
        playerState = "Parry";
    }

    private void ResetParry()
    {
        isParrying = false;
        wasParryInvoked = false;
        wasParryPressed = false;
        playerState = "";
    }

    private void DrawingAttackHitbox()
    {
        drawingAttackHitbox = false;
    }

    private void ActivateCombo()
    {
        canCombo = true;
    }

    private void ResetCombo()
    {
        canCombo = false;
    }

    private void ActivateCooldown()
    {
        isOnCooldown = true;
    }

    private void CooldownUpdate()
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

    private void ResetCooldown(float time)
    {
        comboTime = time;
    }

    private void ResetRoll()
    {
        isRolling = false;
    }

    private void ResetDamage()
    {
        canBeDamaged = true;
    }
    #endregion

    #region Overlap Hitbox
    private void OverlapAttack()
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

    private void NormalSlashVFXController()
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

    private void ComboSlashVFXController()
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

    private void ParryLogic()
    {
        if (!wasParryInvoked)
        {
            wasParryInvoked = true;
            Invoke("ResetParry", parryWindowTime.y);
        }
        PlayAnimation(animationIDs[9], true); // Parry
    }
    #endregion

    #region Animation
    private void PlayerAnimations()
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

    #region Utility
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

    #region Debug
    private void DrawAttackHitbox()
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

    private void OnDrawGizmos()
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
