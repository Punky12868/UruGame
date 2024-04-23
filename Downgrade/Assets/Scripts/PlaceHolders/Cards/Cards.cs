using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Cards : MonoBehaviour
{
    GameObject cardHolder;
    [SerializeField] private DowngradeCard dgCard;

    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text cardDescription;
    [SerializeField] private Image cardSprite;

    private void Awake()
    {
        cardHolder = GameObject.FindGameObjectWithTag("CardsHolder");

        cardName.text = dgCard.cardName;
        cardDescription.text = dgCard.cardDescription;
        cardSprite.sprite = dgCard.cardSprite;
    }

    public void UseCard()
    {
        cardHolder.SetActive(false);
        //FindObjectOfType<PlayerController>().SetCanMove(true);
        dgCard.CardEffect();

        int cardAmount = cardHolder.transform.childCount;

        for (int i = 0; i < cardAmount; i++)
        {
            if (cardHolder.transform.GetChild(i).GetComponent<Cards>() != this)
            {
                Destroy(cardHolder.transform.GetChild(i).GetComponent<Cards>().gameObject);
            }
        }
        Destroy(gameObject);
        //PauseGame.Resume();

        //FindObjectOfType<Damage>().OnDamage();
        //FindObjectOfType<RestartController>().SaveProgress();
    }
}
