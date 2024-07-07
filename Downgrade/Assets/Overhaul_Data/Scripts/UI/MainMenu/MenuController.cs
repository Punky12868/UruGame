using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using DG.Tweening;
using Rewired;
using System;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    #region Variables

    Player input;

    [SerializeField] private GameObject eventSystem;
    [SerializeField] private Transform target;
    [SerializeField] private Canvas currentCanvas;
    [SerializeField] private Button firstSelectedButton;

    [SerializeField] private float duration = 1;
    [SerializeField] private Ease easeType = Ease.InOutExpo;

    private Button currentSelectedButton;
    private Button SelectedButton;
    private Vector3 targetPos;
    private Vector3 targetRot;
    private bool onInputCooldown = false;
    private SliderListener sliderListener;

    private List<MenuHistory> menuHistory = new List<MenuHistory>();

    [HideInInspector] public event Action OnSaveHistory;
    [HideInInspector] public event Action OnLoadHistory;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (target == null || currentCanvas == null || firstSelectedButton == null) this.enabled = false;

        input = ReInput.players.GetPlayer(0);
        DeactivateAllButtons();

        SelectedButton = firstSelectedButton;
        targetPos = target.position;
        targetRot = target.rotation.eulerAngles;

        SaveHistory();
    }

    private void Update() 
    { 
        if (input.GetButtonDown("Cancel") && !onInputCooldown) LoadHistory();
        //eventSystem.SetActive(!onInputCooldown);

        if (EventSystem.current == null) return;
        GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
        if (selectedGameObject != null && selectedGameObject.GetComponent<Button>() != null && selectedGameObject.GetComponent<Button>() != currentSelectedButton)
        {
            currentSelectedButton = selectedGameObject.GetComponent<Button>();
        }
    }

    #endregion

    #region Buttons Behaviours

    public void GetNewCanvas(Canvas newCanvas)
    {
        if (newCanvas == null || currentCanvas == newCanvas) return;

        Button[] oldCanvasButtons = currentCanvas.GetComponentsInChildren<Button>();
        Button[] newCanvasButtons = newCanvas.GetComponentsInChildren<Button>();

        foreach (Button button in oldCanvasButtons) button.interactable = false;

        foreach (Button button in newCanvasButtons) if (!button.transform.GetComponentInParent<Slider>()) button.interactable = true;

        currentCanvas = newCanvas;
    }

    public void SelectButton(Button button)
    {
        SelectedButton = button;
        Invoke("InvokeButtonSelection", duration);
    }

    public void SelectButtonWithoutCooldown(Button button)
    {
        SelectedButton = button;
        Invoke("InvokeButtonSelection", 0.1f);
    }

    public void GetNewPos(Transform pos)
    {
        if (pos == null || target.position == pos.position && target.rotation == pos.rotation) return;

        target.DOMove(pos.position, duration).SetEase(easeType);
        target.DORotate(pos.rotation.eulerAngles, duration).SetEase(easeType);

        targetPos = pos.position;
        targetRot = pos.rotation.eulerAngles;

        InvokeMethods();
    }

    public void CancelButtonSim()
    {
        LoadHistory();
    }

    #endregion

    #region Lists History

    public void SaveHistory()
    {
        MenuHistory history = new MenuHistory();
        history.canvas = currentCanvas;
        history.button = currentSelectedButton;
        history.pos = targetPos;
        history.rot = targetRot;

        menuHistory.Add(history);
        OnSaveHistory?.Invoke();
    }

    public void LoadHistory()
    {
        if (menuHistory.Count > 1)
        {
            GetNewCanvas(menuHistory[menuHistory.Count - 2].canvas);
            GetNewPosFromHistory(menuHistory[menuHistory.Count - 2].pos, menuHistory[menuHistory.Count - 2].rot);
            SelectButtonWithoutCooldown(menuHistory[menuHistory.Count - 1].button);
            menuHistory.RemoveAt(menuHistory.Count - 1);
            
            OnLoadHistory?.Invoke();
        }
    }

    #endregion

    #region Utility

    private void GetNewPosFromHistory(Vector3 pos, Vector3 rot)
    {
        target.DOMove(pos, duration).SetEase(easeType);
        target.DORotate(rot, duration).SetEase(easeType);

        targetPos = pos;
        targetRot = rot;

        InvokeMethods();
    }

    public void DeactivateButtonsWithGameObject(GameObject go)
    {
        Button[] buttons = go.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++) buttons[i].interactable = false;
    }

    public void ActivateButtonsWithGameObject(GameObject go)
    {
        Button[] buttons = go.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++) buttons[i].interactable = true;
    }

    private void DeactivateAllButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        for (int i = 0; i < buttons.Length; i++) if (buttons[i].transform.GetComponentInParent<Canvas>() != currentCanvas) buttons[i].interactable = false;
    }

    public void SetSliderListener(SliderListener slider) { sliderListener = slider; }
    public SliderListener GetSliderListener() { return sliderListener; }

    public Canvas GetCurrentCanvas() { return currentCanvas;}

    private void InvokeMethods()
    {
        if (onInputCooldown)
        {
            Invoker.CancelInvoke(InvokeInputCooldown);
            Invoker.CancelInvoke(InvokeButtonSelection);
        }

        if (sliderListener != null)
        {
            onInputCooldown = true;
            eventSystem.SetActive(false);
            Invoker.InvokeDelayed(InvokeInputCooldown, 0.1f);
            Invoker.InvokeDelayed(InvokeButtonSelection, 0.1f);
            Invoker.InvokeDelayed(InvokeButtonDeactivation, 0.1f);
            sliderListener = null;
        }
        else
        {
            onInputCooldown = true;
            eventSystem.SetActive(false);
            Invoker.InvokeDelayed(InvokeInputCooldown, duration);
            Invoker.InvokeDelayed(InvokeButtonSelection, duration);
            Invoker.InvokeDelayed(InvokeButtonDeactivation, 0.1f);
        }
        
    }

    private void InvokeButtonSelection() {SelectedButton.Select();}
    private void InvokeInputCooldown() {onInputCooldown = false; eventSystem.SetActive(true); }
    private void InvokeButtonDeactivation() {DeactivateAllButtons();}
    public bool IsOnInputCooldown() { return onInputCooldown; }

    #endregion
}

#region Serializable Classes
[Serializable]
public class MenuHistory
{
    public Canvas canvas;
    public Button button;
    public Vector3 pos;
    public Vector3 rot;
}
#endregion