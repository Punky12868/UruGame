using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Cards : MonoBehaviour
{
    [SerializeField] private DowngradeCard dgCard;

    [SerializeField] private GameObject cardObject;
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text cardStat;
    [SerializeField] private TMP_Text cardDescription;

    public void SetDgCard(DowngradeCard dg)
    {
        dgCard = dg;
    }

    public void SetCard()
    {
        cardObject = dgCard.cardObject;
        cardName.text = dgCard.cardName;
        cardStat.text = dgCard.cardStat;
        cardDescription.text = dgCard.cardDescription;
    }

    public void UseCard()
    {
        //FindObjectOfType<PlayerController>().SetCanMove(true);
        dgCard.CardEffect();
        Destroy(gameObject);
        //PauseGame.Resume();

        //FindObjectOfType<Damage>().OnDamage();
        //FindObjectOfType<RestartController>().SaveProgress();
    }
}
