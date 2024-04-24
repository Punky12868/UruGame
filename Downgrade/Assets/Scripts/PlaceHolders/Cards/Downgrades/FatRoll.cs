using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FatRoll_Dg", menuName = "Downgrade/FatRoll")]
public class FatRoll : DowngradeCard
{
    public float speedAmmount = 1.5f;
    public float time = 1;
    public override void CardEffect()
    {
        DowngradeSystem.Instance.SetFatRollDg(speedAmmount, time, dgIcon);
    }
}
