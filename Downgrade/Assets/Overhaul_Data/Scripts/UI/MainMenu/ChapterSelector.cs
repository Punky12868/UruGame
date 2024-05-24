using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using DG.Tweening;
using UnityEngine.UI;

public class ChapterSelector : MonoBehaviour
{
    Player Player;

    [SerializeField] private Transform target;
    [SerializeField] private float duration = 1;
    [SerializeField] private Ease easeType = Ease.InOutExpo;
    [SerializeField] private int maxChapters = 4;
    [SerializeField] private Button[] chaptersFirstButton;

    [SerializeField] private Button[] chapterIButtons;
    [SerializeField] private Button[] chapterIIButtons;
    [SerializeField] private Button[] chapterIIIButtons;
    [SerializeField] private Button[] chapterIVButtons;

    int currentChapter = 0;
    bool active = false;
    bool isOnTransition = false;

    private void Awake() { Player = ReInput.players.GetPlayer(0); maxChapters--; }
    private void Update() 
    {
        active = FindObjectOfType<MainMenuTransition>().GetListening();
        Inputs(); 
    }

    private void Inputs()
    {
        if (!active && currentChapter != 0) ResetChapter();
        if (!active || isOnTransition) return;
        if (Player.GetButtonDown("RightTrigger")) MoveChapters(true);
        if (Player.GetButtonDown("LeftTrigger")) MoveChapters(false);
    }

    private void MoveChapters(bool value)
    {
        if (value && currentChapter < maxChapters)
        {
            currentChapter++;
            ActivateChapter(currentChapter);
            isOnTransition = true;
            Invoker.InvokeDelayed(ResetOnTransition, duration);
            target.DOLocalMoveX(target.localPosition.x - 1920, duration).SetEase(easeType);
        }
        else if (!value && currentChapter > 0)
        {
            currentChapter--;
            ActivateChapter(currentChapter);
            isOnTransition = true;
            Invoker.InvokeDelayed(ResetOnTransition, duration);
            target.DOLocalMoveX(target.localPosition.x + 1920, duration).SetEase(easeType);
        }
    }

    private void ActivateChapter(int chapterIndex)
    {
        DeactivateAllChaptersExcept(chapterIndex);
        switch (chapterIndex)
        {
            case 0:
                SetButtonsActive(chapterIButtons, true);
                break;
            case 1:
                SetButtonsActive(chapterIIButtons, true);
                break;
            case 2:
                SetButtonsActive(chapterIIIButtons, true);
                break;
            case 3:
                SetButtonsActive(chapterIVButtons, true);
                break;
        }
    }

    private void DeactivateAllChaptersExcept(int activeChapterIndex)
    {
        if (activeChapterIndex != 0) SetButtonsActive(chapterIButtons, false);
        if (activeChapterIndex != 1) SetButtonsActive(chapterIIButtons, false);
        if (activeChapterIndex != 2) SetButtonsActive(chapterIIIButtons, false);
        if (activeChapterIndex != 3) SetButtonsActive(chapterIVButtons, false);
    }

    private void SetButtonsActive(Button[] buttons, bool isActive)
    {
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(isActive);
        }
    }

    private void ResetChapter()
    {
        currentChapter = 0;
        ActivateChapter(0);
        target.DOLocalMoveX(0, duration);
    }

    public void ResetOnTransition() { isOnTransition = false; chaptersFirstButton[currentChapter].Select(); }
}
