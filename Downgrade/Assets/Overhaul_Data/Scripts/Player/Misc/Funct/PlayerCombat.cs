using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : PlayerBase
{
    #region Attack
    public override void OverlapAttack()
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
        NotifyPlayerObservers(AllPlayerActions.LowStamina);
        PlaySound(attackClips);
        Collider[] hitColliders = Physics.OverlapBox(hitboxCenter.position, hitboxSize, Quaternion.LookRotation(lastDirection));

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log("Hit");
                hit.GetComponent<OldEnemyBase>().TakeDamage(attackDamage);
            }

            if (hit.CompareTag("Destructible"))
            {
                hit.GetComponent<Prop>().OnHit();
            }
        }
    }
    #endregion

    #region VFX
    public override void NormalSlashVFXController()
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

    public override void ComboSlashVFXController()
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
    public override void ParryLogic()
    {
        if (!wasParryInvoked)
        {
            wasParryInvoked = true;
            Invoke("ResetParry", parryWindowTime.y);
        }
        PlayAnimation(animationIDs[9], true); // Parry
    }
    #endregion
}
