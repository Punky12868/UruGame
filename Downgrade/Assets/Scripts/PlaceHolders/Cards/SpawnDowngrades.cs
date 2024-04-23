using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDowngrades : MonoBehaviour
{
    [SerializeField] private int ammountToSpawn = 3;
    [SerializeField] private GameObject cardsObject;
    [SerializeField] private List<DowngradeCard> downgradePoll = new List<DowngradeCard>();

    List<DowngradeCard> Alldowngrades = new List<DowngradeCard>();

    public void SpawnCards()
    {
        Alldowngrades = downgradePoll;

        for (int i = 0; i < ammountToSpawn; i++)
        {
            int selectedDg = Random.Range(0, Alldowngrades.Count);

            GameObject card = Instantiate(cardsObject, transform);
            card.GetComponent<Cards>().SetDgCard(Alldowngrades[selectedDg]);
            card.GetComponent<Cards>().SetCard();
            Alldowngrades.RemoveAt(selectedDg);
        }
    }
}
