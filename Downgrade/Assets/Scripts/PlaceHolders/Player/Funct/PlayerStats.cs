using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : PlayerBase
{
    public override void Stamina()
    {
        if (isRolling || isAttacking)
        {
            staminaTimer = 0;
        }
        else
        {
            if (staminaTimer < staminaCooldown)
            {
                staminaTimer += Time.deltaTime;
            }
            else
            {
                if (currentStamina < stamina)
                {
                    currentStamina += staminaRegenSpeed * Time.deltaTime;
                }
            }
        }
    }
}
