using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChapterSelector : MonoBehaviour
{
    Player Player;

    [SerializeField] private Transform target;
    [SerializeField] private float duration = 1;
    [SerializeField] private Ease easeType = Ease.InOutExpo;
    [SerializeField] private int maxChapters = 4;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Button[] chaptersFirstButton;

    [SerializeField] private Button[] chapterIButtons;
    [SerializeField] private Button[] chapterIIButtons;
    [SerializeField] private Button[] chapterIIIButtons;
    [SerializeField] private Button[] chapterIVButtons;

    int currentChapter = 0;
    bool isOnTransition = false;

    bool _active = false;
    public bool Active
    {
        get { return _active; }
        set
        {
            if (_active != value)
            {
                _active = value;
                OnActiveChanged?.Invoke(_active);
            }
        }
    }

    public delegate void ActiveChanged(bool newValue);
    public event ActiveChanged OnActiveChanged;

    private void Awake() 
    {
        Player = ReInput.players.GetPlayer(0); 
        maxChapters--;
        OnActiveChanged += ActivateChapter;
        OnActiveChanged += ActivateMainMenuButtons;
    }
    private void Update() 
    {
        Active = FindObjectOfType<MainMenuTransition>().GetListening();
        Inputs(); 
    }

    private void Inputs()
    {
        if (!Active && currentChapter != 0) ResetChapter();
        if (!Active || isOnTransition) return;
        if (Player.GetButtonDown("RightTrigger")) MoveChapters(true);
        if (Player.GetButtonDown("LeftTrigger")) MoveChapters(false);
    }

    private void MoveChapters(bool value)
    {
        if (value && currentChapter < maxChapters)
        {
            currentChapter++;
            ActivateChapter(true);
            isOnTransition = true;
            Invoker.InvokeDelayed(ResetOnTransition, duration);
            target.DOLocalMoveX(target.localPosition.x - 1920, duration).SetEase(easeType);
        }
        else if (!value && currentChapter > 0)
        {
            currentChapter--;
            ActivateChapter(true);
            isOnTransition = true;
            Invoker.InvokeDelayed(ResetOnTransition, duration);
            target.DOLocalMoveX(target.localPosition.x + 1920, duration).SetEase(easeType);
        }
    }

    private void ActivateChapter(bool isActive)
    {
        if (!isActive) return;

        DeactivateAllChaptersExcept(currentChapter);
        switch (currentChapter)
        {
            case 0:
                SetButtonsInteractable(chapterIButtons, true);
                break;
            case 1:
                SetButtonsInteractable(chapterIIButtons, true);
                break;
            case 2:
                SetButtonsInteractable(chapterIIIButtons, true);
                break;
            case 3:
                SetButtonsInteractable(chapterIVButtons, true);
                break;
        }
    }

    private void DeactivateAllChaptersExcept(int activeChapterIndex)
    {
        if (activeChapterIndex != 0) SetButtonsInteractable(chapterIButtons, false);
        if (activeChapterIndex != 1) SetButtonsInteractable(chapterIIButtons, false);
        if (activeChapterIndex != 2) SetButtonsInteractable(chapterIIIButtons, false);
        if (activeChapterIndex != 3) SetButtonsInteractable(chapterIVButtons, false);
    }

    private void SetButtonsInteractable(Button[] buttons, bool isActive)
    {
        foreach (Button button in buttons)
        {
            button.interactable = isActive;
        }
    }

    private void ResetChapter()
    {
        currentChapter = 0;
        ActivateChapter(true);
        target.DOLocalMoveX(0, duration);
    }

    public void ActivateMainMenuButtons(bool value)
    {
        if (value) return;
        FindObjectOfType<MenuController>().ActivateButtonsWithGameObject(mainMenu);
    }

    public void ResetOnTransition() 
    {
        isOnTransition = false;

        if (!Active) return; //EventSystem.current.firstSelectedGameObject.GetComponent<Button>().Select();
        else chaptersFirstButton[currentChapter].Select();
    }
    public bool GetTransition() { return isOnTransition; }
    public int GetCurrentChapter() { return currentChapter; }
    public Button[] GetFirstSelectedButtons() { return chaptersFirstButton; } 
    public Button[] GetChaptersButtons(int index)
    {
        switch (index)
        {
            case 0: return chapterIButtons;
            case 1: return chapterIIButtons;
            case 2: return chapterIIIButtons;
            case 3: return chapterIVButtons;
            default: return null;
        }
    }
}
