using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsAnim : MonoBehaviour
{
    [SerializeField] private List<GameObject> cards;
    [SerializeField] private List<GameObject> cardTexts;

    public void AddCard(GameObject card, GameObject textPanel)
    {
        cards.Add(card);
        cardTexts.Add(textPanel);
    }

    public void RemoveCards()
    {
        cards.Clear();
        cardTexts.Clear();
    }

    private void Update()
    {
        if (cards.Count > 0)
        {
            for (int i = 0; i < cards.Count; i++)
            {
            }
        }
    }
}
