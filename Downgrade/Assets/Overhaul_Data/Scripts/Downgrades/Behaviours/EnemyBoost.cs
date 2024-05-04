using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBoost_Dg", menuName = "Downgrade/EnemyBoost")]
public class EnemyBoost : DowngradeCard
{
    public int damageAmmount = 1;
    public float timeThresshold = 1;
    public override void CardEffect()
    {
        DowngradeSystem.Instance.SetEnemyBoostDg(damageAmmount, timeThresshold, dgIcon);
    }
}
