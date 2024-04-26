using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Cards : MonoBehaviour
{
    [SerializeField] private DowngradeCard dgCard;

    [SerializeField] private GameObject cardObject;

    [SerializeField] private bool hasTextEffect;
    [SerializeField] private string cardNameTextEffect;
    [SerializeField] private string cardStatTextEffect;
    [SerializeField] private string cardDescriptionTextEffect;

    [SerializeField] private CanvasGroup cardTextPanel;
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text cardStat;
    [SerializeField] private TMP_Text cardDescription;
    [SerializeField] private Image cardImage;

    [SerializeField] Transform endPos;
    private Vector3 end;
    private Vector3 target;

    [SerializeField] private float moveDuration;
    [SerializeField] private float rotationDuration;
    [SerializeField] private float fadeDuration;

    [SerializeField] private int cardMusic;
    [SerializeField] private Vector2 wiggleEffect;
    [SerializeField] private float wiggleResetTime;
    [SerializeField] private Ease easeType;

    bool wiggle;
    float customTime;

    private void Awake()
    {
        if (cardTextPanel.alpha != 0)
            cardTextPanel.alpha = 0;
    }

    private void Update()
    {
        if (wiggle)
        {
            customTime += Time.unscaledDeltaTime;

            if (customTime >= wiggleResetTime)
            {
                Vector3 newPos = target + new Vector3(Random.Range(wiggleEffect.x, wiggleEffect.y), Random.Range(wiggleEffect.x, wiggleEffect.y), 0);
                cardObject.transform.DOMove(newPos, wiggleResetTime * 3).SetEase(easeType).SetUpdate(UpdateType.Normal, true);
                customTime = 0;
            }
        }
    }

    public void SetDgCard(DowngradeCard dg)
    {
        dgCard = dg;
    }

    public void SetCard()
    {
        cardImage.sprite = dgCard.cardBackSprite;

        if (hasTextEffect)
        {
            cardName.text = "<" + cardNameTextEffect + ">" + dgCard.cardName;
            cardStat.text = "<" + cardStatTextEffect + ">" + dgCard.cardStat;
            cardDescription.text = "<" + cardDescriptionTextEffect + ">" + dgCard.cardDescription;
        }
        else
        {
            cardName.text = dgCard.cardName;
            cardStat.text = dgCard.cardStat;
            cardDescription.text = dgCard.cardDescription;
        }
    }

    public void SetTarget(Vector3 target)
    {
        this.target = target;
        cardObject.transform.DOMove(target, moveDuration).SetEase(Ease.OutBounce).SetUpdate(UpdateType.Normal, true);
        Invoker.InvokeDelayed(ChangeTarget, moveDuration);
    }

    public void SetCardObjPos()
    {
        cardObject.transform.position += new Vector3(0, 500, 0);
    }

    public void UseCard()
    {
        dgCard.CardEffect();
        GameManager.Instance.PauseGame(false, false);
        GameManager.Instance.LoadNextScene();
        AudioManager.instance.PlayMusic(cardMusic);
        DowngradeSystem.Instance.SetDowngrade(dgCard.selectedDowngrade);

        /*Cards[] cards = FindObjectsOfType<Cards>();
        foreach (Cards card in cards)
        {
            Destroy(card.gameObject);
        }*/
    }

    private void ChangeTarget()
    {
        end = endPos.position;
        target = end;
        cardObject.transform.DOMove(target, moveDuration).SetEase(Ease.OutBounce).SetUpdate(UpdateType.Normal, true);

        cardTextPanel.DOFade(1, fadeDuration).SetUpdate(UpdateType.Normal, true);
        StartCoroutine(RotateCard());
    }

    IEnumerator RotateCard()
    {
        float elapsedTime = 0;

        Quaternion startRotation = Quaternion.Euler(0, 180, 0);
        Quaternion targetRotation = Quaternion.Euler(0, 0, 0); // Adjust the target rotation as needed

        while (elapsedTime < rotationDuration)
        {
            float t = elapsedTime / rotationDuration;
            cardObject.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            // Update the elapsed time using unscaledDeltaTime
            elapsedTime += Time.unscaledDeltaTime;

            if (t >= 0.5f) // Change the condition based on your requirement
            {
                cardImage.sprite = dgCard.cardSprite;
            }

            yield return null;
        }

        cardObject.GetComponent<Button>().interactable = true;

        wiggle = true;
    }
}
