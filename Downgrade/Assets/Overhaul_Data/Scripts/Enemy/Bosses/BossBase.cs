using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class BossBase : Subject
{
    #region Variables
    [HideInInspector] protected Animator anim;
    [HideInInspector] protected Rigidbody rb;
    [HideInInspector] protected Transform pivot;
    [HideInInspector] protected Transform target;
    [HideInInspector] protected Vector3 lastTargetDir;
    [HideInInspector] protected Vector3 targetDir;
    [HideInInspector] protected ParticleSystem.EmissionModule _particleEmission;

    [HideInInspector] protected float animClipLength;
    [HideInInspector] protected bool isAnimationDone;

    [HideInInspector] protected bool isMoving;
    [HideInInspector] protected bool isAttacking;
    [HideInInspector] protected string attackType;
    [HideInInspector] protected bool isSummoningObjects;
    [HideInInspector] protected bool isSpriteFlipped;
    [HideInInspector] protected bool hasQueuedAnimation;

    [Header("Boss General Stats")]
    public string bossName;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float ammountOfFases;

    [Header("Health")]
    public float health;
    protected float currentHealth;

    [Header("Attack")]
    public AttackBase[] attacks;

    [Header("Dialogs"), TextArea]
    public string[] dialogLines;
    #endregion
}
