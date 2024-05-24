using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rodillas_Dg", menuName = "Downgrade/Rodillas")]
public class Rodillas : DowngradeCard
{
    public float staminaThresshold = 5f;
    public float healthLossPercentage = 10;
    public override void CardEffect()
    {
        DowngradeSystem.Instance.SetRodillasDg(staminaThresshold, healthLossPercentage, dgIcon);
    }
}
