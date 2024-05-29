using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnDowngrades : MonoBehaviour
{
    [SerializeField] private int ammountToSpawn = 3;
    [SerializeField] private GameObject cardsObject;

    [SerializeField] GameObject downgradesMenu;
    [SerializeField] private Transform cardsMidPos;
    [SerializeField] private Transform cardsStartPos;
    [SerializeField] private float fadeDuration;
    [SerializeField] private bool spawnOnAwake;
    

    [SerializeField] private List<DowngradeCard> downgradePoll = new List<DowngradeCard>();

    List<DowngradeCard> Alldowngrades = new List<DowngradeCard>();

    private void Awake()
    {
        if (spawnOnAwake) Invoke("SpawnCards", 0.1f);
    }

    public void SpawnCards()
    {
        GameManager.Instance.PauseGame(true, true);
        Alldowngrades = downgradePoll;

        downgradesMenu.GetComponent<CanvasGroup>().DOFade(1, fadeDuration).SetUpdate(UpdateType.Normal, true);

        for (int i = 0; i < ammountToSpawn; i++)
        {
            int selectedDg = Random.Range(0, Alldowngrades.Count);
            Debug.Log(Alldowngrades.Count);

            GameObject card = Instantiate(cardsObject, downgradesMenu.transform);

            card.GetComponent<Cards>().SetCardObjPos();
            card.GetComponent<Cards>().SetTarget(cardsMidPos.position);
            card.GetComponent<Cards>().SetDgCard(Alldowngrades[selectedDg]);
            card.GetComponent<Cards>().SetCard();
            Alldowngrades.RemoveAt(selectedDg);

            if (i == 1)
            {
                card.GetComponentInChildren<Button>().Select();
            }

            card.SetActive(true);
            card.GetComponent<CanvasGroup>().DOFade(1, fadeDuration).SetUpdate(UpdateType.Normal, true);
        }
    }

    private void Update()
    {
        downgradesMenu.SetActive(GameManager.Instance.IsSelectingDowngrade());

        //if (Time.timeScale == 0) Time.timeScale = 1;
    }
}
