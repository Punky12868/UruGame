using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRangedEnemy : NewEnemyBase
{
    [Header("RE Type")]
    [SerializeField] protected bool isStatic;

    [Header("RE Projectile")]
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected Transform projectileSpawnPoint;

    [Header("RE Combat")]
    [SerializeField] protected bool projectileCanBeParried = false;
    [SerializeField] protected float projectileLifeTime;
    [SerializeField] protected float projectileSpeed;
}
