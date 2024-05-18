using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Canvas currentCanvas;
    [SerializeField] private Button firstSelectedButton;

    [SerializeField] private float duration = 1;
    [SerializeField] private Ease easeType = Ease.InOutExpo;

    private Button SelectedButton;
    private Vector3 targetPos;
    private Vector3 targetRot;

    [SerializeField] private Stack<MenuHistory> menuHistory = new Stack<MenuHistory>();

    private void Awake()
    {
        if (target == null || currentCanvas == null || firstSelectedButton == null) this.enabled = false;

        DeactivateAllButtons();

        SelectedButton = firstSelectedButton;
        SaveHistory();
    }

    public void GetNewCanvas(Canvas newCanvas)
    {
        Button[] oldCanvasButtons = currentCanvas.GetComponentsInChildren<Button>();
        Button[] newCanvasButtons = newCanvas.GetComponentsInChildren<Button>();

        foreach (Button button in oldCanvasButtons)
        {
            button.interactable = false;
        }

        foreach (Button button in newCanvasButtons)
        {
            button.interactable = true;
        }

        currentCanvas = newCanvas;
    }

    public void SelectButton(Button button)
    {
        SelectedButton = button;
        Invoke("InvokeButtonSelection", duration);
    }

    public void GetNewPos(Transform pos)
    {
        target.DOMove(pos.position, duration).SetEase(easeType);
        target.DORotate(pos.rotation.eulerAngles, duration).SetEase(easeType);

        targetPos = pos.position;
        targetRot = pos.rotation.eulerAngles;
    }

    public void GetNewPosFromHistory(Vector3 pos, Vector3 rot)
    {
        target.DOMove(pos, duration).SetEase(easeType);
        target.DORotate(rot, duration).SetEase(easeType);

        targetPos = pos;
        targetRot = rot;
    }

    private void InvokeButtonSelection()
    {
        SelectedButton.Select();
    }

    #region LIFO History

    public void SaveHistory()
    {
        MenuHistory history = new MenuHistory
        {
            canvas = currentCanvas,
            button = SelectedButton,
            pos = targetPos,
            rot = targetRot
        };

        menuHistory.Push(history);
    }

    public void LoadHistory()
    {
        if (menuHistory.Count > 0)
        {
            MenuHistory history = menuHistory.Pop();

            GetNewCanvas(history.canvas);
            SelectButton(history.button);
            GetNewPosFromHistory(history.pos, history.rot);
        }
    }

    #endregion

    #region Utility

    private void DeactivateAllButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].transform.parent == currentCanvas.transform) continue;
            buttons[i].interactable = false;
        }
    }

    #endregion
}

[Serializable]
public class MenuHistory
{
    public Canvas canvas;
    public Button button;
    public Vector3 pos;
    public Vector3 rot;
}
