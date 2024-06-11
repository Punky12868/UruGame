using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBigEnemy : NewEnemyBase
{
    [Header("BE Combat")]
    [SerializeField] protected bool canBeParried = true;
    [SerializeField] protected float chargeAttackDamage = 5;
    [SerializeField] protected float chargeAttackKnockback = 15;
    [SerializeField] protected bool canParryChargeAttack;
    [SerializeField] protected float chargeDecitionCooldown = 2.5f;

    [Header("BE C_Attack Odds")]
    public int maxOdds = 1000;
    public int oddsToChargeAttack = 250;

    [Header("BE Hitbox")]
    [SerializeField] protected Transform hitboxCenter;
    [SerializeField] protected Vector3 attackHitboxSize = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] protected Vector3 chargedAttackHitboxSize = new Vector3(1, 1, 1);
    [SerializeField] protected float hitboxOffset;

    [Header("BE Ranges")]
    [SerializeField] protected float closeAttackRange = 0.8f;
    [SerializeField] protected float farAttackRange = 2;
}
