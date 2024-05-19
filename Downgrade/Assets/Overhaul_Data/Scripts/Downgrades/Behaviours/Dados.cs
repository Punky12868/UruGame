using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dados_Dg", menuName = "Downgrade/Dados")]
public class Dados : DowngradeCard
{
    public int minRolls; public int maxRolls;
    public override void CardEffect()
    {
        int randomRolls = Random.Range(minRolls, maxRolls + 1);
        DowngradeSystem.Instance.SetDadosDg(randomRolls, dgIcon);
    }
}
