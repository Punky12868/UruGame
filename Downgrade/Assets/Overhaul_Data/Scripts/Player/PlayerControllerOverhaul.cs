using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.Rendering;

[RequireComponent(typeof(PlayerInventory))] [RequireComponent(typeof(PlayerInteraction))]
[RequireComponent(typeof(AnimationHolder))] [RequireComponent(typeof(Rigidbody))]
public class PlayerControllerOverhaul : Subject, IAnimController
{
    #region Variables

    private Player input;
    private PlayerInventory inventory;
    private AnimationHolder animHolder;

    private Rigidbody rb;
    private Vector3 direction, lastDirection;
    private List<AnimationClip> animationIDs;

    [Header("General")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float iFrames = 0.3f;
    [SerializeField] private Vector2 attackDamage = new Vector2(15f, 25f);
    [SerializeField] private AnimationCurve rollSpeed;
    private float currentHealth;
    private float moveDuration = 0f;

    [Header("Stamina")]
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float staminaCooldown = 0.3f;
    [SerializeField] private float staminaRegenSpeed = 25f;
    [SerializeField] private float staminaUsageRoll = 10f;
    [SerializeField] private float staminaUsageAttack = 3f;
    private float staminaTimer, currentStamina;

    [Header("Fighting")]
    [SerializeField] private Transform hitboxCenter;
    [SerializeField] private float offset = 0.3f;
    [SerializeField] private bool canBeStaggered = false;

    [Header("Ability")]
    [SerializeField] private GameObject abilityPrefab;
    [SerializeField] private float abilityCooldown = 3f;
    [SerializeField] private Vector2 abilityDamage = new Vector2(25f, 35f);
    [SerializeField] private float abilityKnockback = 10f;
    [SerializeField] private float abilityLifeTime = 3f;

    [Header("Rewards")]
    [SerializeField] private float healthBigEnemyReward = 5f;
    [SerializeField] private float healthBossReward = 10f;
    [SerializeField] private float staminaNormalReward = 25f;
    [SerializeField] private float staminaBigEnemyReward = 50f;
    [SerializeField] private float staminaBossReward = 100f;

    [Header("Combat VFX")]
    [SerializeField] private float vfxSpeed = 15f;
    [SerializeField] private GameObject normalSlashVFX;
    [SerializeField] private GameObject comboSlashVFX;
    [SerializeField] private Material inpactVFX;
    private float normalVfxTime, comboVfxTime;
    private bool isNormalVFXPlaying = false;
    private bool isComboVFXPlaying = false;

    [Header("Particles")]
    [SerializeField] private ParticleSystem parryParticleEmission;
    [SerializeField] private ParticleSystem parryParticleEmissionTwo;
    [SerializeField] private ParticleSystem hitParticleEmission;
    private ParticleSystem.EmissionModule _parryParticleEmission, _parryParticleEmissionTwo, _hitParticleEmission;

    [Header("AudioClips")]
    [SerializeField] private AudioClip[] attackClips;
    [SerializeField] private AudioClip[] abilityClips;
    [SerializeField] private AudioClip[] rollClips;
    [SerializeField] private AudioClip[] hitClips;
    [SerializeField] private AudioClip[] deathClips;
    private AudioSource audioSource;

    [Header("Input")]
    [SerializeField] private float directionThreshold = 0.1f;
    [SerializeField] private Vector3 hitboxPos = new Vector3(0f, 0.25f, 0f);
    [SerializeField] private Vector3 hitboxSize = new Vector3(0.48f, 0.2f, 0.25f);

    [Header("Debug")]
    [SerializeField] private bool debugTools = true;
    [SerializeField] private bool drawHitbox = true;
    [SerializeField] private bool drawHitboxOnGameplay = true;
    [SerializeField] private float attackHitboxTime = 0.2f;
    [SerializeField] private Color attackHitboxColor = new Color(1, 0, 0, 1);

    #region States
    private bool isDead = false;
    private bool isFacingLeft;
    private bool isAttacking = false;
    private bool isComboAttack = false;
    private bool isRolling = false;
    private bool isOnCooldown = false;
    private bool isAbilityOnCooldown = false;
    private bool canMove = true;
    private bool canAttack = true;
    private bool canCombo = false;
    private bool canBeDamaged = true;
    private bool wasHitNotified;
    private bool wasKilledNotified;
    private float notHittingTime;
    private float notHittingTimeThreshold;
    private float notKillingTime;
    private float notKillingTimeThreshold;
    private bool isNotHitting = false;
    private bool isNotKilling = false;
    private bool monedaActive = false;
    private bool wasParryPressed = false;
    private bool paralized = false;
    private bool finiteRolling = false;
    private int rollAmmount = 999;
    private bool drawingAttackHitbox = false;
    private string playerState;
    #endregion

    #endregion

    #region Unity Methods

    public void Awake()
    {
        input = ReInput.players.GetPlayer(0);
        inventory = GetComponent<PlayerInventory>();
        rb = GetComponent<Rigidbody>();
        SetAnimHolder();
        audioSource = GetComponent<AudioSource>();
        currentHealth = health;
        currentStamina = stamina;
        //FindObjectOfType<PlayerUI>().SetUI();
        normalVfxTime = -1;
        comboVfxTime = -1;

        DowngradeSystem.Instance.SetPlayer(this);
        _parryParticleEmission = parryParticleEmission.emission;
        _parryParticleEmission.enabled = false;
        _parryParticleEmissionTwo = parryParticleEmissionTwo.emission;
        _parryParticleEmissionTwo.enabled = false;
        _hitParticleEmission = hitParticleEmission.emission;
        _hitParticleEmission.enabled = false;
        Invoker.InvokeDelayed(DelayedAwake, 0.1f);
        
        
        //NotifyObservers();
    }
    private void DelayedAwake() 
    {
        NotifyPlayerObservers(AllPlayerActions.Start);
        FindObjectOfType<CutOutObject>().AddTarget(transform); 
        if (FindObjectOfType<SetPlayerMap>()) FindObjectOfType<SetPlayerMap>().PlayerStarts(); 
    }

    public void Update()
    {
        NotHittingKillingTimer();
        Stamina();
        SlashVFXProxy();

        normalSlashVFX.GetComponent<Renderer>().material.SetFloat("_Status", normalVfxTime);
        comboSlashVFX.GetComponent<Renderer>().material.SetFloat("_Status", comboVfxTime);

        if (!canMove || isDead || paralized) return;

        Inputs();
        //PlayerAnimations();

        //CooldownUpdate();
        FlipSprite();
    }

    public void FixedUpdate()
    {
        if (!canMove || isAttacking || paralized || isOnCooldown) return;

        if (isRolling)
        {
            moveDuration += Time.deltaTime;
            rb.velocity = lastDirection.normalized * rollSpeed.Evaluate(moveDuration);
            return;
        }
        else moveDuration = 0;

        RotateHitboxCentreToFaceTheDirection();

        if (direction.sqrMagnitude > directionThreshold)
        {
            rb.velocity = direction.normalized * speed;
            lastDirection = direction;
            PlayAnimation(1);
        }
        else PlayAnimation(0);
    }

    #endregion

    #region Actions
    public void Inputs()
    {
        if (isAttacking || wasParryPressed || isOnCooldown) direction = Vector3.zero;
        else direction = new Vector3(input.GetAxisRaw("Horizontal"), 0, input.GetAxisRaw("Vertical"));

        if (input.GetButtonDown("Attack")) OverlapAttack();
        if (input.GetButtonDown("Parry")) ParryLogic();
        if (input.GetButtonDown("Roll")) Roll();
        if (input.GetButtonDown("Use Item")) UseItem();
        if (input.GetButtonDown("Drop Item")) DropItem();
        if (input.GetButtonDown("Use Ability")) UseAbility();
    }
    #endregion

    #region Player Logic

    #region Input Behaviours

    #region Attack
    private void OverlapAttack()
    {
        if (currentStamina < staminaUsageAttack || !canAttack) return;

        #region ComboLogic
        bool isCombo = false;
        if (canCombo)
        {
            PlayAnimation(4, true, true); // has window time event ^^ combo attack
            Debug.Log("Combo");
            isCombo = true;
        }
        else isCombo = false;

        if (!IsAnimationDone() || isOnCooldown) {if (!isCombo) return;}

        if (!drawingAttackHitbox)
        {
            drawingAttackHitbox = true;
            //Invoke("DrawingAttackHitbox", attackHitboxTime); // not necessary
        }

        if (!canCombo && !isCombo)
        {
            PlayAnimation(3, true, true); // has window time event ^^ normal attack
            Debug.Log("Attack");
        }

        if (isCombo) isComboVFXPlaying = true;
        else isNormalVFXPlaying = true;
        #endregion

        UseStamina(staminaUsageAttack);
        //rb.AddForce(lastDirection.normalized * attackForce, ForceMode.Impulse);
        PlaySound(attackClips);
        HitboxCall(hitboxCenter.position, hitboxSize);
    }

    private void NotHittingKillingTimer()
    {
        notHittingTime += Time.deltaTime; notKillingTime += Time.deltaTime;

        if (notHittingTime > notHittingTimeThreshold) isNotHitting = true;
        if (notKillingTime > notKillingTimeThreshold) isNotKilling = true;

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
            isNotHitting = false; wasHitNotified = false;
        }
        else if (action == "Kill")
        {
            notKillingTime = 0;
            isNotKilling = false; wasKilledNotified = false;
        }
    }
    #endregion

