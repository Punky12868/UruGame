using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stamina_Dg", menuName = "Downgrade/Stamina")]
public class StaminaDg : DowngradeCard
{
    public float time;
    public override void CardEffect()
    {
        DowngradeSystem.Instance.SetStaminaDg(time, dgIcon);
    }
}
