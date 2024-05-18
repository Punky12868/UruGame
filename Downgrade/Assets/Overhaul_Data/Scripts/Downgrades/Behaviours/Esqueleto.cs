using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Esqueleto_Dg", menuName = "Downgrade/Esqueleto")]
public class Esqueleto : DowngradeCard
{
    public int damageAmmount = 1;
    public float timeThresshold = 1;
    public override void CardEffect()
    {
        DowngradeSystem.Instance.SetEnemyBoostDg(damageAmmount, timeThresshold, dgIcon);
    }
}
