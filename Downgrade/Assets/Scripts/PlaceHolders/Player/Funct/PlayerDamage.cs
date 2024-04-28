using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : PlayerBase
{
    public override void TakeDamage(float damage, float knockbackForce, Vector3 damagePos)
    {
        if (isDead || !canBeDamaged)
            return;

        rb.AddForce(-damagePos.normalized * knockbackForce, ForceMode.Impulse);

        if (canBeStaggered)
        {
            canMove = false;
            Invoke("ActivateMovement", damagedCooldown);
            PlayAnimation(animationIDs[0], false, true);
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            PlaySound(hitClips);
            NotifyPlayerObservers(AllPlayerActions.LowHealth);
        }
    }

    public override void Die()
    {
        //Destroy(gameObject);
        Debug.Log("Dead");
        isDead = true;
        playerState = "Dead";

        direction = Vector3.zero;
        lastDirection = Vector3.zero;
        PlaySound(deathClips);
        NotifyPlayerObservers(AllPlayerActions.Die);
        FindObjectOfType<TextScreens>().OnDeath();
    }
}
