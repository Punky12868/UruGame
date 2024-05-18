using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Debil_Dg", menuName = "Downgrade/Debil")]
public class Debil : DowngradeCard
{
    public Vector2 damageAmmount = new Vector2(20, 30);
    public float time = 1f;
    public override void CardEffect()
    {
        DowngradeSystem.Instance.SetWeaknessDg(damageAmmount, time, dgIcon);
    }
}

