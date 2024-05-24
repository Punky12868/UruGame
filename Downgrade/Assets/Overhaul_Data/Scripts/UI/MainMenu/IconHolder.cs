using UnityEngine.UI;
using UnityEngine;
using Rewired;

public class IconHolder : MonoBehaviour
{
    [Header("Icons")]
    [SerializeField] private Image[] allIcons;

    [Header("Keyboard Sprites")]
    [SerializeField] private Sprite[] keyboardIcons;

    [Header("Joystick Sprites")]
    [SerializeField] private Sprite[] joystickIcons;

    private void Update()
    {
        if (GetCurrentController.GetLastActiveController() == null) ChangeIcon(true);
        else
        {
            if (GetCurrentController.GetLastActiveController().type == ControllerType.Joystick) ChangeIcon(false);
            if (GetCurrentController.GetLastActiveController().type == ControllerType.Keyboard) ChangeIcon(true);
        }
        
    }
    private void ChangeIcon(bool value) { for (int i = 0; i < allIcons.Length; i++) allIcons[i].sprite = value ? keyboardIcons[i] : joystickIcons[i]; }
}
