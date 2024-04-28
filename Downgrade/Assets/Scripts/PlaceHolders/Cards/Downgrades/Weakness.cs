using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weakness_Dg", menuName = "Downgrade/Weakness")]
public class Weakness : DowngradeCard
{
    public Vector2 damageAmmount = new Vector2(20, 30);
    public float time = 1f;
    public override void CardEffect()
    {
        DowngradeSystem.Instance.SetWeaknessDg(damageAmmount, time, dgIcon);
    }
}

