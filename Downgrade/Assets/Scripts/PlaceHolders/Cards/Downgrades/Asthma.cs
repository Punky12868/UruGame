using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Asthma_Dg", menuName = "Downgrade/Asthma")]
public class Asthma : DowngradeCard
{
    public float staminaThresshold = 5f;
    public float healthLossPercentage = 10;
    public override void CardEffect()
    {
        DowngradeSystem.Instance.SetAsthmaDg(staminaThresshold, healthLossPercentage, dgIcon);
    }
}
