using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerButtons : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private GameObject keyboardPanel;
    [SerializeField] private GameObject joystickPanel;
    [SerializeField] private Button keyboardButton;
    [SerializeField] private Button joystickButton;

    [SerializeField] private Sprite[] buttonSprites;
    private Button currentButton;
    private Button lastButton;

    private void Update()
    {
        if (EventSystem.current == null) return;

        GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;

        if (selectedGameObject != null && selectedGameObject.GetComponent<Button>() != null)
        {
            if (selectedGameObject.GetComponent<Button>() != currentButton)
            {
                currentButton = selectedGameObject.GetComponent<Button>();
                ChangeButtonSprite();
            }
        }

        if (FindObjectOfType<PauseMenu>().GetOnController()) lastButton = currentButton;
    }

    private void ChangeButtonSprite()
    {
        buttonImage.sprite = currentButton == keyboardButton ? buttonSprites[0] : buttonSprites[1];
        keyboardPanel.SetActive(currentButton == keyboardButton);
        joystickPanel.SetActive(currentButton != keyboardButton);
    }

    public void SelectLastButton()
    {
        if (lastButton != null) lastButton.Select();
        else keyboardButton.Select();
    }
}
