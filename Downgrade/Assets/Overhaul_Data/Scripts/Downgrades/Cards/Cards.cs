using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using EasyTransition;
using UnityEngine;
using DG.Tweening;
using TMPro;

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
    [SerializeField] private float wiggleEffectMultiplier;
    [SerializeField] private float growAmmountWhenSelected;
    [SerializeField] private float growSpeed;
    [SerializeField] private float wiggleResetTime;
    [SerializeField] private Ease easeType;

    [SerializeField] private Ease rotEaseType;
    [SerializeField] private float rotateCardTimeout;
    [SerializeField] private Vector2 cardRotationAmmount;
    [SerializeField] private float cardRotationMultiplier;
    [SerializeField] private float cardRotationSpeed;
    [SerializeField] private float cardRotationSpeedMultiplier;

    [SerializeField] private bool loadSpecificScene;
    [SerializeField] private int loadSceneIndex;
    [SerializeField] private TransitionSettings transitionSettings;
    [SerializeField] private float transitionDelay;

    bool wiggle;
    bool isSelected;
    float customTime;
    float rotateCardTime;

    private void Awake()
    {
        if (cardTextPanel.alpha != 0)
            cardTextPanel.alpha = 0;

        if (growAmmountWhenSelected < 1) growAmmountWhenSelected = 1;
        Debug.Log(SimpleSaveLoad.Instance.LoadData<SelectedDowngrade>(FileType.Gameplay, "Downgrade", SelectedDowngrade.None));
    }

    private void Update()
    {
        isSelected = EventSystem.current.currentSelectedGameObject == cardObject;

        if (wiggle)
        {
            customTime += Time.unscaledDeltaTime;

            if (customTime >= wiggleResetTime)
            {
                Vector3 newPos = target;

                /*if (!isSelected)
                {
                    newPos = target + new Vector3(Random.Range(wiggleEffect.x, wiggleEffect.y), Random.Range(wiggleEffect.x, wiggleEffect.y), 0);
                }
                else
                {
                    newPos = target + new Vector3(Random.Range(wiggleEffect.x * wiggleEffectMultiplier, wiggleEffect.y * wiggleEffectMultiplier), Random.Range(wiggleEffect.x * wiggleEffectMultiplier, wiggleEffect.y * wiggleEffectMultiplier), 0);
                }*/

                cardObject.transform.DOMove(newPos, wiggleResetTime * 3).SetEase(easeType).SetUpdate(UpdateType.Normal, true);
                customTime = 0;
            }
        }

        if (isSelected) cardObject.transform.localScale = Vector3.Lerp(cardObject.transform.localScale, new Vector3(growAmmountWhenSelected, growAmmountWhenSelected, growAmmountWhenSelected), Time.unscaledDeltaTime * growSpeed);  
        else cardObject.transform.localScale = Vector3.Lerp(cardObject.transform.localScale, new Vector3(1, 1, 1), Time.unscaledDeltaTime * growSpeed);

        /*if (rotateCardTime >= rotateCardTimeout)
        {
            SetCardRotation();
            rotateCardTime = 0;
        }
        else
        {
            rotateCardTime += Time.unscaledDeltaTime;
        }*/
    }

    public void SetDgCard(DowngradeCard dg)
    {
        dgCard = dg;
        
        //FindObjectOfType<SimpleSaveLoad>().LoadData<DowngradeCard>(FileType.Gameplay, "Downgrade");
        SpriteState Ss = new SpriteState();
        Ss.highlightedSprite = dgCard.cardSelectedSprite;
        Ss.selectedSprite = dgCard.cardSelectedSprite;
        Ss.pressedSprite = dgCard.cardSelectedSprite;
        //Ss.disabledSprite = dgCard.cardSprite;
        GetComponentInChildren<Button>().spriteState = Ss;
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

    private void SetCardRotation()
    {
        if (!isSelected)
        {
            cardObject.transform.DORotate(new Vector3(0, 0, Random.Range(cardRotationAmmount.x, cardRotationAmmount.y)), cardRotationSpeed).SetEase(rotEaseType).SetLoops(2, LoopType.Yoyo).SetUpdate(UpdateType.Normal, true);

        }
        else
        {
            cardObject.transform.DORotate(new Vector3(0, 0, Random.Range(cardRotationAmmount.x, cardRotationAmmount.y) * cardRotationMultiplier), cardRotationSpeed / cardRotationSpeedMultiplier).SetEase(rotEaseType).SetLoops(2, LoopType.Yoyo).SetUpdate(UpdateType.Normal, true);
        }
    }

    public void UseCard()
    {
        dgCard.CardEffect();
        
        GameManager.Instance.PauseGame(false, false);
        //GameManager.Instance.LoadNextScene();
        AudioManager.instance.PlayMusic(cardMusic);
        DowngradeSystem.Instance.SetDowngrade(dgCard.selectedDowngrade);
        if (loadSpecificScene) TransitionManager.Instance().Transition(loadSceneIndex, transitionSettings, transitionDelay);
        else TransitionManager.Instance().Transition(SceneManager.GetActiveScene().buildIndex + 1, transitionSettings, transitionDelay);
        //FindObjectOfType<SimpleSaveLoad>().SaveData<SelectedDowngrade>(FileType.Gameplay, "Downgrade", dgCard.selectedDowngrade);
        //dgCard = FindObjectOfType<SimpleSaveLoad>().LoadData<DowngradeCard>(FileType.Gameplay, "Downgrade");


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
