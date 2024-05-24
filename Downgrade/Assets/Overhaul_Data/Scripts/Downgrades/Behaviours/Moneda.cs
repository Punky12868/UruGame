using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Moneda_Dg", menuName = "Downgrade/Moneda")]
public class Moneda : DowngradeCard
{
    public float healthLossPercentage = 10;
    public override void CardEffect()
    {
        DowngradeSystem.Instance.SetMonedaDg(healthLossPercentage, dgIcon);
    }
}
