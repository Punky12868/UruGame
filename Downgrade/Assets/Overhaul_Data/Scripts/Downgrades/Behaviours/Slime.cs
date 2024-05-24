using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Slime_Dg", menuName = "Downgrade/Slime")]
public class Slime : DowngradeCard
{
    public float speedAmmount = 1.5f;
    public float time = 1;
    public override void CardEffect()
    {
        DowngradeSystem.Instance.SetSlimeDg(speedAmmount, time, dgIcon);
    }
}
