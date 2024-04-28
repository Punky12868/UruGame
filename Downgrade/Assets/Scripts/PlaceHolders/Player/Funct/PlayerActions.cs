using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : PlayerBase
{
    public override void Inputs()
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

        if (input.GetButtonDown("UseItem"))
        {
            inventory.UseItem();
        }

        if (input.GetButtonDown("DropItem"))
        {
            inventory.DropItem();
        }
    }

    public override void Roll()
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
        NotifyPlayerObservers(AllPlayerActions.LowStamina);
        isRolling = true;
        canBeDamaged = false;
        PlaySound(rollClips);
        Invoke("ResetDamage", rollInmunity);
        Invoke("ResetRoll", rollCooldown);
    }
}
