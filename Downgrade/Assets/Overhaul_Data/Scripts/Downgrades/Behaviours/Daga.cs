using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Daga_Dg", menuName = "Downgrade/Daga")]
public class Daga : DowngradeCard
{
    public override void CardEffect()
    {
        DowngradeSystem.Instance.SetDagaDg(dgIcon);
    }
}
