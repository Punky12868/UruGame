using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[RequireComponent(typeof(PlayerAnims))]
[RequireComponent(typeof(PlayerSounds))]
[RequireComponent(typeof(PlayerUtility))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(PlayerRewards))]
[RequireComponent(typeof(PlayerDamage))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerActions))]
[RequireComponent(typeof(PlayerInteraction))]
[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(PlayerUI))]

public class PlayerComponents : PlayerBase
{
    public override void Awake()
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

    public override void Update()
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

    public override void FixedUpdate()
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
}
