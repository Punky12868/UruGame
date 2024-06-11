using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSmallEnemy : NewEnemyBase
{
    [Header("SE Hitbox")]
    [SerializeField] protected Transform hitboxCenter;
    [SerializeField] protected Vector3 attackHitboxSize = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] protected float hitboxOffset;

    [Header("SE Ranges")]
    [SerializeField] protected float closeAttackRange = 0.8f;

    #region Utility
    protected void HitboxFaceToTarget()
    {
        if (isAttacking) return;

        Vector3 direction = (target.position - transform.position).normalized * hitboxOffset;
        Vector3 desiredPosition = transform.position + direction;
        Quaternion rotation = Quaternion.LookRotation(direction);
        hitboxCenter.rotation = rotation;
        hitboxCenter.position = new Vector3(desiredPosition.x, hitboxCenter.position.y, desiredPosition.z);
    }

    #endregion
}
