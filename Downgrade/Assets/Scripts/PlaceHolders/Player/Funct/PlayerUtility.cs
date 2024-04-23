using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUtility : PlayerBase
{
    #region Utility
    public override void RotateHitboxCentreToFaceTheDirection()
    {
        if (lastDirection == Vector3.zero)
            return;

        Vector3 direction = lastDirection.normalized;
        Vector3 desiredPosition = transform.position + direction * offset;
        Quaternion rotation = Quaternion.LookRotation(direction);
        hitboxCenter.rotation = rotation;
        hitboxCenter.position = new Vector3(desiredPosition.x, hitboxCenter.position.y, desiredPosition.z);
    }

    public override float GetCurrentHealth()
    {
        return currentHealth;
    }

    public override float GetCurrentStamina()
    {
        return currentStamina;
    }

    public override string GetPlayerState()
    {
        return playerState;
    }

    public override Vector3 GetLastDirection()
    {
        return lastDirection;
    }
    #endregion

    #region Debug
    public override void DrawAttackHitbox()
    {
        if (lastDirection == Vector3.zero)
        {
            VisualizeBox.DisplayBox(hitboxPos + hitboxCenter.position, hitboxSize, Quaternion.identity, attackHitboxColor);
        }
        else
        {
            VisualizeBox.DisplayBox(hitboxPos + hitboxCenter.position, hitboxSize, Quaternion.LookRotation(lastDirection), attackHitboxColor);
        }
    }

    public override void OnDrawGizmos()
    {
        if (drawHitbox)
        {
            if (drawHitboxOnGameplay)
            {
                if (drawingAttackHitbox)
                    DrawAttackHitbox();
            }
            else
            {
                DrawAttackHitbox();
            }
        }
    }
    #endregion
}