    #region Parry
    private void ParryLogic()
    {
        if (!wasParryPressed)
        {
            Debug.Log("Parry Pressed");

            PlayAnimation(5, true);
        }
    }
    #endregion

    #region Roll
    public void Roll()
    {
        if (isRolling || isAttacking || currentStamina < staminaUsageRoll || lastDirection == Vector3.zero || finiteRolling && rollAmmount <= 0) return;

        if (finiteRolling) rollAmmount--;

        UseStamina(staminaUsageRoll);
        PlayAnimation(2, true, true);
        NotifyPlayerObservers(AllPlayerActions.Dodge);
        PlaySound(rollClips);
    }
    #endregion

    #region Ability
    private void UseAbility()
    {
        if (isAbilityOnCooldown) { NotifyPlayerObservers(AllPlayerActions.useAbilityOnCooldown); return; }
        PlayAnimation(7, true);
        PlaySound(abilityClips);
        isAbilityOnCooldown = true;
        Invoker.InvokeDelayed(ResetAbilityCooldown, abilityCooldown);
        NotifyPlayerObservers(AllPlayerActions.useAbility);
    }

    public void SpawnAbilityPrefab()
    {
        //if (isAbilityOnCooldown) return;
        Vector3 spawnPos = new Vector3(transform.position.x, -0.05f, transform.position.z);
        GameObject ability = Instantiate(abilityPrefab, spawnPos, Quaternion.LookRotation(lastDirection));
        int randomDmg = Random.Range((int)abilityDamage.x, (int)abilityDamage.y + 1);
        ability.GetComponent<AbilityCell>().SetVariables(randomDmg, abilityKnockback, abilityLifeTime, lastDirection, true, false, false);
    }
    #endregion

