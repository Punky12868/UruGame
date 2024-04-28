using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerBase : Subject
{
    // PlaceHolder for the PlayerController
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
    public bool drawHitbox = true;
    public bool drawHitboxOnGameplay = true;
    public float attackHitboxTime = 0.2f;
    public Color attackHitboxColor = new Color(1, 0, 0, 1);
    public Color parryHitboxColor = new Color(0, 1, 0, 1);

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

    #region Misc
    public virtual void Awake()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void Inputs()
    {
    }

    public virtual void Stamina()
    {
    }

    public virtual void Roll()
    {
    }
    #endregion

    #region Damage
    public virtual void TakeDamage(float damage, float knockbackForce, Vector3 damagePos)
    {
    }

    public virtual void Die()
    {
    }
    #endregion

    #region Rewards
    public virtual void GetHealth(float healthReward)
    {
    }

    public virtual void GetStamina(float staminaReward)
    {
    }

    public virtual void GetParryReward(bool isBigEnemy, bool isBoss)
    {
    }
    #endregion

    #region Invokes
    public virtual void ActivateMovement()
    {
        canMove = true;
    }

    public virtual void ActivateParry()
    {
        isParrying = true;
        playerState = "Parry";
    }

    public virtual void ResetParry()
    {
        isParrying = false;
        wasParryInvoked = false;
        wasParryPressed = false;
        playerState = "";
    }

    public virtual void DrawingAttackHitbox()
    {
        drawingAttackHitbox = false;
    }

    public virtual void ActivateCombo()
    {
        canCombo = true;
    }

    public virtual void ResetCombo()
    {
        canCombo = false;
    }

    public virtual void ActivateCooldown()
    {
        isOnCooldown = true;
    }

    public virtual void CooldownUpdate()
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

    public virtual void ResetCooldown(float time)
    {
        comboTime = time;
    }

    public virtual void ResetRoll()
    {
        isRolling = false;
    }

    public virtual void ResetDamage()
    {
        canBeDamaged = true;
    }
    #endregion

    #region Combat
    public virtual void OverlapAttack()
    {
    }
    #endregion

    #region VFX
    public virtual void NormalSlashVFXController()
    {
    }
    public virtual void ComboSlashVFXController()
    {
    }
    #endregion

    #region Parry
    public virtual void ParryLogic()
    {
    }
    #endregion

    #region Animation
    public virtual void PlayerAnimations()
    {
    }

    public virtual void PlayAnimation(string animName)
    {
    }

    public virtual void PlayAnimation(string animName, bool hasExitTime)
    {
    }

    public virtual void PlayAnimation(string animName, bool hasExitTime, bool bypassExitTime)
    {
    }

    public virtual void ResetAnimClipUpdate()
    {
    }
    #endregion

    #region Sound
    public virtual void PlaySound(AudioClip[] clip)
    {
    }
    #endregion

    #region Utility
    public virtual void RotateHitboxCentreToFaceTheDirection()
    {
    }

    public virtual float GetCurrentHealth()
    {
        return currentHealth;
    }

    public virtual float GetCurrentStamina()
    {
        return currentStamina;
    }

    public virtual string GetPlayerState()
    {
        return playerState;
    }

    public virtual Vector3 GetLastDirection()
    {
        return lastDirection;
    }
    #endregion

    #region Debug
    public virtual void DrawAttackHitbox()
    {
    }

    public virtual void OnDrawGizmos()
    {
    }
    #endregion
}
