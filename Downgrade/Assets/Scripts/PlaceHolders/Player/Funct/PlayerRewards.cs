using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRewards : PlayerBase
{
    public override void GetHealth(float healthReward)
    {
        currentHealth += healthReward;

        if (currentHealth > health)
            currentHealth = health;
    }

    public override void GetStamina(float staminaReward)
    {
        currentStamina += staminaReward;

        if (currentStamina > stamina)
            currentStamina = stamina;
    }

    public override void GetParryReward(bool isBigEnemy, bool isBoss)
    {
        if (isBigEnemy)
        {
            GetStamina(staminaBigEnemyReward);
            GetHealth(healthBigEnemyReward);
        }
        else if (isBoss)
        {
            GetStamina(staminaBossReward);
            GetHealth(healthBossReward);
            // damage multiplier
        }
        else
        {
            GetStamina(staminaNormalReward);
        }
    }
}