    #region Item
    #region Use Item
    private void UseItem()
    {
        if (inventory.HasItem()) NotifyPlayerObservers(AllPlayerActions.useItem);
        else NotifyPlayerObservers(AllPlayerActions.useEmptyItem);

        if (!monedaActive) inventory.UseItem();
    }
    #endregion

    #region Drop Item
    private void DropItem()
    {
        if (inventory.HasItem()) NotifyPlayerObservers(AllPlayerActions.dropItem);
        else NotifyPlayerObservers(AllPlayerActions.dropEmptyItem);

        inventory.DropItem();
    }
    #endregion
    #endregion

    #endregion

    #region System Logic

    #region AttackHitbox

    private void HitboxCall(Vector3 pos, Vector3 size)
    {
        Collider[] hitColliders = Physics.OverlapBox(pos, size, Quaternion.LookRotation(lastDirection));
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log("Hit");

                int damage = Random.Range((int)attackDamage.x, (int)attackDamage.y + 1);
                if (hit.GetComponent<EnemyBase>()) HitEnemy(hit, damage);
                else if (hit.GetComponent<BossBase>()) HitBoss(hit, damage);
            }

            if (hit.CompareTag("Destructible")) hit.GetComponent<Prop>().OnHit();
        }
    }

    private void HitEnemy(Collider hit, int damage)
    {
        Debug.Log("Hit");

        if (hit.GetComponent<EnemyBase>().GetCurrentHealth() - damage <= 0)
        {
            ResetHittingKilling("Kill");
            NotifyPlayerObservers(AllPlayerActions.KilledEnemy);
        }
        else
        {
            ResetHittingKilling("Hit");
            NotifyPlayerObservers(AllPlayerActions.HitEnemy);
        }

        hit.GetComponent<EnemyBase>().TakeDamageProxy(damage);
    }

    private void HitBoss(Collider hit, int damage)
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

    #endregion

    #region Reward
    private void GainHealth(float healthReward)
    {
        currentHealth += healthReward;
        if (currentHealth > health) currentHealth = health;
        NotifyPlayerObservers(AllPlayerActions.Heal);
    }

    private void GainStamina(float staminaReward)
    {
        currentStamina += staminaReward;
        if (currentStamina > stamina) currentStamina = stamina;
    }

    private void GetParryReward(EnemyType type, bool isProjectile = false)
    {
        NotifyPlayerObservers(AllPlayerActions.SuccesfullParry);
        _parryParticleEmission.enabled = true;
        _parryParticleEmissionTwo.enabled = true;
        inpactVFX.SetFloat("_isOn", 1);
        Invoker.InvokeDelayed(DisableParryParticles, 0.2f);

        if (isProjectile) return;

        switch (type)
        {
            case EnemyType.Small:
                GainStamina(staminaNormalReward);
                break;
            case EnemyType.Big:
                GainStamina(staminaBigEnemyReward);
                GainHealth(healthBigEnemyReward);
                break;
            case EnemyType.Boss:
                GainStamina(staminaBossReward);
                GainHealth(healthBossReward);
                break;
            case EnemyType.None:
                break;
        }
    }
    #endregion

    #region Stamina
    public void Stamina()
    {
        if (isRolling || isAttacking) staminaTimer = 0;
        else
        {
            if (staminaTimer < staminaCooldown) {staminaTimer += Time.deltaTime;}
            else {if (currentStamina < stamina) currentStamina += staminaRegenSpeed * Time.deltaTime;}
        }
    }

    public void UseStamina(float value)
    {
        currentStamina -= value;
        if (currentStamina < 0) currentStamina = 0;

        NotifyPlayerObservers(AllPlayerActions.StaminaChanged);
        NotifyPlayerObservers(AllPlayerActions.LowStamina);
    }
    #endregion

    #region Damage
    private void TakeDamage(float damage)
    {
        if (isDead || !canBeDamaged) return;

        if (canBeStaggered)
        {
            canMove = false;
            //Invoke("ActivateMovement", damagedCooldown);
            //PlayAnimation(0, false, true); // has window time event ^^
        }

        currentHealth -= damage;

        _hitParticleEmission.enabled = true;
        Invoker.InvokeDelayed(DisableHitParticles, 0.1f);

        if (currentHealth <= 0) Die();
        else
        {
            //PlayAnimation(animationIDs[6]);
            PlaySound(hitClips);
            NotifyPlayerObservers(AllPlayerActions.LowHealth);
            canBeDamaged = false;
            Invoker.InvokeDelayed(ResetImmunity, iFrames);
        }
        PlayAnimation(8, true, true, true);
    }

    private void TakeDamage(float damage, float knockbackForce, Vector3 damagePos)
    {
        if (isDead || !canBeDamaged) return;

        rb.AddForce(-damagePos.normalized * knockbackForce, ForceMode.Impulse);
        TakeDamage(damage);
    }

    private void Die()
    {
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

    #endregion

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
        if (clip.Length > 0)
        {
            int random = Random.Range(0, clip.Length);
            AudioManager.instance.PlayCustomSFX(clip[random], audioSource);
        }
        else AudioManager.instance.PlayCustomSFX(clip[0], audioSource);
    }
    #endregion

    #region RotateHitbox
    public void RotateHitboxCentreToFaceTheDirection()
    {
        if (lastDirection == Vector3.zero) return;

        Vector3 direction = lastDirection.normalized;
        Vector3 desiredPosition = transform.position + direction * offset;
        Quaternion rotation = Quaternion.LookRotation(direction);
        hitboxCenter.rotation = rotation;
        hitboxCenter.position = new Vector3(desiredPosition.x, hitboxCenter.position.y, desiredPosition.z);
    }
    #endregion

    #region Sprite
    #region Flip Sprite Logic
    private void FlipSprite()
    {
        if (isRolling) return;
        if (direction.x < 0) isFacingLeft = false;
        else if (direction.x > 0) isFacingLeft = true;
        RotateSprite();
    }
    #endregion

    #region Rotate Sprite
    private void RotateSprite()
    {
        if (isFacingLeft) GetComponent<SpriteRenderer>().flipX = true;
        else GetComponent<SpriteRenderer>().flipX = false;
    }
    #endregion
    #endregion

    #region VFX
    private void SlashVFXProxy()
    {
        NormalSlashVFXController();
        ComboSlashVFXController();
    }

    private void NormalSlashVFXController()
    {
        if (!isNormalVFXPlaying) return;

        normalVfxTime += Time.deltaTime * vfxSpeed;

        if (normalVfxTime >= 1)
        {
            normalVfxTime = -1;
            isNormalVFXPlaying = false;
        }
    }

    private void ComboSlashVFXController()
    {
        if (!isComboVFXPlaying) return;

        comboVfxTime += Time.deltaTime * vfxSpeed;

        if (comboVfxTime >= 1)
        {
            comboVfxTime = -1;
            isComboVFXPlaying = false;
        }
    }
    #endregion

    #region Get - Set - Invoke - Proxys

    #region Get
    public float GetHealth() { return currentHealth; }
    public float GetStamina() { return currentStamina; }
    public float GetSpeed() { return speed; }
    public Vector2 GetDamage() { return attackDamage; }
    public string GetPlayerState() { return playerState; }
    public Vector3 GetLastDirection() { return lastDirection; }
    public bool IsNotHittingEnemy() { return isNotHitting; }
    public bool IsNotKillingEnemy() { return isNotKilling; }
    public bool GetImmunity() { return !canBeDamaged; }
    #endregion

    #region Set
    public void SetMovement(bool value) { canMove = value; }
    public void SetSpeed(float value) { speed = value; }
    public void SetDamage(Vector2 value) { attackDamage = value; }
    public void SetCanAttack(bool value) { canAttack = value; }
    public void SetMoneda(bool value) {monedaActive = value; }
    public void DrawAttackHitbox(bool value) { drawingAttackHitbox = value; }
    public void SetAttacking(bool value) { if (!isComboAttack) { isAttacking = value; } }
    public void SetAttackingOnCombo(bool value) { isAttacking = value; }
    public void SetCombo(bool value) { canCombo = value; }
    public void SetComboAttack(bool value) { isComboAttack = value; }
    public void SetCooldown(bool value) { if (!isComboAttack) { isOnCooldown = value; } }
    public void SetCooldownOnCombo(bool value) { isOnCooldown = value; }
    public void SetRoll(bool value) { isRolling = value; }
    public void SetImmunity(bool value) { canBeDamaged = !value; }
    public void SetParryParticles(bool value) { _parryParticleEmission.enabled = value; _parryParticleEmissionTwo.enabled = value; }
    public void SetHitParticles(bool value) { _hitParticleEmission.enabled = value; }
    public void ApplyForce(float value) { rb.AddForce(lastDirection.normalized * value, ForceMode.Impulse); }
    public void SetParryState(bool value) { if (value) playerState = "Parry"; else playerState = ""; }
    public void SetParryPressed(bool value) { wasParryPressed = value; }
    public void SetAbilityCooldown(bool value) { isAbilityOnCooldown = value; }


    public void SetParalisisStatus(bool status, float reset)
    {
        paralized = status;
        Invoke("ResetParalisis", reset);
    }

    public void SetRollQuantity(bool status, int value)
    {
        finiteRolling = status;
        rollAmmount = value;
    }

    public void MonedaUseItem(bool value)
    {
        if (monedaActive)
        {
            if (value) inventory.UseItem();
            else inventory.DeleteItem();
        }
    }
    #endregion

    #region Proxys
    public void FailParry() { NotifyPlayerObservers(AllPlayerActions.FailedParry); PlayAnimation(8, true, true, false);}
    public void TakeDamageProxy(float damage, float knockbackForce = 0, Vector3 damagePos = new Vector3()) { TakeDamage(damage, knockbackForce, damagePos);}
    public void GainHealthProxy(float healthReward) { GainHealth(healthReward);}
    public void GainStaminaProxy(float staminaReward) { GainStamina(staminaReward);}
    public void GetParryRewardProxy(EnemyType type, bool isProjectile = false) { GetParryReward(type, isProjectile);}
    #endregion

    #region Invoke
    public void ResetParalisis() {paralized = false;}
    public void DisableHitParticles() { _hitParticleEmission.enabled = false;}
    public void DisableParryParticles() { _parryParticleEmission.enabled = false; _parryParticleEmissionTwo.enabled = false; inpactVFX.SetFloat("_isOn", 0); }
    public void ResetImmunity() { canBeDamaged = true;}
    public void ResetAbilityCooldown() { isAbilityOnCooldown = false;}
    #endregion

    #endregion

    #endregion

    #region Debug
    private void DrawAttackHitbox()
    {
        if (lastDirection == Vector3.zero) VisualizeBox.DisplayBox(hitboxPos + hitboxCenter.position, hitboxSize, Quaternion.identity, attackHitboxColor);
        else VisualizeBox.DisplayBox(hitboxPos + hitboxCenter.position, hitboxSize, Quaternion.LookRotation(lastDirection), attackHitboxColor);
    }

    private void OnDrawGizmos()
    {
        if (drawHitbox)
        {
            if (drawHitboxOnGameplay) {if (drawingAttackHitbox) DrawAttackHitbox();}
            else DrawAttackHitbox();
        }
    }
    
    #endregion
}
