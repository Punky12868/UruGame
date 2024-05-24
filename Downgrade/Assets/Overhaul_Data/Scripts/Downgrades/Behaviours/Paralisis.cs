using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Paralisis_Dg", menuName = "Downgrade/Paralisis")]
public class Paralisis : DowngradeCard
{
    public float damage;
    public float time;
    public override void CardEffect()
    {
        DowngradeSystem.Instance.SetParalisisDg(damage, time, dgIcon);
    }
}
