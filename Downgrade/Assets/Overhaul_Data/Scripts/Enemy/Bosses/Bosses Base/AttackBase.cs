using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBase : ScriptableObject
{
    public string attackClipId;
    public string attackName;
    public float damage;
    public float range;
    public float cooldown;
    public float duration;
    public float knockback;
    public bool canPlayerStun;

    public virtual void Attack(Transform target, Transform pivot, Animator anim, Rigidbody rb)
    {
    }
}
