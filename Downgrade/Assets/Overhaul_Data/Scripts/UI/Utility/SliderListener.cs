using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Rewired;

public class SliderListener : MonoBehaviour
{
    private Player input;
    private UIOptionsController uiOptionsController;

    [SerializeField] private Button currentSelectedButton;
    private bool buttonDown;

    [SerializeField] private bool onGameplay;

    private void Awake()
    {
        input = ReInput.players.GetPlayer(0);
        uiOptionsController = GetComponent<UIOptionsController>();
    }

    private void Update()
    {
        Inputs();
    }

    private void Inputs()
    {
        if (input.GetAxisRaw("H") == 0 && !input.GetAnyButton()) buttonDown = false;
        if (!IsListening()) return;
        SetListening();

        if (input.GetAxisRaw("H") > 0 && !buttonDown)
        {
            uiOptionsController.IncreaseDecreaseSliderValue(1);
            buttonDown = true;
        }

        if (input.GetAxisRaw("H") < 0 && !buttonDown)
        {
            uiOptionsController.IncreaseDecreaseSliderValue(-1);
            buttonDown = true;
        }

    }

    private void SetListening()
    {
        if (onGameplay) return;
        if (IsListening() && FindObjectOfType<MenuController>().GetSliderListener() != this)
        {
            FindObjectOfType<MenuController>().SetSliderListener(this);
        }
    }

    private bool IsListening()
    {
        if (EventSystem.current == null) return false;
        GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
        if (selectedGameObject == null) return false;
        if (selectedGameObject.GetComponent<Button>() != currentSelectedButton)
        {
            return false;
        }
        else return true;
    }

    public bool GetListening()
    {
        return IsListening();
    }
}
