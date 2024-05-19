using UnityEngine.UI;
using UnityEngine;

public class ButtonStatusProxy : MonoBehaviour
{
    MenuController menuController;

    private void Awake()
    {
        menuController = FindObjectOfType<MenuController>();
        menuController.OnLoadHistory += DisableButton;
    }

    private void DisableButton() 
    {
        GetComponent<Button>().interactable = false;

        Button[] currentCanvasButtons = GetComponentInParent<Canvas>().gameObject.GetComponentsInChildren<Button>();
        foreach (Button button in currentCanvasButtons) if (!button.transform.GetComponentInParent<Slider>()) button.interactable = true;
    }
}
