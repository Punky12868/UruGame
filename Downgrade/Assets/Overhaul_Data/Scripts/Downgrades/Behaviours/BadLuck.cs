using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "5050_Dg", menuName = "Downgrade/5050")]
public class BadLuck : DowngradeCard
{
    public float healthLossPercentage = 10;
    public override void CardEffect()
    {
        DowngradeSystem.Instance.SetBadLuckDg(healthLossPercentage, dgIcon);
    }
}
